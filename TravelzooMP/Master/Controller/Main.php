<?php
/**
 * 后台管理
 */
class Main extends Controller
{

    public function index(){
        //登录验证
		$psObj = new PublicService();
		$isLogin = $psObj->checkLoginBackend(true);
		if(!$isLogin){
			redirect("/main_login.html");
		}   
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("init/index.html" ,$data);
    }
	/**
	 * 登录页 
	 */
	public function login(){
		//处理提交
		if ($this->request->isPostBack()) {
			$postArr = $this->request->getPOST();
			$fromUrl = decode_str($postArr["from_url"]);
			$userObjR = Factory::loadModel("BkUser");
			$userArr = $userObjR->getOne("nickname='{$postArr["nickname"]}'");
			//dump($userArr);exit();
			if(empty($userArr)){
				js_msg_back("用户名或密码错误");
				exit();
			}
			if(md5($postArr["user_pwd"]) == $userArr["user_pwd"]){
				Cookie::set("tybkud",$userArr["id"],259200,true,MY_APP_DOMAIN);
				$authCode = get_auth_code($userArr["id"]);
				Cookie::set("tybkau",$authCode,259200);
				$_SESSION['bkUserArr'] = $userArr;
				//记录登录时间
				//$psObj = new PublicService();
				//$psObj->setLastOnlineTime($userArr["id"]);
				empty($fromUrl) ? js_show("parent.location.href='/'") : js_show("parent.location.href='$fromUrl'");
				exit();
			}else{
				//js_show("alert('密码错误~！请重新填写');");
				js_msg_back("用户名或密码错误");
				exit();
			}
		}

		//得到参数
		$_GET = $this->request->get();
		$data["fromUrl"] = $_GET["f"];
		//加载视图
		$data['no_visible_elements'] = 1;
		$data['header'] = PublicService::getHeaderHtmlBackend($data);
		$data['footer'] = PublicService::getFooterHtmlBackend($data);		
		return View::parse('init/login.html', $data);
    }
    
    /**
     * 导出
     */
    public function output(){
    		$getArr = $this->request->get();
    		if (empty($getArr["model"])) {
    			js_msg_back("参数错误");
    		}
    		$where = "";
    		
    		switch ($getArr["model"]){
    			case 'LotteryKey': $where = "status = 1";
    				break;
    			default: $where = "1";
    				break;
    		}
    		
    		if (!empty($getArr["beginid"]) && !empty($getArr["endid"])) {
    			$where .= " and id between {$getArr['beginid']} and {$getArr['endid']}";
    		}
    		
    		
    		$itemModel = Factory::loadModel($getArr["model"]);
    		$itemList = $itemModel->getList($where);
    		
    		$t = ',';
    		$outputFileName = $getArr["model"].time().".csv";
    		$outputFileName = mb_convert_encoding($outputFileName,'gb2312','UTF-8');
    		
    		header("Content-Type: application/vnd.ms-execl");
    		header("Content-Disposition: attachment; filename=".$outputFileName);
    		header("Pragma: no-cache");
    		header("Expires: 0");
    		$outputTitle = $itemModel->outputTitle;
    		$outputField = $itemModel->outputField;
    		$title = str_replace("|", $t, $outputTitle);
    		$title .= "\n";   		
    		$fieldArr = explode("|", $outputField);
    		$con = "";
    		foreach ($itemList as $arr)
    		{
    			foreach ($fieldArr as $field){
    				$str = str_replace(",", "，", $arr[$field]);
    				$con .= $str.$t;
    			}
    			$con .= "\n";    		
    		}
    		$output = $title.$con;
    		$output = mb_convert_encoding($output,'gb2312','UTF-8');
    		echo $output;
    }
    
    
    
	/**
	 * 退出登录
	 *
	 */
	public function logout(){
		Cookie::del("tybkud");
		Cookie::del("tybkau");
		Session::del("bkUserArr");
		redirect("/");
	}
	
}
?>