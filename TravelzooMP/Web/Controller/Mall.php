<?php
/**
 * 小程序
 */
class Mall extends Controller
{

	//获取产品测试
	public function getDeals(){
		set_time_limit(0);
		//得到参数
		$getArr = $this->request->get();
		if ($getArr["offset"] >= 0 && !empty($getArr["limit"]) ) {
			$tzoo = new TravelZoo();
			$paramsArr["offset"] = $getArr["offset"];
			$paramsArr["limit"] = $getArr["limit"];
			$retList = $tzoo->getDeals($paramsArr);

			dump($retList);
		}
	}
	//vouchers buy测试
	public function vouchersBuy(){
		set_time_limit(0);

		$tzoo = new TravelZoo();

		$paramsArr["DealId"] = 279008;
		$paramsArr["VoucherQuantity"] = 2;
		for($i=0;$i<$paramsArr["VoucherQuantity"];$i++){
			$recipientArr[] = array("FirstName"=>"b","LastName"=>"a");
		}
		$paramsArr["DealRecipients"] = json_encode($recipientArr);
		$paramsArr["Customer"] = '{"FirstName":"b","LastName":"a","AffiliateCustomerId":"10001"}';
		$paramsArr["SourceIP"] = "127.0.0.1";
		$paramsArr["EmailAddress"] = "99419824@qq.com";
		$retList = $tzoo->buyVouchers($paramsArr);

		dump($retList);
	}
	//get voucher status测试
	public function getVoucherStatus(){
		set_time_limit(0);
		//得到参数
		$getArr = $this->request->get();

		$tzoo = new TravelZoo();
		$paramsArr["transactionId"] = $getArr["tid"];
		$paramsArr["voucherId"] = $getArr["vid"];
		$retList = $tzoo->getVoucherStatus($paramsArr);

		dump($retList);

	}
	//微信支付测试
	public function wxpay(){
		$postArr["order_no"] = "app".date("YmdHis").rand(100,999);
		$postArr["total_price"] = 1013;

		$wxApp = new WxApp();
		$jsApiParameters = $wxApp->unifiedOrder("oTh3r0G_nasbaTSweR6v-AFTEeOM","ttt123",$postArr);
		dump($jsApiParameters);
	}
	//批量生成二维码图片
	public function makeQrcodeImg(){
		/*
		$detailModel = Factory::loadModel("UserOrderDetail");
		$detailList = $detailModel->getItemList("qrcode_name=''","","");
		foreach ($detailList as $key=>$val){
			$fileDir = "./images/upload/qrcode/".date("Ymd")."/";
			$fileName = $val["bar_code"].".png";
			$content = $val["bar_code"]."/".$val["pin"];
			create_QRcode_img($fileDir,$fileName,$content);
			
			$postArr["qrcode_name"] = date("Ymd")."/".$fileName;
			$detailModel->updateItem($postArr,"id='{$val["id"]}'");
		}
		*/
	}
	//邮件测试
	public function sendEmailTest(){
		$title = "Tzoo邮件测试";
		$body = "邮件内容";
		$toEmailArr = "99419824";
		$psObj = new PublicService();
		$retJson = $psObj->sendMail($title,$body,$toEmailArr);
		$retList = json_decode($retJson,true);
		if ($retList["message"]=="success") {
			echo "OK";
		}else{
			echo $retList["errors"][0];
		}
		dump($retList);
	}
	//test
	public function test(){
		// "pages/tuanOrderDetails/index"
		// $gid = "1";
		// $fileDir = "images/upload/miniqrcode/".date("Ymd")."/";
		// $fileName = $gid.".png";
		// $wxApp = new WxApp();
		// $wxApp->createMiniQrcode("gid,".$gid,"",$fileDir,$fileName);

		$tzoo = new TravelZoo();
		$paramsArr["email"] = urlencode("kevin.cao@m-int.cn");
		$paramsArr["adId"] = "293782";
		$retStr = $tzoo->signUp($paramsArr);
		dump($retStr);

		//发送模板消息
		// $templateArr["openid"] = "oTh3r0G_nasbaTSweR6v-AFTEeOM";
		// $templateArr["url"] = "/pages/wode/wode";
		// $templateArr["formId"] = "wx2215384038905804b51c07170027202296";
		// $templateArr["keyword1"] = "test1";
		// $templateArr["keyword2"] = "3333,123";
		// $templateArr["keyword3"] = "2018-11-01";
		// $templateArr["keyword4"] = "minutes1";
		// $templateArr["keyword5"] = "021-33637869";
		// $wxApp = new WxApp();
		// $retStr = $wxApp->sendPayTemplateMsg($templateArr);
		// dump($retStr);

		// echo date("Y-m-d H:i:s",time()-7*24*60*60+8*60*60);

	}




	//记录调用API的日志
	public function addApiLog($type,$oid,$tid,$errCode,$errMsg,$friendlyMsg){
		$buyLogArr["type"] = $type;
		$buyLogArr["oid"] = $oid;
		$buyLogArr["transaction_id"] = $tid;
		$buyLogArr["error_code"] = $errCode;
		$buyLogArr["error_msg"] = $errMsg;
		$buyLogArr["error_friendly_msg"] = $friendlyMsg;
		$buyLogModel = Factory::loadModel("BuyLog");
		$buyLogModel->addItem($buyLogArr);
	}
	//退款成功/失败，发送邮件。参数：type 1 Back office；2微信
	public function sendEmail($orderArr,$refundFee,$type,$isSuccess,$failCode="",$failInfo=""){
		$refundFee = $refundFee/100;
		if ($type=="1") {
			if ($isSuccess) {
				$title = "BackOffice退款申请 审核成功，单号：".$orderArr["order_no"];
				$body = "<br><br>您有一笔微信小程序订单，BO退款通过，即将退款。<br><br>";
			}else{
				$title = "BackOffice退款申请 审核失败，单号：".$orderArr["order_no"];
				$body = "<br><br>您有一笔微信小程序订单，BO退款失败，错误代码：$failCode 原因：$failInfo <br><br>";
			}
		}else{
			if ($isSuccess) {
				$title = "微信小程序 退款成功，单号：".$orderArr["order_no"];
				$body = "<br><br>您有一笔微信小程序订单，已于 ".date("Y年m月d日 H:i:s")." 退款成功，退款金额：".$refundFee." 元<br><br>";
			}else{
				$title = "微信小程序 退款失败，单号：".$orderArr["order_no"];
				$body = "<br><br>您有一笔微信小程序订单，已于 ".date("Y年m月d日 H:i:s")." 退款失败，退款金额：".$refundFee." 元。错误代码：$failCode 原因：$failInfo <br><br>";
			}
		}
		if ($orderArr["ucid"]) {
			$couponModel = Factory::loadModel("UserCoupon");
			$userCouponArr = $couponModel->getOneItem("id='{$orderArr["ucid"]}'");
			$isUseCoupon = "是<br>优惠券类型/金额：".$userCouponArr["couponArr"]["title"];
		}else{
			$isUseCoupon = "否";
		}
		$body .= <<<EOD
订单详情如下：<br><br>

联系人信息：<br>
姓名：{$orderArr["username"]} 电话：{$orderArr["phone"]} 邮箱：{$orderArr["email"]} <br><br>

付款人信息：<br>
付款人UID：{$orderArr["uid"]} 付款人昵称：{$orderArr["userArr"]["nickname"]} 付款时间：{$orderArr["pay_time"]} <br><br>

订单单号：{$orderArr["order_no"]} <br><br>

退款金额：{$refundFee}元 <br><br>

是否有优惠券：{$isUseCoupon} <br><br>
EOD;
		//优惠券类型/金额：
		$toEmailArr = "wzhang@travelzoo.com;lislin@travelzoo.com;ashen@travelzoo.com;fchau@travelzoo.com;jlai@travelzoo.com;hmeng@travelzoo.com;nxun@travelzoo.com;99419824@qq.com";
		$psObj = new PublicService();
		$retJson = $psObj->sendMail($title,$body,$toEmailArr);
		$retList = json_decode($retJson,true);
	}
	//单个更新兑换券状态
	public function updateVoucherStatusOne(){
		set_time_limit(0);
		//得到参数
		$getArr = $this->request->get();

		$tzoo = new TravelZoo();
		$paramsArr["transactionId"] = "";
		$paramsArr["voucherId"] = $getArr["vid"];
		$retList = $tzoo->getVoucherStatus($paramsArr);
		$i=0;
		$detailModel = Factory::loadModel("UserOrderDetail");
		foreach ($retList["VoucherStatuses"] as $vArr) {
			if($vArr["StatusId"]==5 || $vArr["StatusId"]==6){//已使用
				$status = 1;
			}elseif ($vArr["StatusId"]==7) {//已过期
				$status = 2;
			}elseif ($vArr["StatusId"]==8) {//已退款
				$status = 6;
			}
			if($status>0){
				$detailModel->updateOne("status='$status'","voucher_id='{$vArr["VoucherId"]}'");
			}
			$i=$i+1;
		}
		echo "共更新".$i."个优惠券";
		//记录日志
		// $logArr["oid"] = -2;
		// $logArr["error_friendly_msg"] = "共更新".$i."个优惠券";
		// $buyLogModel = Factory::loadModel("BuyLog");
		// $buyLogModel->addItem($logArr);
	}
	//每天中午12点运行一次，把所有拼团成功的拼团状态改为未中奖，发送模板消息，并发送10元优惠券
	public function updateGroupStatusSuccess(){
		set_time_limit(0);

		//优惠券资料
		$couponModel = Factory::loadModel("Coupon");
		$couponArr = $couponModel->getOne("id='3'");
		//取所有拼团成功的订单列表
		$orderModel = Factory::loadModel("UserOrderGroup");
		$orderList = $orderModel->getItemList("group_status=2 AND pay_status=1 AND is_del=0","","id ASC");
		$checkDay = date("Y-m-d H:i:s",time()-7*24*60*60);

		$i=0;
		if (!empty($orderList)){
			foreach ($orderList as $orderArr) {
				$orderModel->updateOne("group_status=3","id='{$orderArr["id"]}'");
				
				//取一条formId发送模板消息用,7天内
				$formWhereArr = "uid='{$orderArr["userArr"]["id"]}' AND status=1 AND ((type=1 AND send_num=0) OR (type=2 AND send_num<3)) AND create_time>'$checkDay'";
				$formModel = Factory::loadModel("UserFormId");
				$formArr = $formModel->getOne($formWhereArr,"id ASC");
				//拼团成功未中奖发模板消息
				$keywordArr["keyword1"] = $orderArr["productArr"]["title"];
				$keywordArr["keyword2"] = date("Y-m-d H:i:s");
				$psObj = new PublicService();
				$psObj->sendAppTemplateMsg("4",$orderArr["userArr"]["openid"],"/pages/tuanList/index",$formArr["form_id"],$keywordArr);

				//发送10元优惠券
				$userCouponModel = Factory::loadModel("UserCoupon");
				$postArr["uid"] = $orderArr["userArr"]["id"];
				$postArr["cid"] = "3";
				$postArr["expiration_time"] = $couponArr["expiration_time"];
				$userCouponModel->addItem($postArr);
				
				$i=$i+1;
			}
		}
		//记录日志
		$friendlyMsg = "共更新".$i."个拼团未中奖订单";
		$this->addApiLog(-3,"","","","",$friendlyMsg);
		echo $friendlyMsg;
	}
	//每天下午18点运行一次，把所有到期产品中拼团未成功的状态改为拼团失败，发送模板消息，并发送5元优惠券，退款
	public function updateGroupStatusFail(){
		set_time_limit(0);

		//优惠券资料
		$couponModel = Factory::loadModel("Coupon");
		$couponArr = $couponModel->getOne("id='4'");
		//取所有今天18点到期的产品
		$toTime = date("Y-m-d")." 18:00:00";
		$pidStr = "";
		$productModel = Factory::loadModel("ProductGroup");
		$productList = $productModel->getList("to_time='$toTime'","","id ASC");
		foreach ($productList as $productArr) {
			if(!empty($pidStr)) $pidStr .= ",";
			$pidStr .= $productArr["id"];
		}
		//取所有到期产品中拼团未成功的订单列表
		$orderModel = Factory::loadModel("UserOrderGroup");
		$orderList = $orderModel->getItemList("pid in($pidStr) AND group_status=0 AND pay_status=1 AND is_del=0","","id ASC");
		$checkDay = date("Y-m-d H:i:s",time()-7*24*60*60);

		$i=0;
		if (!empty($orderList)){
			foreach ($orderList as $orderArr) {
				$orderModel->updateOne("group_status=1","id='{$orderArr["id"]}'");
				
				//取一条formId发送模板消息用,7天内
				$formWhereArr = "uid='{$orderArr["userArr"]["id"]}' AND status=1 AND ((type=1 AND send_num=0) OR (type=2 AND send_num<3)) AND create_time>'$checkDay'";
				$formModel = Factory::loadModel("UserFormId");
				$formArr = $formModel->getOne($formWhereArr,"id ASC");
				//拼团失败发模板消息
				$keywordArr["keyword1"] = $orderArr["productArr"]["title"];
				$keywordArr["keyword2"] = $orderArr["total_price"];
				$keywordArr["keyword4"] = $orderArr["pay_time"];
				$keywordArr["keyword5"] = $orderArr["total_price"];
				$psObj = new PublicService();
				$psObj->sendAppTemplateMsg("3",$orderArr["userArr"]["openid"],"/pages/discount/discount",$formArr["form_id"],$keywordArr);

				//发送5元优惠券
				$userCouponModel = Factory::loadModel("UserCoupon");
				$postArr["uid"] = $orderArr["userArr"]["id"];
				$postArr["cid"] = "4";
				$postArr["expiration_time"] = $couponArr["expiration_time"];
				$userCouponModel->addItem($postArr);
				
				//退款
				$refundFee = $totalFee = $orderArr["total_price"]*100;
				$wxApp = new WxApp();
				$wxRetArr = $wxApp->refundOrder($orderArr["wx_transaction_id"],$refundFee,$totalFee,"1");
				if(array_key_exists("return_code", $wxRetArr) && array_key_exists("result_code", $wxRetArr) && $wxRetArr["return_code"] == "SUCCESS" && $wxRetArr["result_code"] == "SUCCESS"){
					//修改此订单状态为已退款
					$updArr["pay_status"] = 6;
					$updArr["wx_refund_id"] = $wxRetArr["refund_id"];
					$updArr["refund_fee"] = $refundFee/100;
					$updArr["refund_time"] = date("Y-m-d H:i:s");
					$orderModel = Factory::loadModel("UserOrderGroup");
					$orderModel->updateItem($updArr,"order_no='{$orderArr["order_no"]}'");
				}
				//记录日志
				$this->addApiLog(4,$orderArr["id"],$orderArr["wx_transaction_id"],$wxRetArr["return_code"],$wxRetArr["return_msg"],"");

				$i=$i+1;
			}
		}
		//记录日志
		$friendlyMsg = "共更新".$i."个拼团失败订单";
		$this->addApiLog(-4,"","","","",$friendlyMsg);
		echo $friendlyMsg;
	}
	//每天中午12:10分更新一次兑换券使用情况，系统自动运行
	public function updateVoucherStatus(){
		set_time_limit(0);
		
		$detailModel = Factory::loadModel("UserOrderDetail");
		$detailList = $detailModel->getList("status=0","","");

		$i=0;
		if (!empty($detailList)){
			$idStr = "";
			foreach ($detailList as $val){
				if(!empty($idStr)) $idStr .= ",";
				$idStr .= $val["voucher_id"];
			}

			$tzoo = new TravelZoo();
			$paramsArr["transactionId"] = "";
			$paramsArr["voucherId"] = $idStr;
			$retList = $tzoo->getVoucherStatus($paramsArr);
			
			$detailModel = Factory::loadModel("UserOrderDetail");
			foreach ($retList["VoucherStatuses"] as $vArr) {
				$status = 0;
				// if($vArr["StatusId"]==2 || $vArr["StatusId"]==3){//未使用
				// 	$status = 0;
				// }else
				if($vArr["StatusId"]==5 || $vArr["StatusId"]==6){//已使用
					$status = 1;
				}elseif ($vArr["StatusId"]==7) {//已过期
					$status = 2;
				}elseif ($vArr["StatusId"]==8) {//已退款
					$status = 6;
				}
				if($status>0){
					$detailModel->updateOne("status='$status'","voucher_id='{$vArr["VoucherId"]}'");
				}
				$i=$i+1;
			}
		}
		//记录日志
		$friendlyMsg = "共更新".$i."个兑换券";
		$this->addApiLog(-2,"","","","",$friendlyMsg);

	}
	//每天夜里0:10分批量更新产品信息，系统自动运行
	public function updateAllDeals(){
		
		set_time_limit(0);
		//得到参数
		// $getArr = $this->request->get();
		// if ($getArr["offset"] >= 0 && !empty($getArr["limit"]) ) {
		$tzoo = new TravelZoo();

		// $paramsArr["offset"] = $getArr["offset"];
		// $paramsArr["limit"] = $getArr["limit"];
		$paramsArr["offset"] = 0;
		$paramsArr["limit"] = 200;
		$retList = $tzoo->getDeals($paramsArr);

		$i=0;
		if( $retList["Error"]["Code"] == "-1" ){
			foreach ($retList["LocalDeals"] as $retArr) {
				$this->addOneProduct($retArr);
				//判断是否有子套餐
				if(!empty($retArr["ChildDeals"])){
					foreach ($retArr["ChildDeals"] as $childArr) {
						$this->addOneProduct($childArr);
					}
				}
				$i=$i+1;
			}
		}
		//记录日志
		$friendlyMsg = "共更新".$i."个商品";
		$this->addApiLog(-1,"","","","",$friendlyMsg);
		
		// }
		
	}
	//导入一个产品
	private function addOneProduct($retArr){
		$productModel = Factory::loadModel("Product");
		$retArr["Title"] = htmlspecialchars($retArr["Title"],ENT_QUOTES);
		$retArr["Lead"] = htmlspecialchars($retArr["Lead"],ENT_QUOTES);
		$retArr["WhyWeLoveIt"] = htmlspecialchars($retArr["WhyWeLoveIt"],ENT_QUOTES);
		$retArr["WhatIsIncluded"] = htmlspecialchars($retArr["WhatIsIncluded"],ENT_QUOTES);
		$retArr["FinePrint"] = htmlspecialchars($retArr["FinePrint"],ENT_QUOTES);
		$retArr["HowToRedeem"] = htmlspecialchars($retArr["HowToRedeem"],ENT_QUOTES);
		$retArr["RedeemFinePrint"] = htmlspecialchars($retArr["RedeemFinePrint"],ENT_QUOTES);
		$retArr["RedeemTermsAndConditions"] = htmlspecialchars($retArr["RedeemTermsAndConditions"],ENT_QUOTES);
		$retArr["PublishDateUtc"] = date("Y-m-d H:i:s",strtotime($retArr["PublishDateUtc"]));
		$retArr["PurchaseExpiryDateUtc"] = date("Y-m-d H:i:s",strtotime($retArr["PurchaseExpiryDateUtc"]));
		$retArr["VoucherExpiryDateUtc"] = date("Y-m-d H:i:s",strtotime($retArr["VoucherExpiryDateUtc"]));
		$retArr["ChildDeals"] = empty($retArr["ChildDeals"]) ? "0" : "1";
		$productArr = $productModel->getOne("Id='{$retArr["Id"]}'");
		if(empty($productArr)){
			$productModel->addItem($retArr);
		}else{
			if ($productArr["noUpdate"]=="0") {//该字段为0则需要更新，为1不要更新
				$productModel->updateItem($retArr,"Id='{$retArr["Id"]}'");
			}
		}
		
		//商户信息
		$retArr["MerchantDetails"]["Name"] = htmlspecialchars($retArr["MerchantDetails"]["Name"],ENT_QUOTES);
		$retArr["MerchantDetails"]["DisplayName"] = htmlspecialchars($retArr["MerchantDetails"]["DisplayName"],ENT_QUOTES);
		$retArr["MerchantDetails"]["Lat"] = $retArr["PickupPoints"][0]["Lat"];
		$retArr["MerchantDetails"]["Lng"] = $retArr["PickupPoints"][0]["Lng"];
		$retArr["MerchantDetails"]["sid"] = $retArr["MerchantDetails"]["Id"];
		$retArr["MerchantDetails"]["pid"] = $retArr["Id"];
		$storeModel = Factory::loadModel("Store");
		$storeArr = $storeModel->getOne("sid='{$retArr["MerchantDetails"]["Id"]}' AND pid='{$retArr["Id"]}'");
		if(empty($storeArr)){
			$storeModel->addItem($retArr["MerchantDetails"]);
		}else{
			$storeModel->updateItem($retArr["MerchantDetails"],"sid='{$retArr["MerchantDetails"]["Id"]}' AND pid='{$retArr["Id"]}'");
		}
		//产品图片
		$imgModel = Factory::loadModel("ProductImg");
		$imgArr = $imgModel->getOne("pid='{$retArr["Id"]}'");
		if(!empty($imgArr)){
			$imgModel->delList("pid='{$retArr["Id"]}'");
		}
		foreach ($retArr["ImageGallery"] as $galleryArr) {
			$imgInsArr["pid"] = $retArr["Id"];
			$imgInsArr["img_name"] = $galleryArr["Url"];
			$imgInsArr["rank"] = $galleryArr["Order"];
			$imgModel->addItem($imgInsArr);
		}

	}
	//普通链接跳转小程序
	public function jump(){

	}



	
}
?>