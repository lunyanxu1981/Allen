<?php
/**
 * 用户优惠券管理
 *
 */
class UserCoupon extends Controller
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
		$getArr = $this->request->get();
		$perPage = 30;
		$nowPage = (isset($getArr["p"]) && $getArr["p"]>=1) ? $getArr["p"] : 1;
		
		//取列表
		$whereArr = "1=1";
		/*
		if ( isset($getArr["type"]) && !empty($getArr["type"]) ) {
			$whereArr .= " AND type='{$getArr["type"]}'";
		}
		if ( isset($getArr["category"]) && !empty($getArr["category"]) ) {
			$whereArr .= " AND category='{$getArr["category"]}'";
		}
		*/
		
		if( !isset($getArr["svalue"]) ) $getArr["svalue"] = "";
		
		if ( $getArr["skey"]=="uid" ) {
			$whereArr .= " AND uid = '{$getArr["svalue"]}'";
		}
		
		$data["getArr"] = $getArr;

		$orderBy = "";
		if ( !empty($getArr["order"]) && !empty($getArr["by"]) ) {
			$orderBy .= "{$getArr["order"]} {$getArr["by"]},";
		}
		$orderBy .= "id DESC";

		$itemObjR = Factory::loadModel("UserCoupon");
		$itemList = $itemObjR->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,$orderBy);
		$data["itemList"] = $itemList;
		//得到总数
		$totalNum = $itemObjR->getCount($whereArr);
		//分页设置
		$page = new Page(array("total"=>$totalNum,"perpage"=>$perPage,"nowindex"=>$nowPage));
		$data["page"] = $page->show(99);
		
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("coupon/userCoupon_index.html",$data);
    }
	/**
	 * 新增
	 */
    public function add(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
		$data["frontURL"] = $this->request->frontURL();
		//处理提交
		if ($this->request->isPostBack()) {
			$postArr = $this->request->getPost();
			// dump($postArr);
			
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("UserCoupon",$master);
			$itemId = $itemObjW->addItem($postArr,$upFileArr);
			js_show("alert('新增成功！');parent.location.href='/userCoupon_index.html'");
			exit();
		}
		// $data["typeList"] = get_one_type_list(0);
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("coupon/userCoupon_add.html",$data);
    }
	/**
	 * 编辑
	 */
	public function edit(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
		$data["frontURL"] = $this->request->frontURL();
		// 处理提交
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("UserCoupon",$master);
			$itemObjW->updateItem($postArr,"id='{$postArr["eid"]}'",$upFileArr);
			js_show("alert('修改成功！');parent.location.href='{$postArr["frontURL"]}'");
			exit();
		}
		$getArr = $this->request->get();
		$itemId = $getArr["eid"];
		//取单条值
		$itemObjR = Factory::loadModel("UserCoupon");
		$itemArr = $itemObjR->getOneItem("id='$itemId'");
		if(empty($itemArr) || $itemArr["is_del"] == "1"){
			js_msg_go("此信息不存在或已被删除",$data["frontURL"]);
			exit();
		}
		$data["itemArr"] = $itemArr;
		// $data["typeList"] = get_one_type_list(0);
		//加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("coupon/userCoupon_edit.html",$data);
	}
	/**
	 * 接口名称：删除项目
	 * URL：	/userCoupon_del.html
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
		$itemObjW = Factory::loadModel("UserCoupon",$master);
		$itemObjW->delItem($itemId);
		exit("1");
	}
	/**
	 * 接口名称：单独保存某一项的值
	 * URL：	/userCoupon_setItem.html
	 * 方式：	POST
	 * 参数：	eid,item,value
	 * 返回：	-1 未登录，-2 参数错误，1 成功
	 */
    public function setItem(){
		//登录验证
		$psObj = new PublicService();
		$isLogin = $psObj->checkLoginBackend(true);
		$uid = Cookie::get("tybkud",true);
		if(!$isLogin) exit("-1");
		
		$postArr = $this->request->getPost();
		if(!is_numeric($postArr["eid"])) exit("-2");
		
		$master = Factory::loadDB("master");
		$itemObjW = Factory::loadModel("UserCoupon",$master);
		$itemObjW->updateOne("{$postArr["item"]}='{$postArr["value"]}'","id='{$postArr["eid"]}'");
		exit("1");
    }
	//ajax取子分类
	public function getSubclass(){
		// $_GET = $this->request->get();
		// $pid = $_GET["pid"];
		// if( empty($pid) )	exit();
		
		// $itemObjR = Factory::loadModel("Tree");
		// $itemList = $itemObjR->getItemList("pid='$pid' AND status=0","","id ASC");
		// $html = '<option value="0">请选择</option>';
		// if (!empty($itemList)){
		// 	foreach ($itemList as $key=>$val){
		// 		$html .= "<option value=\"{$val["id"]}\">{$val["title"]}</option>";
		// 	}
		// }
		// echo $html;
	}
	
	
}
?>