<?php
/**
 * 后台拼团产品管理
 *
 */
class ProductGroup extends Controller
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
		$whereArr = "is_del=0";
		/*
		if ( isset($getArr["type"]) && !empty($getArr["type"]) ) {
			$whereArr .= " AND type='{$getArr["type"]}'";
		}
		if ( isset($getArr["category"]) && !empty($getArr["category"]) ) {
			$whereArr .= " AND category='{$getArr["category"]}'";
		}
		*/
		/*
		if( !isset($getArr["svalue"]) ) $getArr["svalue"] = "";
		
		if ( $getArr["skey"]=="phone" ) {
			$whereArr .= " AND phone like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="member_code" ){
			$whereArr .= " AND member_code like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="realname" ){
			$whereArr .= " AND realname like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="waitCheck" ){
			$whereArr .= " AND idcard_check=0 AND idcard!=''";
		}elseif( $getArr["skey"]=="checked" ){
			$whereArr .= " AND idcard_check>0";
		}
		*/
		$data["getArr"] = $getArr;

		$orderBy = "";
		if ( !empty($getArr["order"]) && !empty($getArr["by"]) ) {
			$orderBy .= "{$getArr["order"]} {$getArr["by"]},";
		}
		$orderBy .= "rank DESC,id DESC";

		$itemObjR = Factory::loadModel("ProductGroup");
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
		return View::parse("product/productGroup_index.html",$data);
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
			
			if($_FILES['img_arr']['name'] != ""){
				if($_FILES['img_arr']['error'] == 1){
					js_show("alert('上传失败，上传文件超过2M');");
					exit();
				}
				//建文件夹
				$photoDir = "../Web/images/upload/productGroup/".date("Ym");
				File::mkDir($photoDir, 0777);
				//上传文件
				$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
				$upFileObj = new UpFile($photoDir, $fileFormat);
				$upFileObj->run("img_arr", 1);
				$upFileArr = $upFileObj->getInfo();
			}
			
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("ProductGroup",$master);
			$itemId = $itemObjW->addItem($postArr,$upFileArr);
			js_show("alert('新增成功！');parent.location.href='/productGroup_index.html'");
			exit();
		}
		// $data["typeList"] = get_one_type_list(0);
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("product/productGroup_add.html",$data);
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
			
			if($_FILES['img_arr']['name'] != ""){
				if($_FILES['img_arr']['error'] == 1){
					js_show("alert('上传失败，上传文件超过2M');");
					exit();
				}
				//建文件夹
				$photoDir = "../Web/images/upload/productGroup/".date("Ym");
				File::mkDir($photoDir, 0777);
				//上传文件
				$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
				$upFileObj = new UpFile($photoDir, $fileFormat);
				$upFileObj->run("img_arr", 1);
				$upFileArr = $upFileObj->getInfo();
			}
			
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("ProductGroup",$master);
			$itemObjW->updateItem($postArr,"id='{$postArr["eid"]}'",$upFileArr);
			js_show("alert('修改成功！');parent.location.href='{$postArr["frontURL"]}'");
			exit();
		}
		$getArr = $this->request->get();
		$itemId = $getArr["eid"];
		//取单条值
		$itemObjR = Factory::loadModel("ProductGroup");
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
		return View::parse("product/productGroup_edit.html",$data);
	}
	/**
	 * 接口名称：删除项目
	 * URL：	/productGroup_del.html
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
		$itemObjW = Factory::loadModel("ProductGroup",$master);
		$itemObjW->delItem($itemId);
		exit("1");
	}
	/**
	 * 接口名称：单独保存某一项的值
	 * URL：	/productGroup_setItem.html
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
		$itemObjW = Factory::loadModel("ProductGroup",$master);
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
	/**
	 * 接口名称：获取产品详情
	 * URL：	/productGroup_getProductDetail.html
	 * 方式：	POST
	 * 参数：dealid tzoo产品id
	 * 返回：200:成功
	 *		401:请输入正确的DealId
	 *      {"status":"200","data":array}
	 */
	public function getProductDetail(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["dealid"]) ){
				echo json_encode(array("status"=>401,"data"=>"请输入正确的DealId"));
				exit();
			}
			//查看产品详情
			$productModel = Factory::loadModel("Product");
			$productArr = $productModel->getOne("Id='{$postArr["dealid"]}'");
			echo json_encode(array("status"=>200,"data"=>$productArr));
		}
	}
	
}
?>