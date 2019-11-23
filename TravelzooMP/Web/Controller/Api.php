<?php
/**
 * 公共API接口
 */
class Api extends Controller
{
	/**
	 * 接口名称：首页推荐分类列表
	 * URL：	/api_getRecommendList.html
	 * 方式：	POST
	 * 参数：
	 * 返回：200:成功
	 * 		{"status":"200","data":array}
	 */
	public function getRecommendList() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			$whereArr = "status=1 AND is_del=0";
			$recommendModel = Factory::loadModel("Recommend");
			$itemList = $recommendModel->getItemList($whereArr,"","type ASC,rank DESC,id ASC");
			echo json_encode(array("status"=>200,"data"=>$itemList));
		}
	}
	/**
	 * 接口名称：首页获取产品列表
	 * URL：	/api_getProductList.html
	 * 方式：	POST
	 * 参数：p 页码(默认1)
	 * 返回：200:成功
	 * 		{"status":"200","data":array}
	 */
	public function getProductList() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			$perPage = 20;
			$nowPage = (isset($postArr["p"]) && $postArr["p"]>=1) ? $postArr["p"] : 1;
			if($nowPage>10) $nowPage = 10;

			$expiryTime = date("Y-m-d H:i:s",time()+24*60*60);
			$whereArr = "ParentDealId=0 AND ((ChildDeals=0 AND IsPurchasable=1) OR (ChildDeals=1 AND IsPurchasable=0)) AND IsSoldOut=0 AND InternalStatus='Published' AND PurchaseExpiryDateUtc > '$expiryTime'";
			$productModel = Factory::loadModel("Product");
			$productList = $productModel->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,"rank DESC,PublishDateUtc DESC");

			$updateTime = date("Y-m-d");
			echo json_encode(array("status"=>200,"data"=>$productList,"updateTime"=>$updateTime));
		}
	}
	/**
	 * 接口名称：获取分类产品列表
	 * URL：	/api_getProductListByType.html
	 * 方式：	POST
	 * 参数：p 页码(默认1)，rid 类别id（2为首页banner列表 3为首页四小类），tid 分类id
	 * 返回：200:成功
	 *		401:参数不完整
	 * 		{"status":"200","data":array,"title":主标题,"desc":描述}
	 */
	public function getProductListByType() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			$perPage = 40;
			$nowPage = (isset($postArr["p"]) && $postArr["p"]>=1) ? $postArr["p"] : 1;
			if( empty($postArr["rid"]) || empty($postArr["tid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			if ($postArr["rid"]=="2") {
				$whereArr = "banner_id='{$postArr["tid"]}'";
				$orderBy = "banner_rank DESC,PublishDateUtc DESC";
			}else{
				$whereArr = "type='{$postArr["tid"]}'";
				$orderBy = "rank DESC,PublishDateUtc DESC";
			}
			$expiryTime = date("Y-m-d H:i:s",time()+24*60*60);
			$whereArr .= " AND ParentDealId=0 AND ((ChildDeals=0 AND IsPurchasable=1) OR (ChildDeals=1 AND IsPurchasable=0)) AND IsSoldOut=0 AND InternalStatus='Published' AND PurchaseExpiryDateUtc > '$expiryTime'";
			$productModel = Factory::loadModel("Product");
			$productList = $productModel->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,$orderBy);

			$recommendModel = Factory::loadModel("Recommend");
			$recoArr = $recommendModel->getOne("id='{$postArr["tid"]}'");
			echo json_encode(array("status"=>200,"data"=>$productList,"title"=>$recoArr["title"],"desc"=>$recoArr["desc"]));
		}
	}
	/**
	 * 接口名称：获取某一产品详情
	 * URL：	/api_getProductDetail.html
	 * 方式：	POST
	 * 参数：pid 产品id
	 * 返回：200:成功
	 *		401:参数不完整
	 *      {"status":"200","data":array}
	 */
	public function getProductDetail(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["pid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			
			$productModel = Factory::loadModel("Product");
			$productArr = $productModel->getOneItem("Id='{$postArr["pid"]}'");

			echo json_encode(array("status"=>200,"data"=>$productArr));
		}
	}
	/**
	 * 接口名称：详情页猜你喜欢，系统推荐产品2个
	 * URL：	/api_getLikeProductList.html
	 * 方式：	POST
	 * 参数：pid 产品id
	 * 返回：200:成功
	 *		401:参数不完整
	 * 		{"status":"200","data":array}
	 */
	public function getLikeProductList() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["pid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			
			$productModel = Factory::loadModel("Product");
			$productArr = $productModel->getOne("Id='{$postArr["pid"]}'");
			
			$expiryTime = date("Y-m-d H:i:s",time()+24*60*60);
			$whereArr = "Category='{$productArr["Category"]}' AND Id<>'{$postArr["pid"]}' AND ParentDealId=0 AND ((ChildDeals=0 AND IsPurchasable=1) OR (ChildDeals=1 AND IsPurchasable=0)) AND IsSoldOut=0 AND InternalStatus='Published' AND PurchaseExpiryDateUtc > '$expiryTime'";
			$productList = $productModel->getItemList($whereArr,"0,2","rank DESC,PublishDateUtc DESC");

			echo json_encode(array("status"=>200,"data"=>$productList));
		}
	}
	/**
	 * 接口名称：用户登录
	 * URL：	/api_userLogin.html
	 * 方式：	POST
	 * 参数：code 用户登录凭证
	 * 返回：200:成功
	 * 		{"status":"200","data":array}
	 */
	public function userLogin() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			$wxApp = new WxApp();
			$openid = $wxApp->getOpenidByCode($postArr["code"]);
			if( empty($openid) ){
				echo json_encode(array("status"=>401,"data"=>"请重新登录"));
				exit();
			}
			echo json_encode(array("status"=>200,"data"=>$openid));
		}
	}
	/**
	 * 接口名称：保存用户资料
	 * URL：	/api_saveUserInfo.html
	 * 方式：	POST
	 * 参数：openid ，nickname 昵称，sex 性别，country 国家，province 省，city 市，headimgurl 头像，encryptedData 完整用户加密数据
	 *		iv 加密算法的初始向量
	 * 返回：200:成功
	 * 		{"status":"200","data":"OK"}
	 */
	public function saveUserInfo() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOne("openid='{$postArr["openid"]}'");
			//获取用户unionid
			$wxApp = new WxApp();
			$retArr = $wxApp->decodeUserInfo($userArr["session_key"],$postArr["encryptedData"],$postArr["iv"]);
			if (is_array($retArr)) {
				$postArr["unionid"] = $retArr["unionId"];
			}
			$userModel->updateItem($postArr,"openid='{$postArr["openid"]}'");
			
			echo json_encode(array("status"=>200,"data"=>"OK"));
		}
	}
	/**
	 * 接口名称：下订单
	 * URL：	/api_addOrder.html
	 * 方式：	POST
	 * 参数：openid ，pid 商品id/套餐id，num 数量，username 联系人，phone 电话，email 邮箱，ucid 用户优惠券id（可选）
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:无此产品，请重新下单
	 *		403:此产品每人限购一份
	 *		404:此产品每人限购一份，您已购买过！
	 *		405:请关闭小程序重试一次
	 *		406:请重新选择您的优惠券！
	 * 		{"status":"200","data":array}
	 */
	public function addOrder() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["pid"]) || empty($postArr["num"]) || empty($postArr["username"]) || empty($postArr["phone"]) || empty($postArr["email"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//产品详情
			$productModel = Factory::loadModel("Product");
			$productArr = $productModel->getOne("Id='{$postArr["pid"]}'");
			if( empty($productArr) ){
				echo json_encode(array("status"=>402,"data"=>"无此产品，请重新下单"));
				exit();
			}
			//0元购的产品，每人限购一份
			$totalPrice = $productArr["Price"]*$postArr["num"];
			if( $totalPrice==0 && $postArr["num"]>1 ){
				echo json_encode(array("status"=>403,"data"=>"此产品每人限购一份"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//0元购的产品，一个人对一个产品只能买一次
			$orderModel = Factory::loadModel("UserOrder");
			$speOrderArr = $orderModel->getOne("uid='{$userArr["id"]}' AND pid='{$postArr["pid"]}'");
			if( $totalPrice==0 && !empty($speOrderArr) ){
				echo json_encode(array("status"=>404,"data"=>"此产品每人限购一份，您已购买过！"));
				exit();
			}
			//优惠券使用判断
			if (!empty($postArr["ucid"])) {
				$couponModel = Factory::loadModel("UserCoupon");
				$userCouponArr = $couponModel->getOneItem("id='{$postArr["ucid"]}' AND uid='{$userArr["id"]}'");
				if ( empty($userCouponArr) || $userCouponArr["status"] != 0 ) {
					echo json_encode(array("status"=>406,"data"=>"请重新选择您的优惠券！"));
					exit();
				}
				$totalPrice = round($totalPrice-$userCouponArr["couponArr"]["price"],2);
				if($totalPrice<0) $totalPrice=0;
			}
			//写订单表
			// if( (MY_APP_DOMAIN=="devtravelzoo.m-int.cn") && in_array($userArr["id"],array("1000","1001","1002","1003","1004","1005","1007")) && $totalPrice>0 ){
			// 	$totalPrice = 0.01*$postArr["num"];
			// }
			$postArr["uid"] = $userArr["id"];
			$postArr["order_no"] = "app".date("YmdHis").rand(100,999);
			$postArr["product_price"] = $productArr["Price"];
			$postArr["total_price"] = $totalPrice;
			// $postArr["message"] = htmlspecialchars($postArr["message"],ENT_QUOTES);
			$orderModel->addItem($postArr);

			if ($totalPrice==0) {
				$payController = Factory::loadController("Pay");
				$payController->zero($postArr["order_no"]);

				$dataArr = "OK";
			}else{
				//微信统一下单
				$body = "Travelzoo旅游族-旅游产品";
				$wxApp = new WxApp();
				$jsApiParameters = $wxApp->unifiedOrder($postArr['openid'],$body,$postArr);

				$dataArr = json_decode($jsApiParameters,true);
				//保存prepay_id
				$prepayId = str_replace("prepay_id=","",$dataArr["package"]);
				// $prepayId = $dataArr["package"];
				$orderModel = Factory::loadModel("UserOrder");
				$orderModel->updateOne("wx_form_id='$prepayId'","order_no='{$postArr["order_no"]}'");
			}
			echo json_encode(array("status"=>200,"data"=>$dataArr));
			exit();
		}
	}
	/**
	 * 接口名称：我的订单列表
	 * URL：	/api_getMyOrderList.html
	 * 方式：	POST
	 * 参数：openid ，p 页码(默认1)
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 * 		{"status":"200","data":array}
	 */
	public function getMyOrderList() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}

			$perPage = 10;
			$nowPage = (isset($postArr["p"]) && $postArr["p"]>=1) ? $postArr["p"] : 1;		

			$detailModel = Factory::loadModel("UserOrderDetail");
			$orderList = $detailModel->getItemList("uid='{$userArr["id"]}' AND status<3",($nowPage-1)*$perPage.",".$perPage,"id DESC");

			echo json_encode(array("status"=>200,"data"=>$orderList));
		}
	}
	/**
	 * 接口名称：查看兑换券详情
	 * URL：	/api_getVoucherDetail.html
	 * 方式：	POST
	 * 参数：openid ，did 订单详情id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:参数不合法
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array}
	 */
	public function getVoucherDetail(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["did"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//判断是否是我的订单
			$detailModel = Factory::loadModel("UserOrderDetail");
			$detailArr = $detailModel->getOneItem("id='{$postArr["did"]}' AND uid='{$userArr["id"]}'");
			if( empty($detailArr) ){
				echo json_encode(array("status"=>402,"data"=>"参数不合法"));
				exit();
			}

			echo json_encode(array("status"=>200,"data"=>$detailArr));
		}
	}
	/**
	 * 接口名称：重新支付（暂未用）
	 * URL：	/api_rePay.html
	 * 方式：	POST
	 * 参数：openid ，oid 订单id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:参数不合法
	 *		405:请关闭小程序重试一次
	 * 		{"status":"200","data":array}
	 */
	public function rePay() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["oid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//判断是否是我的订单
			$orderModel = Factory::loadModel("UserOrder");
			$orderArr = $orderModel->getOneItem("id='{$postArr["oid"]}' AND uid='{$userArr["id"]}'");
			if( empty($orderArr) ){
				echo json_encode(array("status"=>402,"data"=>"参数不合法"));
				exit();
			}
			//产品详情
			$productModel = Factory::loadModel("Product");
			$productArr = $productModel->getOne("Id='{$orderArr["pid"]}'");
			
			//微信统一下单
			$wxApp = new WxApp();
			$jsApiParameters = $wxApp->unifiedOrder($postArr['openid'],$productArr["Title"],$orderArr);

			echo $jsApiParameters;
		}
	}
	/**
	 * 接口名称：获取我最近一次订单中的资料（下订单时读取）
	 * URL：	/api_getMyOneOrder.html
	 * 方式：	POST
	 * 参数：openid
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array}
	 */
	public function getMyOneOrder(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}

			//我的最近一次订单
			$orderModel = Factory::loadModel("UserOrder");
			$orderArr = $orderModel->getOne("uid='{$userArr["id"]}'","id DESC");

			echo json_encode(array("status"=>200,"data"=>$orderArr));
		}
	}
	/**
	 * 接口名称：更新兑换券状态
	 * URL：	/api_setVoucherStatus.html
	 * 方式：	POST
	 * 参数：openid ，did 订单详情id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":"OK"}
	 */
	public function setVoucherStatus(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["did"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//更新兑换券状态为已使用
			$detailModel = Factory::loadModel("UserOrderDetail");
			$detailArr = $detailModel->getOne("id='{$postArr["did"]}' AND uid='{$userArr["id"]}'");
			if ($detailArr["status"]==0) {
				$detailModel->updateOne("status=1","id='{$postArr["did"]}' AND uid='{$userArr["id"]}'");
			}

			echo json_encode(array("status"=>200,"data"=>"OK"));
		}
	}
	/**
	 * 接口名称：后台一键退款
	 * URL：	/api_refundVoucher.html
	 * 方式：	POST
	 * 参数：did 订单详情id（多个用,隔开） refundPercent 退款百分比(默认100)
	 * 返回：200:成功
	 * 		{"status":"200","data":array}
	 */
	public function refundVoucher() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if(empty($postArr["refundPercent"])) $postArr["refundPercent"]=100;
			//查询voucher id
			$didStr = $postArr["did"];
			$detailModel = Factory::loadModel("UserOrderDetail");
			$detailList = $detailModel->getList("id in($didStr)","","");
			if( empty($detailList) ){
				echo json_encode(array("status"=>401,"data"=>"无此订单，请重试"));
				exit();
			}
			$voucherIdStr = "";
			foreach ($detailList as $detailArr) {
				if(!empty($voucherIdStr)) $voucherIdStr .= ",";
				$voucherIdStr .= $detailArr["voucher_id"];
			}
			//订单信息
			$oid = $detailList[0]["oid"];
			$orderModel = Factory::loadModel("UserOrder");
			$orderArr = $orderModel->getOneItem("id='$oid'");
			//调Tzoo的退款接口
			$tzooArr["transactionId"] = $detailList[0]["transaction_id"];
			$tzooArr["voucherId"] = $voucherIdStr;
			$tzoo = new TravelZoo();
			$tzRetArr = $tzoo->refundVouchers($tzooArr);
			// $tzRetArr["IsRefundSuccess"] = true;

			$num = count($detailList);//需要退款的商品数量
			$totalFee = $orderArr["total_price"]*100;//单位分
			$refundFee = round(($orderArr["product_price"]*100*$num)*($postArr["refundPercent"]/100));//单位分
			//优惠券使用判断
			if (!empty($orderArr["ucid"])) {
				$couponModel = Factory::loadModel("UserCoupon");
				$userCouponArr = $couponModel->getOneItem("id='{$orderArr["ucid"]}'");
				$refundFee = $refundFee-$userCouponArr["couponArr"]["price"]*100;
			}

			if ($tzRetArr["IsRefundSuccess"]) {
				//调Tzoo的退款接口成功，发邮件通知
				$mallController = Factory::loadController("Mall");
				$mallController->sendEmail($orderArr,$refundFee,"1",true);
				//记录日志
				$mallController->addApiLog(2,$oid,$detailList[0]["transaction_id"],$tzRetArr["Error"]["Code"],$tzRetArr["Error"]["Message"],$tzRetArr["Error"]["UserFriendlyMessage"]);

				//微信退款接口
				$wxApp = new WxApp();
				$wxRetArr = $wxApp->refundOrder($orderArr["wx_transaction_id"],$refundFee,$totalFee);

				$jsonStatus = $wxRetArr["return_code"];
				$jsonData = $wxRetArr["return_msg"];
				
				if(array_key_exists("return_code", $wxRetArr) && array_key_exists("result_code", $wxRetArr) && $wxRetArr["return_code"] == "SUCCESS" && $wxRetArr["result_code"] == "SUCCESS"){
					//发邮件
					$mallController->sendEmail($orderArr,$refundFee,"2",true);
					//修改此订单状态为已退款
					$insArr["status"] = 6;
					$insArr["wx_refund_id"] = $wxRetArr["refund_id"];
					$insArr["refund_fee"] = $refundFee/100;
					$insArr["refund_time"] = date("Y-m-d H:i:s");
					$orderModel = Factory::loadModel("UserOrder");
					$orderModel->updateItem($insArr,"order_no='{$orderArr["order_no"]}'");
					//退回已使用的优惠券
					if ($orderArr["ucid"]) {
						$couponModel = Factory::loadModel("UserCoupon");
						$couponModel->updateOne("status=0","id='{$orderArr["ucid"]}'");
					}
				}else{
					$mallController->sendEmail($orderArr,$refundFee,"2",false,$jsonStatus,$jsonData);
				}
				//记录日志
				$mallController->addApiLog(2,$oid,$detailList[0]["transaction_id"],$jsonStatus,$jsonData,"");

			}else{
				// 调Tzoo的退款接口失败，发邮件通知
				$jsonStatus = $tzRetArr["Error"]["Code"];
				$jsonData = $tzRetArr["Error"]["Message"];
				$friendlyMsg = $tzRetArr["Error"]["UserFriendlyMessage"];

				$mallController = Factory::loadController("Mall");
				$mallController->sendEmail($orderArr,$refundFee,"1",false,$jsonStatus,$jsonData);
				//记录日志
				$mallController->addApiLog(2,$oid,$detailList[0]["transaction_id"],$jsonStatus,$jsonData,$friendlyMsg);
			}

			echo json_encode(array("status"=>$jsonStatus,"data"=>$jsonData));
		}
	}
	/**
	 * 接口名称：判断是否购买过商品，是否领取过优惠券
	 * URL：	/api_getMyOneSuccessOrder.html
	 * 方式：	POST
	 * 参数：openid
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array,"couponArr":array}
	 */
	public function getMyOneSuccessOrder(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}

			//我的最近一次成功的订单
			$orderModel = Factory::loadModel("UserOrder");
			$orderArr = $orderModel->getOne("uid='{$userArr["id"]}' AND status in(1,6)");

			//是否领取过50/68的优惠券
			$couponModel = Factory::loadModel("UserCoupon");
			$couponArr = $couponModel->getOne("uid='{$userArr["id"]}' AND cid in(1,2)");
			if(MY_APP_DOMAIN=="travelzoo.m-int.cn") {
				$couponArr = "1";
			}
			echo json_encode(array("status"=>200,"data"=>$orderArr,"couponArr"=>$couponArr));
		}
	}
	/**
	 * 接口名称：邻取优惠券
	 * URL：	/api_addCoupon.html
	 * 方式：	POST
	 * 参数：openid，cid 优惠券id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:您已领取过此优惠券，不用重复领取哦！
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array}
	 */
	public function addCoupon(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["cid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//优惠券资料
			$couponModel = Factory::loadModel("Coupon");
			$couponArr = $couponModel->getOne("id='{$postArr["cid"]}'");
			//领取优惠券
			$userCouponModel = Factory::loadModel("UserCoupon");
			$userCouponArr = $userCouponModel->getOne("uid='{$userArr["id"]}' AND cid='{$postArr["cid"]}'");
			if (!empty($userCouponArr)) {
				echo json_encode(array("status"=>402,"data"=>"您已领取过此优惠券，不用重复领取哦！"));
				exit();
			}else{
				$postArr["uid"] = $userArr["id"];
				if ($couponArr["expiration_time"] == "0000-00-00 00:00:00") {//没有过期时间，默认领取后的一个月后过期
					$postArr["expiration_time"] = date("Y-m-d H:i:s",strtotime("+1 month"));
				}else{
					$postArr["expiration_time"] = $couponArr["expiration_time"];
				}
				$userCouponModel->addItem($postArr);
			}
			echo json_encode(array("status"=>200,"data"=>"领取成功！"));
		}
	}
	/**
	 * 接口名称：我的优惠券列表
	 * URL：	/api_getMyCouponList.html
	 * 方式：	POST
	 * 参数：openid ，p 页码(默认1)，status 状态(-1所有 0未使用 1已使用 2已过期 )
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 * 		{"status":"200","data":array}
	 */
	public function getMyCouponList() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || !isset($postArr["status"]) || $postArr["status"]==="" ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}

			$perPage = 20;
			$nowPage = (isset($postArr["p"]) && $postArr["p"]>=1) ? $postArr["p"] : 1;		

			$whereArr = "uid='{$userArr["id"]}'";
			if ($postArr["status"]>=0) {
				$whereArr .= " AND status='{$postArr["status"]}'";
			}
			$couponModel = Factory::loadModel("UserCoupon");
			$couponList = $couponModel->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,"id DESC");

			echo json_encode(array("status"=>200,"data"=>$couponList));
		}
	}




	/**
	 * 接口名称：获取参与团购的中奖名单
	 * URL：	/api_getGroupWinList.html
	 * 方式：	POST
	 * 参数：
	 * 返回：200:成功
	 *      {"status":"200","data":array}
	 */
	public function getGroupWinList(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			
			//中奖用户列表
			$noticeModel = Factory::loadModel("Notice");
			$noticeList = $noticeModel->getList("status=1 AND is_del=0","0,10","rank DESC,id DESC");
			// $orderModel = Factory::loadModel("UserOrderGroup");
			// $userList = $orderModel->getList("group_status in(4,5) AND is_del=0","0,10","gid ASC,id ASC");
			// $userModel = Factory::loadModel("User");
			// foreach ($userList as $key=>$value) {
			// 	$userArr = $userModel->getOne("id='{$value["uid"]}'");
			// 	$userArr["smartPhone"] = substr_replace($value["phone"],"****",3,4);
			// 	$userList[$key]["userArr"]  = $userArr;
			// }

			echo json_encode(array("status"=>200,"data"=>$noticeList));
		}
	}
	/**
	 * 接口名称：团购产品列表
	 * URL：	/api_getGroupProductList.html
	 * 方式：	POST
	 * 参数：p 页码(默认1)，gt 团购时间（1今日 2明日）
	 * 返回：200:成功
	 * 		{"status":"200","data":array}
	 */
	public function getGroupProductList() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			$perPage = 20;
			$nowPage = (isset($postArr["p"]) && $postArr["p"]>=1) ? $postArr["p"] : 1;
			if($nowPage>10) $nowPage = 10;

			$gt = (isset($postArr["gt"]) && $postArr["gt"]>=1) ? $postArr["gt"] : 1;
			$nowTime = date("Y-m-d H:i:s");
			$tomorrow = date("Y-m-d",strtotime("+1 day"));

			$whereArr = "is_del=0 AND status=1";
			if($gt=="1"){
				$whereArr .= " AND from_time<='$nowTime' AND to_time>'$nowTime'";
			}else{
				$whereArr .= " AND date(from_time)='$tomorrow'";
			}
			$productModel = Factory::loadModel("ProductGroup");
			$productList = $productModel->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,"rank DESC,id DESC");

			echo json_encode(array("status"=>200,"data"=>$productList));
		}
	}
	/**
	 * 接口名称：获取团购产品详情
	 * URL：	/api_getGroupProductDetail.html
	 * 方式：	POST
	 * 参数：openid，pid 产品id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array,"gid":拼团id}
	 */
	public function getGroupProductDetail(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["pid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//查看产品详情
			$productModel = Factory::loadModel("ProductGroup");
			$productArr = $productModel->getOneItem("id='{$postArr["pid"]}' AND is_del=0");

			//判断我有没有参与此产品的拼团
			$orderModel = Factory::loadModel("UserOrderGroup");
			$orderArr = $orderModel->getOne("uid='{$userArr["id"]}' AND pid='{$postArr["pid"]}' AND pay_status in(1,6) AND is_del=0");
			$gid=0;
			if( !empty($orderArr) ){
				$gid = $orderArr["gid"]=="0" ?  $orderArr["id"] : $orderArr["gid"];
			}
			echo json_encode(array("status"=>200,"data"=>$productArr,"gid"=>$gid));
		}
	}
	/**
	 * 接口名称：我的团购订单列表
	 * URL：	/api_getMyGroupOrderList.html
	 * 方式：	POST
	 * 参数：openid ，p 页码(默认1)
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 * 		{"status":"200","data":array}
	 */
	public function getMyGroupOrderList() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}

			$perPage = 10;
			$nowPage = (isset($postArr["p"]) && $postArr["p"]>=1) ? $postArr["p"] : 1;		

			$orderModel = Factory::loadModel("UserOrderGroup");
			$orderList = $orderModel->getItemList("uid='{$userArr["id"]}' AND pay_status in(1,6) AND is_del=0",($nowPage-1)*$perPage.",".$perPage,"id DESC");

			echo json_encode(array("status"=>200,"data"=>$orderList));
		}
	}
	/**
	 * 接口名称：获取我的团购订单详情
	 * URL：	/api_getMyGroupOrderDetail.html
	 * 方式：	POST
	 * 参数：openid ，oid 订单id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:参数不合法
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array,"userList":array,"inGroup":0或1,"gid":拼团id,"winStatus":中奖情况,
	 *			"inGroupProduct":0或1,"myGid":我的拼团id(如果参与过此产品的拼团)}
	 */
	public function getMyGroupOrderDetail(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["oid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$myArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($myArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//判断订单是否存在
			$orderModel = Factory::loadModel("UserOrderGroup");
			$orderArr = $orderModel->getOneItem("id='{$postArr["oid"]}' AND pay_status in(1,6) AND is_del=0");
			if( empty($orderArr) ){
				echo json_encode(array("status"=>402,"data"=>"参数不合法"));
				exit();
			}
			//判断我是否参与此产品的拼团
			$checkArr = $orderModel->getOne("uid='{$myArr["id"]}' AND pid='{$orderArr["pid"]}' AND pay_status in(1,6) AND is_del=0");
			if (!empty($checkArr)) {
				$inGroupProduct = 1;
				$myGid = ($checkArr["gid"]=="0") ? $checkArr["id"] : $checkArr["gid"];//我的拼团id
			}else{
				$inGroupProduct = 0;
				$myGid = 0;
			}
			
			//拼团用户列表
			$gid = ($orderArr["gid"]=="0") ? $orderArr["id"] : $orderArr["gid"];//拼团id
			$inGroup = 0;//判断我是否在此拼团列表里面
			$winStatus = "";//中奖情况
			$userList = $orderModel->getList("id='$gid' or gid='$gid' AND pay_status in(1,6) AND is_del=0","","gid ASC,id ASC");
			$userModel = Factory::loadModel("User");
			foreach ($userList as $key=>$value) {
				$userList[$key]["userArr"] = $userModel->getOneItem("id='{$value["uid"]}'");
				if($value["uid"] == $myArr["id"]){
					$inGroup = 1;
					$winStatus = $value["group_status"];
				}
			}
			echo json_encode(array("status"=>200,"data"=>$orderArr,"userList"=>$userList,"inGroup"=>$inGroup,"gid"=>$gid,"winStatus"=>$winStatus,"inGroupProduct"=>$inGroupProduct,"myGid"=>$myGid));
		}
	}
	/**
	 * 接口名称：获取我最近一次团购订单中的资料（下订单时读取）
	 * URL：	/api_getMyOneGroupOrder.html
	 * 方式：	POST
	 * 参数：openid
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array}
	 */
	public function getMyOneGroupOrder(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}

			//我的最近一次团购订单
			$orderModel = Factory::loadModel("UserOrderGroup");
			$orderArr = $orderModel->getOne("uid='{$userArr["id"]}' AND is_del=0","id DESC");

			echo json_encode(array("status"=>200,"data"=>$orderArr));
		}
	}
	/**
	 * 接口名称：下团购订单
	 * URL：	/api_addGroupOrder.html
	 * 方式：	POST
	 * 参数：openid ，gid 拼团id(如果是发起者值为0)，pid 产品id，username 联系人，phone 电话，email 邮箱，formId 表单id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:无此产品，请重新下单
	 *		403:参数不合法
	 *		404:您已参与此产品的拼团，不用重复参与哦
	 *		405:请关闭小程序重试一次
	 * 		{"status":"200","data":array,"oid":订单id}
	 */
	public function addGroupOrder() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["pid"]) || empty($postArr["username"]) || empty($postArr["phone"]) || empty($postArr["email"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//产品详情
			$productModel = Factory::loadModel("ProductGroup");
			$productArr = $productModel->getOne("id='{$postArr["pid"]}' AND status=1 AND is_del=0");
			if( empty($productArr) ){
				echo json_encode(array("status"=>402,"data"=>"无此产品，请重新下单"));
				exit();
			}
			//判断gid是否合法
			if ( !empty($postArr["gid"]) ) {
				$orderModel = Factory::loadModel("UserOrderGroup");
				$orderArr = $orderModel->getOne("id='{$postArr["gid"]}' AND pay_status in(1,6) AND is_del=0");
				if( $orderArr["gid"] != "0" ){
					echo json_encode(array("status"=>403,"data"=>"参数不合法"));
					exit();
				}
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}
			//判断我是否参与此产品的拼团
			$orderModel = Factory::loadModel("UserOrderGroup");
			$checkArr = $orderModel->getOne("uid='{$userArr["id"]}' AND pid='{$postArr["pid"]}' AND pay_status in(1,6) AND is_del=0");
			if( !empty($checkArr) ){
				echo json_encode(array("status"=>404,"data"=>"您已参与此产品的拼团，不用重复参与哦"));
				exit();
			}
			//写订单表
			$postArr["gid"] = empty($postArr["gid"]) ? 0 : $postArr["gid"];
			$postArr["uid"] = $userArr["id"];
			$postArr["order_no"] = "app".date("YmdHis").rand(100,999);
			$postArr["total_price"] = $productArr["price"];
			$postArr["username"] = htmlspecialchars(trim($postArr["username"]),ENT_QUOTES);
			$postArr["phone"] = htmlspecialchars(trim($postArr["phone"]),ENT_QUOTES);
			$postArr["email"] = htmlspecialchars(trim($postArr["email"]),ENT_QUOTES);
			$oid = $orderModel->addItem($postArr);

			//微信统一下单
			$body = "Travelzoo旅游族-拼团产品";
			$wxApp = new WxApp();
			$jsApiParameters = $wxApp->unifiedOrder($postArr['openid'],$body,$postArr);

			$dataArr = json_decode($jsApiParameters,true);

			//保存form_id
			$formModel = Factory::loadModel("UserFormId");
			$insArr["uid"] = $userArr["id"];
			$insArr["type"] = 1;
			$insArr["form_id"] = $postArr["formId"];
			$formModel->addItem($insArr);
			//保存prepay_id
			$insArr["type"] = 2;
			$insArr["form_id"] = str_replace("prepay_id=","",$dataArr["package"]);
			$formModel->addItem($insArr);

			// $orderModel->updateOne("wx_form_id='{$postArr["formId"]}'","order_no='{$postArr["order_no"]}'");
			echo json_encode(array("status"=>200,"data"=>$dataArr,"oid"=>$oid));
			exit();
		}
	}
	/**
	 * 接口名称：填写反馈信息
	 * URL：	/api_addFeedback.html
	 * 方式：	POST
	 * 参数：openid ，content 反馈内容
	 * 返回：200:成功
	 *		401:参数不完整
	 *		405:请关闭小程序重试一次
	 * 		{"status":"200","data":"OK"}
	 */
	public function addFeedback() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["openid"]) || empty($postArr["content"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			//我的信息
			// $userArr = $_SESSION["userArr"];
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("openid='{$postArr["openid"]}'");
			if( empty($userArr) ){
				echo json_encode(array("status"=>405,"data"=>"请关闭小程序重试一次"));
				exit();
			}

			$postArr["uid"] = $userArr["id"];
			$feedbackModel = Factory::loadModel("UserFeedback");
			$feedbackModel->addItem($postArr);

			echo json_encode(array("status"=>200,"data"=>"OK"));
		}
	}
	/**
	 * 接口名称：发送模板消息--后台调用
	 * URL：	/api_sendTemplateMsg.html
	 * 方式：	POST
	 * 参数：type 模板类别，openid，url 打开地址，formId，keywordArr json串
	 * 返回：200:成功
	 *		401:参数不完整
	 * 		{"status":"200","data":"OK"}
	 */
	public function sendTemplateMsg() {
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			if( empty($postArr["type"]) || empty($postArr["openid"]) || empty($postArr["url"]) || empty($postArr["formId"]) || empty($postArr["keywordArr"]) ){
				echo json_encode(array("status"=>401,"data"=>"参数不完整"));
				exit();
			}
			$keywordArr = json_decode(urldecode($postArr["keywordArr"]),true);
			$psObj = new PublicService();
			$psObj->sendAppTemplateMsg($postArr["type"],$postArr["openid"],$postArr["url"],$postArr["formId"],$keywordArr);

			echo json_encode(array("status"=>200,"data"=>"OK"));
		}
	}


	
	
}
?>