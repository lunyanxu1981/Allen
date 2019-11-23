<?php
/**
 * 后台用户管理
 *
 */
class BkUser extends Controller
{    
	/**
	 * 查询
	 */
    public function index(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
        //得到参数
		$_GET = $this->request->get();
		$perPage = 20;
		$nowPage = (isset($_GET["p"]) && $_GET["p"]>=1) ? $_GET["p"] : 1;
		//取列表
		$whereArr = "is_show=1";
		$itemObjR = Factory::loadModel("BkUser");
		$itemList = $itemObjR->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,"id ASC");
		$data["itemList"] = $itemList;
		//得到总数
		$totalNum = $itemObjR->getCount($whereArr);
		//分页设置
		$page = new Page(array("total"=>$totalNum,"perpage"=>$perPage,"nowindex"=>$nowPage));
		$data["page"] = $page->show(20);
		
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("bkuser_index.html",$data);
    }
	/**
	 * 新增
	 */
    public function add(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
		//处理提交
		if ($this->request->isPostBack()) {
			$postArr = $this->request->getPost();
			//dump($postArr);
			/* 
			if($_FILES['img_arr']['name'] != ""){
				if($_FILES['img_arr']['error'] == 1){
					js_show("alert('上传失败，上传文件超过2M');");
					exit();
				}
				//建文件夹
				$photoDir = "../../tuye3/images/upload/user/";
				File::mkDir($photoDir, 0777);
				//上传文件
				$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
				$upFileObj = new UpFile($photoDir, $fileFormat);
				$upFileObj->run("img_arr", 1);
				$upFileArr = $upFileObj->getInfo();
			} */
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("BkUser",$master);
			$itemId = $itemObjW->addItem($postArr,$upFileArr);
			js_show("alert('新增成功！');parent.location.href='/bkuser_index.html'");
			exit();
		}
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("bkuser_add.html",$data);
    }
	/**
	 * 编辑
	 */
	public function edit(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
		// 处理提交
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			/* 
			if($_FILES['img_arr']['name'] != ""){
				if($_FILES['img_arr']['error'] == 1){
					js_show("alert('上传失败，上传文件超过2M');");
					exit();
				}
				//建文件夹
				$photoDir = "../../tuye3/images/upload/user/";
				File::mkDir($photoDir, 0777);
				//上传文件
				$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
				$upFileObj = new UpFile($photoDir, $fileFormat);
				$upFileObj->run("img_arr", 1);
				$upFileArr = $upFileObj->getInfo();
			} */
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("BkUser",$master);
			$itemObjW->updateItem($postArr,$upFileArr);
			js_show("alert('修改成功！');parent.location.href='/bkuser_index.html'");
			exit();
		}
		$_GET = $this->request->get();
		$itemId = $_GET["eid"];
		//取单条值
		$itemObjR = Factory::loadModel("BkUser");
		$itemArr = $itemObjR->getOneItem($itemId);
		if(empty($itemArr) || $itemArr["is_deleted"] == "1"){
			js_msg_go("此信息不存在或已被删除","/user_index.html");
			exit();
		}
		$data["itemArr"] = $itemArr;
		//加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("bkuser_edit.html",$data);
	}
	/**
	 * 修改密码
	 */
	public function editpwd(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
		// 处理提交
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			$newPwd = md5($postArr["new_pwd"]);
			
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("BkUser",$master);
			//判断原密码是否正确
			$itemArr = $itemObjW->getOne("id='$uid'");
			//dump($itemArr);exit();
			if(md5($postArr["ori_pwd"]) != $itemArr["user_pwd"] ){
				js_show("alert('原密码不正确');");
				exit();
			}
			//写DB
			$itemObjW->updateOne("user_pwd='$newPwd'","id='$uid'");
			js_show("alert('修改成功！');parent.location.href='/bkuser_index.html'");
			exit();
		}
		//加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("bkuser_editpwd.html",$data);
	}
	/**
	 * 接口名称：删除项目
	 * URL：	/user_del.html
	 * 方式：	POST
	 * 参数：	eid
	 * 返回：	-1 未登录，-2 参数错误，1 成功
	 */
	public function del(){
		//登录验证
		$psObj = new PublicService();
		$isLogin = $psObj->checkLoginBackend(true);
		$uid = Cookie::get("tybkud",true);
		if(!$isLogin) exit("-1");
		
		$postArr = $this->request->getPost();
		$itemId = $postArr["eid"];
		if(!is_numeric($itemId)) exit("-2");
		
		$master = Factory::loadDB("master");
		$itemObjW = Factory::loadModel("BkUser",$master);
		$itemObjW->delItem($itemId);
		exit("1");
	}
	
	
}
?>