<?php
/**
 * 后台订单管理
 *
 */
class UserOrder extends Controller
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
		
		if ( isset($getArr["type"]) && !empty($getArr["type"]) ) {
			$productModel = Factory::loadModel("Product");
			$productList = $productModel->getList("Category='{$getArr["type"]}'","","");
			foreach ($productList as $productArr) {
				if(!empty($pidStr)) $pidStr .= ",";
				$pidStr .= $productArr["Id"];
			}
			$whereArr .= " AND pid in($pidStr)";
		}
		/*if ( isset($getArr["category"]) && !empty($getArr["category"]) ) {
			$whereArr .= " AND category='{$getArr["category"]}'";
		}
		*/
		
		if( !isset($getArr["svalue"]) ) $getArr["svalue"] = "";
		
		if ( $getArr["skey"]=="title" ) {
			$pidStr = "";
			$productModel = Factory::loadModel("Product");
			$productList = $productModel->getList("Title like '%{$getArr["svalue"]}%'","","");
			foreach ($productList as $productArr) {
				if(!empty($pidStr)) $pidStr .= ",";
				$pidStr .= $productArr["Id"];
			}
			$whereArr .= " AND pid in($pidStr)";
		}elseif ( $getArr["skey"]=="order_no" ) {
			$whereArr .= " AND order_no like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="bar_code" ){//先去详单详情表中找对应oid
			$detailModel = Factory::loadModel("UserOrderDetail");
			$detailArr = $detailModel->getOne("bar_code like '%{$getArr["svalue"]}%'");
			$whereArr .= " AND id='{$detailArr["oid"]}'";
		}elseif( $getArr["skey"]=="email" ){
			$whereArr .= " AND email like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="phone" ){
			$whereArr .= " AND phone like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="username" ){
			$whereArr .= " AND username like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="total_price" ){
			$whereArr .= " AND total_price like '%{$getArr["svalue"]}%'";
		}elseif( $getArr["skey"]=="status" ){
			$whereArr .= " AND status = '{$getArr["svalue"]}'";
		}

		$today = date("Y-m-d");
		$startDay = date("Y-m-d",strtotime("-7 day"));
		$getArr["start"] = $startDay." 00:00:00";
		$getArr["end"] = $today." 23:59:59";
		
		$data["getArr"] = $getArr;

		$itemObjR = Factory::loadModel("UserOrder");
		if ($getArr["special"] == "1") {//只显示那些特别的订单，购买成功但未生成兑换券的
			$whereArr .= " AND status=1 AND is_ok=0";
			$itemList = $itemObjR->getItemList($whereArr,"","id DESC");
			$speList = "";
			foreach ($itemList as $itemArr) {
				if (empty($itemArr["detailList"])) {
					$speList[] = $itemArr;
				}else{//对没有问题的订单做一下标志，下次就不会再次查询了
					$itemObjR->updateOne("is_ok=1","id='{$itemArr["id"]}'");
				}
			}
			$itemList = $speList;
		}else{
			$itemList = $itemObjR->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,"id DESC");
			//得到总数
			$totalNum = $itemObjR->getCount($whereArr);
			//分页设置
			$page = new Page(array("total"=>$totalNum,"perpage"=>$perPage,"nowindex"=>$nowPage));
			$data["page"] = $page->show(99);
		}
		$data["itemList"] = $itemList;
		
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("user/userOrder_index.html",$data);
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
			$itemObjW = Factory::loadModel("UserOrder",$master);
			$itemId = $itemObjW->addItem($postArr,$upFileArr);
			js_show("alert('新增成功！');parent.location.href='/order_index.html'");
			exit();
		}
		// $data["typeList"] = get_one_type_list(0);
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("user/userOrder_add.html",$data);
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
			
			//当状态变为已退款时，修改订单详情中状态为已退款
			if ($postArr["status"] == "6") {
				if (empty($postArr["didArr"])) {
					js_show("alert('请先勾选需要退款的子产品！');");
					exit();
				}
				js_show("alert('退款处理中，请稍后。。。');");
				//调用退款接口
				$didStr = "";
				foreach ($postArr["didArr"] as $did) {
					if(!empty($didStr)) $didStr .= ",";
					$didStr .= $did;
				}
				$url = "https://".MY_VAN_DOMAIN."/api_refundVoucher.html";
				$paramsArr["did"] = $didStr;
				$paramsArr["refundPercent"] = $postArr["refund_percent"];
				$retJson = fgc_post_data($url,$paramsArr);
				// Log::write("tzoo","BK edit order:\n"."params:\n".json_encode($paramsArr)."\n return:\n".$retJson);

				$retList = json_decode($retJson,true);
				if ($retList["status"] == "SUCCESS") {//接口调用成功
					$detailModel = Factory::loadModel("UserOrderDetail");
					foreach ($postArr["didArr"] as $did) {
						$detailModel->updateOne("status=6,refund_percent='{$postArr["refund_percent"]}',refund_reason='{$postArr["refund_reason"]}'","id='$did'");
					}
					js_show("alert('退款成功！');");
				}else{
					js_show("alert('退款失败！失败代码：{$retList["status"]} 失败原因：{$retList["data"]}');");
				}
				// dump($postArr);
			}else{
				$postArr["message"] = $postArr["refund_reason"];//当作备注
				$orderModel = Factory::loadModel("UserOrder");
				$orderModel->updateItem($postArr,"id='{$postArr["eid"]}'");
				js_show("alert('修改成功！');parent.location.href='{$postArr["frontURL"]}'");
			}
			exit();
		}
		$getArr = $this->request->get();
		$itemId = $getArr["eid"];
		//取单条值
		$itemObjR = Factory::loadModel("UserOrder");
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
		return View::parse("user/userOrder_edit.html",$data);
	}
	/**
	 * 接口名称：删除项目
	 * URL：	/order_del.html
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
		$itemObjW = Factory::loadModel("UserOrder",$master);
		$itemObjW->delItem($itemId);
		exit("1");
	}
	 /**
	 * 接口名称：设置排序值
	 * URL：	/order_setRank.html
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
		$itemObjW = Factory::loadModel("UserOrder",$master);
		$itemObjW->updateOne("rank={$postArr["rank"]}","id='$itemId'");
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
	 * 订单导出excel
	 */
    public function exportExcel(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
        //得到参数
		$getArr = $this->request->get();
		// $today = date("Y-m-d");
		// $yesterday = date("Y-m-d",strtotime("-1 day"));
		// if ( empty($getArr["start"]) ) {
		// 	$getArr["start"] = $yesterday." 15:00:00";
		// }
		// if ( empty($getArr["end"]) ) {
		// 	$getArr["end"] = $today." 15:00:00";
		// }
		//取列表
		$whereArr = "status=1 AND is_del=0 AND pay_time>='{$getArr["start"]}' AND pay_time<'{$getArr["end"]}'";
		$itemObjR = Factory::loadModel("UserOrder");
		$itemList = $itemObjR->getItemList($whereArr,"","id ASC");

		$startDay = date("Y-m-d",strtotime($getArr["start"]));
		$endDay = date("Y-m-d",strtotime($getArr["end"]));
		//导出excel
		Header("content=text/html; charset=gb2312");
		Header("Content-type:   application/octet-stream");
		Header("Accept-Ranges:   bytes");
		Header("Content-type:application/vnd.ms-excel");   
		Header("Content-Disposition:attachment;filename="."Travelzoo小程序订单".$startDay."-".$endDay.".xls");   

		$con = "订单号\t商品名\t类别\t单价\t数量\t总价\t微信昵称\t联系人姓名\t电话\t邮箱\t下单时间\t优惠券\t\n";
		if (!empty($itemList)){
			foreach ($itemList as $itemArr){
				// foreach ( $itemArr["detailList"] as $detailArr ) {
					$con .= $itemArr["order_no"]."\t";
					$con .= $itemArr["productArr"]["Title"]."\t";
					$con .= $itemArr["productArr"]["Category"]."\t";
					$con .= $itemArr["product_price"]."\t";
					$con .= $itemArr["num"]."\t";
					$con .= $itemArr["total_price"]."\t";
					$con .= $itemArr["userArr"]["nickname"]."\t";
					$con .= $itemArr["username"]."\t";
					$con .= $itemArr["phone"]."\t";
					$con .= $itemArr["email"]."\t";
					$con .= $itemArr["pay_time"]."\t";
					if ($itemArr["ucid"]) {
						$couponModel = Factory::loadModel("UserCoupon");
						$userCouponArr = $couponModel->getOneItem("id='{$itemArr["ucid"]}'");
						$con .= $userCouponArr["couponArr"]["title"]."\n";
					}else{
						$con .= "无"."\n";
					}
				// }
			}
		}
		echo mb_convert_encoding($con,'gb2312','UTF-8');;
    }
	
	
}
?>