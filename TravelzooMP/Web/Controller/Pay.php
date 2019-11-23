<?php
/**
 * 支付类
 */
class Pay extends Controller
{
	//0元购
	public function zero($orderNo){
		require_once 'Libs/wxpay/example/log.php';
		//初始化日志
		$logHandler= new CLogFileHandler("Libs/wxpay/logs/".date('Y-m-d').'.log');
		$log = Log::Init($logHandler, 15);

		$orderModel = Factory::loadModel("UserOrder");
		$orderArr = $orderModel->getOne("order_no='$orderNo'");
	
		$result["result_code"] = "";
		$result["transaction_id"] = "";
		$notify = new PayNotifyCallBack();
		$notify->paySuccess($orderArr,$result);
	}
	//微信支付异步通知
	public function notify(){
		ini_set('date.timezone','Asia/Shanghai');
		// error_reporting(E_ERROR);

		require_once "Libs/wxpay/lib/WxPay.Api.php";
		require_once 'Libs/wxpay/lib/WxPay.Notify.php';
		require_once 'Libs/wxpay/example/log.php';
		//初始化日志
		$logHandler= new CLogFileHandler("Libs/wxpay/logs/".date('Y-m-d').'.log');
		$log = Log::Init($logHandler, 15);
		

		// Log::DEBUG("begin notify");
		$notify = new PayNotifyCallBack();
		$notify->Handle(false);
		
	}

	/*
	 * 红包支付
	 */
	public function hbpay(){
		ini_set('date.timezone','Asia/Shanghai');
		
		$wechatObj = new WeChat();
		$wxJsApiPay = new WxJsApiPay();
	
		$input = new WxPayHongBaoOrder();
		
		$input->SetSend_name("晟毅传媒");//红包发送者名称
		$input->SetRe_openid("oWqMJs8nzeNl2cjqao-HKq151hUE");//接受红包的用户
		$input->SetTotal_amount(1000);//付款金额，单位分
		$input->SetTotal_num(1);//红包发放总人数 
		$input->SetWishing("红包祝福语");//红包祝福语
		$input->SetAct_name("活动名");//活动名称
		$input->SetRemark("备注");//备注信息
		
		$result = WxPayApi::hbOrder($input);
		
		var_dump($result);
	}


	
}


require_once "Libs/wxpay/lib/WxPay.Api.php";
require_once 'Libs/wxpay/lib/WxPay.Notify.php';
require_once 'Libs/wxpay/example/log.php';

class PayNotifyCallBack extends WxPayNotify
{
	//查询订单
	public function Queryorder($transaction_id){
		$input = new WxPayOrderQuery();
		$input->SetTransaction_id($transaction_id);
		$result = WxPayApi::orderQuery($input);
		// Log::DEBUG("get result");

		if(array_key_exists("return_code", $result)	&& array_key_exists("result_code", $result)	&& $result["return_code"] == "SUCCESS" && $result["result_code"] == "SUCCESS"){
			
			$orderNo = $result["out_trade_no"];
			$wxTotalFee = $result["total_fee"];
			$orderModel = Factory::loadModel("UserOrder");
			$orderArr = $orderModel->getOne("order_no='$orderNo'");
			// Log::DEBUG("get orderArr");

			if ( empty($orderArr) ) {
				$orderGroupModel = Factory::loadModel("UserOrderGroup");
				$orderGroupArr = $orderGroupModel->getOne("order_no='$orderNo'");
				if ( $orderGroupArr["pay_status"]=="0" && (intval(strval($orderGroupArr["total_price"]*100))==$wxTotalFee) ) {
					// Log::DEBUG("updateItem start group");
					$this->paySuccessGroup($orderGroupArr,$result);
				}
			}else{
				if ( $orderArr["status"]=="0" && (intval(strval($orderArr["total_price"]*100))==$wxTotalFee) ) {
					// Log::DEBUG("updateItem start");
					$this->paySuccess($orderArr,$result);
				}
			}
			// Log::DEBUG("all is over");
			return true;
		}
		return false;
	}
	
	//重写回调处理函数
	public function NotifyProcess($data, &$msg){
		// Log::DEBUG("call back");
		$notfiyOutput = array();
		
		if(!array_key_exists("transaction_id", $data)){
			$msg = "输入参数不正确";
			return false;
		}
		//查询订单，判断订单真实性
		if(!$this->Queryorder($data["transaction_id"])){
			$msg = "订单查询失败";
			return false;
		}
		return true;
	}
	//拼团支付成功后处理流程
	public function paySuccessGroup($orderArr,$result){
		$userModel = Factory::loadModel("User");
		$userArr = $userModel->getOne("id='{$orderArr["uid"]}'");
		//如果是团长则生成小程序码，不是则用团长的码
		$gid = ($orderArr["gid"]=="0") ? $orderArr["id"] : $orderArr["gid"];
		if ($orderArr["gid"]=="0") {
			$scene = "gid,".$gid;
			$fileDir = "./images/upload/miniqrcode/".date("Ymd")."/";
			$fileName = $gid.".png";
			$wxApp = new WxApp();
			$wxApp->createMiniQrcode($scene,"",$fileDir,$fileName);
			$updArr["qrcode"] = date("Ymd")."/".$fileName;
		}else{
			$orderModel = Factory::loadModel("UserOrderGroup");
			$tuanOrderArr = $orderModel->getOne("id='{$orderArr["gid"]}'");
			$updArr["qrcode"] = $tuanOrderArr["qrcode"];
		}
		//修改订单状态为已付款
		$updArr["pay_status"] = 1;
		$updArr["pay_time"] = date("Y-m-d H:i:s");
		$updArr["wx_result_code"] = $result["result_code"];
		$updArr["wx_transaction_id"] = $result["transaction_id"];
		$orderModel = Factory::loadModel("UserOrderGroup");
		$orderModel->updateItem($updArr,"order_no='{$orderArr["order_no"]}'");
		// Log::DEBUG("updateItem end group");
		//取一条formId发送模板消息用,7天内
		$checkDay = date("Y-m-d H:i:s",time()-7*24*60*60);
		$formWhereArr = "uid='{$userArr["id"]}' AND status=1 AND ((type=1 AND send_num=0) OR (type=2 AND send_num<3)) AND create_time>'$checkDay'";
		$formModel = Factory::loadModel("UserFormId");
		$formArr = $formModel->getOne($formWhereArr,"id ASC");
		//如果此团已满，则要把此团的所有订单的团状态改为2
		$psObj = new PublicService();
		$nowNum = $orderModel->getCount("id='$gid' or gid='$gid' AND pay_status in(1,6) AND group_status=0 AND is_del=0");
		$productModel = Factory::loadModel("ProductGroup");
		$productArr = $productModel->getOne("id='{$orderArr["pid"]}' AND is_del=0");
		if ($nowNum>=$productArr["num"]) {
			$orderModel = Factory::loadModel("UserOrderGroup");
			$orderModel->updateList("group_status=2","id='$gid' or gid='$gid' AND pay_status in(1,6) AND group_status=0 AND is_del=0");
			//拼团成功发模板消息
			$keywordArr["keyword1"] = $productArr["title"];
			$keywordArr["keyword2"] = "￥".$productArr["price"];
			$keywordArr["keyword3"] = $productArr["num"];
			$keywordArr["keyword4"] = $updArr["pay_time"];
			$psObj->sendAppTemplateMsg("2",$userArr["openid"],"/pages/tuanList/index",$formArr["form_id"],$keywordArr);
		}else{
			//拼团待成团通知
			$keywordArr["keyword1"] = $productArr["title"];
			$keywordArr["keyword2"] = "￥".$productArr["price"];
			$keywordArr["keyword3"] = $updArr["pay_time"];
			$keywordArr["keyword4"] = $productArr["to_time"];
			$keywordArr["keyword5"] = $productArr["num"]-$nowNum;
			$psObj->sendAppTemplateMsg("1",$userArr["openid"],"/pages/tuanOrderDetails/index?gid=".$gid,$formArr["form_id"],$keywordArr);
		}
		//调用Tzoo注册接口
		$tzoo = new TravelZoo();
		$paramsArr["email"] = urlencode($orderArr["email"]);
		$paramsArr["adId"] = $productArr["tzoo_pid"];
		$tzooArr = $tzoo->signUp($paramsArr);
		//记录API日志
		$mallController = Factory::loadController("Mall");
		$mallController->addApiLog(3,$orderArr["id"],"",$tzooArr["Status"],$tzooArr["Message"],$tzooArr["PixelUrl"]);

	}
	//支付成功后处理流程
	public function paySuccess($orderArr,$result){
		$userModel = Factory::loadModel("User");
		$userArr = $userModel->getOne("id='{$orderArr["uid"]}'");
		//修改订单状态为已付款
		$insArr["status"] = 1;
		$insArr["pay_time"] = date("Y-m-d H:i:s");
		$insArr["wx_result_code"] = $result["result_code"];
		$insArr["wx_transaction_id"] = $result["transaction_id"];
		$orderModel = Factory::loadModel("UserOrder");
		$orderModel->updateItem($insArr,"order_no='{$orderArr["order_no"]}'");
		//修改优惠券状态为已使用
		if (!empty($orderArr["ucid"])) {
			$couponModel = Factory::loadModel("UserCoupon");
			$couponModel->updateOne("status=1","id='{$orderArr["ucid"]}'");
		}
		// Log::DEBUG("updateItem end");
		//1.调Tzoo支付成功接口获取券号 2.保存券号至订单明细表 3.记录API购买日志 3.将券号发送至用户手机短信
		$voucherArr = $this->getTzooVouchers($orderArr);

		if($voucherArr["Error"]["Code"]=="-1"){
			//保存券号
			$detailInsertArr["oid"] = $orderArr["id"];
			$detailInsertArr["uid"] = $orderArr["uid"];
			$detailInsertArr["pid"] = $orderArr["pid"];
			$detailInsertArr["transaction_id"] = $voucherArr["PaymentTransactionId"];
			$codePinStr = "";

			for($i=0;$i<$orderArr["num"];$i++){
				$detailInsertArr["voucher_id"] = $voucherArr["Vouchers"][$i]["Id"];
				$detailInsertArr["bar_code"] = $voucherArr["Vouchers"][$i]["BarCode"];
				$detailInsertArr["pin"] = $voucherArr["Vouchers"][$i]["PIN"];
				$detailInsertArr["expiration_time"] = date("Y-m-d H:i:s",strtotime($voucherArr["Vouchers"][$i]["ExpirationDateUtc"]));
				//生成二维码图片
				$fileDir = "./images/upload/qrcode/".date("Ymd")."/";
				$fileName = $detailInsertArr["bar_code"].".png";
				$content = $detailInsertArr["bar_code"]."/".$detailInsertArr["pin"];
				create_QRcode_img($fileDir,$fileName,$content);

				$detailInsertArr["qrcode_name"] = date("Ymd")."/".$fileName;
				$detailModel = Factory::loadModel("UserOrderDetail");
				$detailModel->addItem($detailInsertArr);
				//短信内容
				if(!empty($codePinStr)) $codePinStr .= "；";
				$codePinStr .= "号码：".$detailInsertArr["bar_code"]."，Pin码：".$detailInsertArr["pin"];
			}
			//发短信，查询父类产品的店铺名称及电话，子类的店铺和电话有可能为空
			$productModel = Factory::loadModel("Product");
			$productArr = $productModel->getOne("Id='{$orderArr["pid"]}'");
			$parentPid = $productArr["ParentDealId"]=="0" ? $productArr["Id"] : $productArr["ParentDealId"];
			$storeModel = Factory::loadModel("Store");
			$storeArr = $storeModel->getOneItem("pid='$parentPid'");
			$psObj = new PublicService();
			// $text = "【旅游族】感谢您使用Travelzoo旅游族，关注Travelzoo旅游族官方微信，通过官方小程序查看兑换券，兑换方式详见我的订单—使用说明。祝您拥有愉快的体验！";
			$text = "【旅游族】感谢购买！您的兑换券".$codePinStr."（请保密），有效期至：".$detailInsertArr["expiration_time"]."。商家：".$storeArr["DisplayName"]."，联系方式：".$storeArr["Phone"]."。";
			$retJson = $psObj->yunpianSendSms($orderArr["phone"],$text);
			// Log::DEBUG("text:".$text."\n sendsms:".$retJson);
			//发送模板消息
			// $templateArr["openid"] = $userArr["openid"];
			// $templateArr["url"] = "";
			// $templateArr["formId"] = $orderArr["wx_form_id"];
			// $templateArr["keyword1"] = $productArr["Title"];
			// $templateArr["keyword2"] = $codePinStr;
			// $templateArr["keyword3"] = $detailInsertArr["expiration_time"];
			// $templateArr["keyword4"] = $storeArr["DisplayName"];
			// $templateArr["keyword5"] = $storeArr["Phone"];
			// $wxApp = new WxApp();
			// $wxApp->sendPayTemplateMsg($templateArr);
		}
		//记录API购买日志
		$mallController = Factory::loadController("Mall");
		$mallController->addApiLog(1,$orderArr["id"],$voucherArr["PaymentTransactionId"],$voucherArr["Error"]["Code"],$voucherArr["Error"]["Message"],$voucherArr["Error"]["UserFriendlyMessage"]);
		/*
		//调用Tzoo注册接口
		$tzoo = new TravelZoo();
		$paramsArr["email"] = urlencode($orderArr["email"]);
		$paramsArr["adId"] = $orderArr["pid"];
		$tzooArr = $tzoo->signUp($paramsArr);
		//记录API日志
		$mallController->addApiLog(3,$orderArr["id"],"",$tzooArr["Status"],$tzooArr["Message"],$tzooArr["PixelUrl"]);
		*/
	}
	//调Tzoo支付成功接口获取券号
	public function getTzooVouchers($orderArr){
		// Log::write("tzoo","pre API:\n");

		$paramsArr["DealId"] = $orderArr["pid"];
		$paramsArr["VoucherQuantity"] = $orderArr["num"];
		for($i=0;$i<$orderArr["num"];$i++){
			$recipientArr[] = array("FirstName"=>"b","LastName"=>"a");
		}
		$paramsArr["DealRecipients"] = json_encode($recipientArr);
		$nameArr = split_cn_name($orderArr["username"]);
		if(empty($nameArr[1])) $nameArr[1]=$nameArr[0];
		$paramsArr["Customer"] = '{"FirstName":"'.$nameArr[1].'","LastName":"'.$nameArr[0].'","AffiliateCustomerId":"'.$orderArr["uid"].'"}';
		$paramsArr["SourceIP"] = "127.0.0.1";
		$paramsArr["EmailAddress"] = $orderArr["email"];
		$tzoo = new TravelZoo();
		$retList = $tzoo->buyVouchers($paramsArr);

		// Log::write("tzoo","get api return:\n".json_encode($retList));
		return $retList;
	}



}





?>