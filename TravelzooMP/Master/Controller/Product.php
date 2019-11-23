<?php
/**
 * 后台商品管理
 *
 */
class Product extends Controller
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
		$perPage = 20;
		$nowPage = (isset($getArr["p"]) && $getArr["p"]>=1) ? $getArr["p"] : 1;

		$expiryTime = date("Y-m-d H:i:s",time()+24*60*60);
		//取列表
		$whereArr = "ParentDealId=0 AND ((ChildDeals=0 AND IsPurchasable=1) OR (ChildDeals=1 AND IsPurchasable=0)) AND IsSoldOut=0 AND InternalStatus='Published' AND PurchaseExpiryDateUtc > '$expiryTime'";
		
		$orderBy = "rank DESC,PublishDateUtc DESC";
		if ( isset($getArr["type"]) && !empty($getArr["type"]) ) {
			$whereArr .= " AND type='{$getArr["type"]}'";
		}
		if ( isset($getArr["banner"]) && !empty($getArr["banner"]) ) {
			$whereArr .= " AND banner_id='{$getArr["banner"]}'";
			$orderBy = "banner_rank DESC,PublishDateUtc DESC";
		}

		if( !isset($getArr["svalue"]) ) $getArr["svalue"] = "";
		if ( $getArr["skey"]=="Title" ) {
			$whereArr .= " AND Title like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="Id" ){
			$whereArr .= " AND Id = '{$getArr["svalue"]}'";
		}elseif( $getArr["skey"]=="Price" ){
			$whereArr .= " AND Price = '{$getArr["svalue"]}'";
		}elseif( $getArr["skey"]=="LocationName" ){
			$whereArr .= " AND LocationName like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="ByLine" ){
			$whereArr .= " AND ByLine like '%{$getArr["svalue"]}%'";
		}
		
		$data["getArr"] = $getArr;

		$itemObjR = Factory::loadModel("Product");
		$itemList = $itemObjR->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,$orderBy);
		$data["itemList"] = $itemList;
		//得到总数
		$totalNum = $itemObjR->getCount($whereArr);
		//分页设置
		$page = new Page(array("total"=>$totalNum,"perpage"=>$perPage,"nowindex"=>$nowPage));
		$data["page"] = $page->show(99);
		
		//商品分类
		$recommendModel = Factory::loadModel("Recommend");
		$typeList = $recommendModel->getItemList("type=3 AND status=1 AND is_del=0","","rank DESC,id ASC");
		$data["typeList"] = $typeList;
		//banner列表
		$bannerList = $recommendModel->getItemList("type=2 AND status=1 AND is_del=0","","rank DESC,id ASC");
		$data["bannerList"] = $bannerList;

		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("product/product_index.html",$data);
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
			/*
			if($_FILES['img_arr']['name'] != ""){
				if($_FILES['img_arr']['error'] == 1){
					js_show("alert('上传失败，上传文件超过2M');");
					exit();
				}
				//建文件夹
				$photoDir = "../Web/images/upload/product/".date("Ym");
				File::mkDir($photoDir, 0777);
				//上传文件
				$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
				$upFileObj = new UpFile($photoDir, $fileFormat);
				$upFileObj->run("img_arr", 1);
				$upFileArr = $upFileObj->getInfo();
			}
			*/
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("Product",$master);
			$itemId = $itemObjW->addItem($postArr,$upFileArr);
			js_show("alert('新增成功！');parent.location.href='/product_index.html'");
			exit();
		}

		// $typeObj = Factory::loadModel("ProductCategory");
		// $typeList = $typeObj->getItemList("pid=0 AND is_del=0","","rank ASC,id ASC");
		// $data["typeList"] = $typeList;

		//属性列表
		// $attrList = unserialize(PRODUCT_ATTR_LIST);
		// $data["attrList"] = $attrList;
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("product/product_add.html",$data);
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

			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("Product",$master);
			if ($postArr["status"] == "1") {//已售罄
				foreach ($postArr["pidArr"] as $pid) {
					$itemObjW->updateOne("IsPurchasable=0,noUpdate=1","Id='$pid'");
				}
			}elseif ($postArr["status"] == "2") {//可售
				foreach ($postArr["pidArr"] as $pid) {
					$itemObjW->updateOne("IsPurchasable=1,noUpdate=0","Id='$pid'");
				}
			}elseif ($postArr["status"] == "3") {//删除
				foreach ($postArr["pidArr"] as $pid) {
					$itemObjW->delItem($pid);
				}
			}
			//写DB
			$itemObjW->updateItem($postArr,"id='{$postArr["eid"]}'");
			js_show("alert('修改成功！');parent.location.href='{$postArr["frontURL"]}'");
			exit();
		}
		$getArr = $this->request->get();
		$itemId = $getArr["eid"];
		//取单条值
		$itemObjR = Factory::loadModel("Product");
		$itemArr = $itemObjR->getOneItem("Id='$itemId'");
		if(empty($itemArr) || $itemArr["is_del"] == "1"){
			js_msg_go("此信息不存在或已被删除",$data["frontURL"]);
			exit();
		}
		$data["itemArr"] = $itemArr;

		//商品分类
		$recommendModel = Factory::loadModel("Recommend");
		$typeList = $recommendModel->getItemList("type=3 AND status=1 AND is_del=0","","rank DESC,id ASC");
		$data["typeList"] = $typeList;
		//banner列表
		$bannerList = $recommendModel->getItemList("type=2 AND status=1 AND is_del=0","","rank DESC,id ASC");
		$data["bannerList"] = $bannerList;

		//属性列表
		// $attrList = unserialize(PRODUCT_ATTR_LIST);
		// $data["attrList"] = $attrList;

		//加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("product/product_edit.html",$data);
	}
	/**
	 * 接口名称：删除项目
	 * URL：	/product_del.html
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
		$itemObjW = Factory::loadModel("Product",$master);
		$itemObjW->delItem($itemId);
		exit("1");
	}
	/**
	 * 接口名称：单独保存某一项的值
	 * URL：	/activity_setItem.html
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
		$itemObjW = Factory::loadModel("Product",$master);
		$itemObjW->updateOne("{$postArr["item"]}='{$postArr["value"]}'","Id='{$postArr["eid"]}'");
		exit("1");
    }
	 /**
	 * 接口名称：设置排序值
	 * URL：	/product_setRank.html
	 * 方式：	POST
	 * 参数：	eid
	 * 返回：	-1 未登录，-2 参数错误，1 成功
	 */
    public function setRank(){
		//登录验证
		$psObj = new PublicService();
		$isLogin = $psObj->checkLoginBackend(true);
		$uid = Cookie::get("tybkud",true);
		if(!$isLogin) exit("-1");
		
		$postArr = $this->request->getPost();
		$itemId = $postArr["eid"];
		if(!is_numeric($itemId)) exit("-2");
		
		$master = Factory::loadDB("master");
		$itemObjW = Factory::loadModel("Product",$master);
		$itemObjW->updateOne("rank={$postArr["rank"]}","Id='$itemId'");
		exit("1");
    }
	//ajax取子分类
	public function getSubclass(){
		$_GET = $this->request->get();
		$pid = $_GET["pid"];
		if( empty($pid) )	exit();
		
		$itemObjR = Factory::loadModel("ProductCategory");
		$itemList = $itemObjR->getItemList("pid='$pid' AND status=0 AND is_del=0","","rank ASC,id ASC");
		$html = '<option value="0">请选择</option>';
		if (!empty($itemList)){
			foreach ($itemList as $key=>$val){
				$html .= "<option value=\"{$val["id"]}\">{$val["name"]}</option>";
			}
		}
		echo $html;
	}
	
	
}
?>