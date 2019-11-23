<?php
/**
 *
 *        
 */
class WxApp {
	private $app_id;
	private $app_secret;
	private $mem_suffix;

	static public function getInstence($params = array()) {
		return new WxApp ( $params );
	}
	
	/**
	 * 构造方法
	 * 定义app_id和app_secret
	 */
	public function __construct() {
		$this->app_id = WXAPP_APP_ID;
		$this->app_secret = WXAPP_APP_SECRET;
		$this->mem_suffix = WXAPP_MEM_SUFFIX;
	}
	/**
	 * 获取access_token
	 * access_token缓存有效期7100秒
	 */
	private function getAccessToken() {
		$mem = new Memcached ();
		$mem->addServer ( 'localhost', 11211 );
		$accessToken = $mem->get ( 'access_token_'.$this->mem_suffix );
		if (empty ( $accessToken )) {
			$url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=".$this->app_id."&secret=".$this->app_secret;
			$data = json_decode ( file_get_contents($url), true );
			$accessToken = $data["access_token"];
			$mem->set ('access_token_'.$this->mem_suffix , $accessToken, 7100 );
		}
		return $accessToken;
	}
	//用code换取用户openid和session_key
	public function getOpenidByCode($code){
		$apiUrl = "https://api.weixin.qq.com/sns/jscode2session?appid=".$this->app_id."&secret=".$this->app_secret."&js_code=$code&grant_type=authorization_code";
		$retJson = curl_get($apiUrl);

		// Log::write("tzoo","-retJson-".$retJson."-appid-".$this->app_id."-secret-".$this->app_secret."-code-".$code);

		$retArr = json_decode($retJson,true);
		if ($retArr["errcode"]==40029) {
			$retStr = 0;
		}else{
			$retStr = $retArr["openid"];
			$_SESSION["userArr"] = $retArr;
			if( !empty($retStr) ){
				$this->saveUser($retArr);
			}
		}
		return $retStr;
	}	
	//记录用户
	private function saveUser($userArr){
		$master = Factory::loadDB("master");
		$userModel = Factory::loadModel("User",$master);
		$result = $userModel->getOne("openid='{$userArr["openid"]}'");

		// Log::write("tzoo","userArr:\n".json_encode($userArr));
		
		if (!empty($result)) {//修改
			$userModel->updateItem($userArr,"openid='{$userArr["openid"]}'");
		}else{//新增
			$userModel->addItem($userArr);
		}
	}
	//解密微信用户基本信息数据
	public function decodeUserInfo($sessionKey,$encryptedData,$iv){
		include_once "wxBizDataCrypt.php";

		$pc = new WXBizDataCrypt($this->app_id, $sessionKey);
		$errCode = $pc->decryptData($encryptedData, $iv, $data );
		$retArr = "";
		if ($errCode == 0) {
		    $retArr = json_decode($data,true);
		} else {
		    $retArr = $errCode;
		}
		// Log::write("tzoo","retArr:".$data);
		return $retArr;
	}
	//微信统一下单
	public function unifiedOrder($openid,$productName,$orderArr){
		ini_set('date.timezone','Asia/Shanghai');
		require_once "Libs/wxpay/lib/WxPay.Api.php";
		require_once "Libs/wxpay/example/WxPay.JsApiPay.php";
		require_once 'Libs/wxpay/example/log.php';
		//初始化日志
		$logHandler= new CLogFileHandler("Libs/wxpay/logs/".date('Y-m-d').'.log');
		$log = Log::Init($logHandler, 15);

		//①、获取用户openid
		$tools = new JsApiPay();
		// $openid = $tools->GetOpenid();
		// Log::DEBUG("title:".$productName."id:".$orderArr);
		//②、统一下单
		$input = new WxPayUnifiedOrder();
		$input->SetBody($productName);//简要描述
		$input->SetAttach("test");//附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
		// $input->SetOut_trade_no(WxPayConfig::MCHID.date("YmdHis"));//商户系统内部的订单号,32个字符内、可包含字母,
		$input->SetOut_trade_no($orderArr["order_no"]);//商户系统内部的订单号,32个字符内、可包含字母,
		$input->SetTotal_fee($orderArr["total_price"]*100);//订单总金额，单位为分
		$input->SetTime_start( date("YmdHis",time()) );//订单生成时间，格式为yyyyMMddHHmmss
		$input->SetTime_expire( date("YmdHis",time() + 3*60*60) );//订单失效时间，格式为yyyyMMddHHmmss
		$input->SetGoods_tag("test");//商品标记，代金券或立减优惠功能的参数
		$input->SetNotify_url("https://".MY_APP_DOMAIN."/pay_notify.html");//接收微信支付异步通知回调地址
		$input->SetTrade_type("JSAPI");//取值如下：JSAPI，NATIVE，APP，
		$input->SetOpenid($openid);//trade_type=JSAPI，此参数必传，用户在商户appid下的唯一标识
		$order = WxPayApi::unifiedOrder($input);
		// echo '<font color="#f00"><b>统一下单支付单信息</b></font><br/>';
		// printf_info($order);
		// Log::DEBUG("order:" . $order);
		// $tools = new JsApiPay();
		$jsApiParameters = $tools->GetJsApiParameters($order);
		// Log::DEBUG("jsApiParameters:" . $jsApiParameters);
		//{"appId":"wx7949b23c7b450278","nonceStr":"bcc63yguia6devcrcam1r6ygf60vzxoi","package":"prepay_id=wx20180402182316bc6aba4b790985744403","signType":"MD5","timeStamp":"1522664596","paySign":"AB773DE57048B2CA96AFB6EFB1666822"}
		return $jsApiParameters;
	}
	//微信退款
	public function refundOrder($wxTransactionId,$refundFee,$totalFee,$source=""){
		ini_set('date.timezone','Asia/Shanghai');
		require_once "Libs/wxpay/lib/WxPay.Api.php";
		require_once "Libs/wxpay/example/WxPay.JsApiPay.php";
		require_once 'Libs/wxpay/example/log.php';
		//初始化日志
		$logHandler= new CLogFileHandler("Libs/wxpay/logs/".date('Y-m-d').'.log');
		$log = Log::Init($logHandler, 15);

		//拿上次的商户退款单号来退款，如果没有则新生成一个,并保存
		if (empty($source)) {
			$orderModel = Factory::loadModel("UserOrder");
			$orderArr = $orderModel->getOne("wx_transaction_id='$wxTransactionId'");
		}else{//拼团订单
			$orderModel = Factory::loadModel("UserOrderGroup");
			$orderArr = $orderModel->getOne("wx_transaction_id='$wxTransactionId'");
		}
		if (empty($orderArr["out_refund_no"])) {
			$outRefundNo = WxPayConfig::MCHID.date("YmdHis");
			$orderModel->updateOne("out_refund_no='$outRefundNo'","wx_transaction_id='$wxTransactionId'");
		}else{
			$outRefundNo = $orderArr["out_refund_no"];
		}

		$input = new WxPayRefund();
		$input->SetTransaction_id($wxTransactionId);
		$input->SetTotal_fee($totalFee);
		$input->SetRefund_fee($refundFee);
	    $input->SetOut_refund_no($outRefundNo);
	    $input->SetOp_user_id(WxPayConfig::MCHID);
		$retArr = WxPayApi::refund($input);
		Log::DEBUG("refund retArr:".json_encode($retArr));
		return $retArr;
	}
	/**
	 * 发送模板消息
	 */
	public function sendPayTemplateMsg($type,$postArr) {
		if($type=="1"){//拼团待成团通知
			$postData = '
			{
	           "touser":"'.$postArr["openid"].'",
	           "template_id":"'.WXAPP_PRE_GROUP.'",
	           "page":"'.$postArr["url"].'",
	           "form_id":"'.$postArr["formId"].'",
	           "data":{
	               "keyword1": {
	                   "value":"'.$postArr["keyword1"].'",
	                   "color":"#173177"
	               },
	               "keyword2":{
	                   "value":"'.$postArr["keyword2"].'",
	                   "color":"#173177"
	               },
	               "keyword3": {
	                   "value":"'.$postArr["keyword3"].'",
	                   "color":"#173177"
	               },
	               "keyword4": {
	                   "value":"'.$postArr["keyword4"].'",
	                   "color":"#173177"
	               },
	               "keyword5":{
	                   "value":"'.$postArr["keyword5"].'",
	                   "color":"#173177"
	               },
	               "keyword6":{
	                   "value":"'.$postArr["keyword6"].'",
	                   "color":"#173177"
	               }
	           }
	        }
			';
		}elseif ($type=="2") {//拼团成功通知
			$postData = '
			{
	           "touser":"'.$postArr["openid"].'",
	           "template_id":"'.WXAPP_GROUP_SUCCESS.'",
	           "page":"'.$postArr["url"].'",
	           "form_id":"'.$postArr["formId"].'",
	           "data":{
	               "keyword1": {
	                   "value":"'.$postArr["keyword1"].'",
	                   "color":"#173177"
	               },
	               "keyword2":{
	                   "value":"'.$postArr["keyword2"].'",
	                   "color":"#173177"
	               },
	               "keyword3": {
	                   "value":"'.$postArr["keyword3"].'",
	                   "color":"#173177"
	               },
	               "keyword4": {
	                   "value":"'.$postArr["keyword4"].'",
	                   "color":"#173177"
	               },
	               "keyword5":{
	                   "value":"'.$postArr["keyword5"].'",
	                   "color":"#173177"
	               }
	           }
	        }
			';
		}elseif ($type=="3") {//拼团失败通知
			$postData = '
			{
	           "touser":"'.$postArr["openid"].'",
	           "template_id":"'.WXAPP_GROUP_FAIL.'",
	           "page":"'.$postArr["url"].'",
	           "form_id":"'.$postArr["formId"].'",
	           "data":{
	               "keyword1": {
	                   "value":"'.$postArr["keyword1"].'",
	                   "color":"#173177"
	               },
	               "keyword2":{
	                   "value":"'.$postArr["keyword2"].'",
	                   "color":"#173177"
	               },
	               "keyword3": {
	                   "value":"'.$postArr["keyword3"].'",
	                   "color":"#173177"
	               },
	               "keyword4": {
	                   "value":"'.$postArr["keyword4"].'",
	                   "color":"#173177"
	               },
	               "keyword5":{
	                   "value":"'.$postArr["keyword5"].'",
	                   "color":"#173177"
	               },
	               "keyword6":{
	                   "value":"'.$postArr["keyword6"].'",
	                   "color":"#173177"
	               }
	           }
	        }
			';
		}elseif ($type=="4") {//开奖结果通知
			$postData = '
			{
	           "touser":"'.$postArr["openid"].'",
	           "template_id":"'.WXAPP_GROUP_WIN.'",
	           "page":"'.$postArr["url"].'",
	           "form_id":"'.$postArr["formId"].'",
	           "data":{
	               "keyword1": {
	                   "value":"'.$postArr["keyword1"].'",
	                   "color":"#173177"
	               },
	               "keyword2":{
	                   "value":"'.$postArr["keyword2"].'",
	                   "color":"#173177"
	               },
	               "keyword3": {
	                   "value":"'.$postArr["keyword3"].'",
	                   "color":"#173177"
	               }
	           }
	        }
			';
		}
		
		$accessToken = $this->getAccessToken();
		$url = "https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token=".$accessToken;
		$retJson = fgc_post_data($url,$postData);
		$retArr = json_decode($retJson,true);

		MpfLog::write("tzoo","template msg:\n"."params:\n".$postData."\n return:\n".$retJson);
		return $retArr;
	}
	//生成小程序码
	public function createMiniQrcode($scene,$page,$fileDir,$fileName){
		$accessToken = $this->getAccessToken();
		$url = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=".$accessToken;
		$postData = '
		{
			"scene":"'.$scene.'",
			"page":"'.$page.'",
			"width":300,
			"auto_color":true,
			"is_hyaline":true
		}
		';
		$result = fgc_post_data($url,$postData);
		File::mkDir($fileDir,0777);
		file_put_contents($fileDir.$fileName, $result);
		// MpfLog::write("tzoo","createMiniQrcode\n".$postData."\n return:\n".$result);
		// Log::DEBUG("sendTemplateMsg:".$postData."\n".$result);
		return ture;
	}





	
	
	/**
	 * 验证签名合法
	 *
	 * @param
	 *        	$paramArr
	 * @return boolean
	 */
	private function checkSignature($paramArr) {
		$signature = $paramArr ["signature"];
		$timestamp = $paramArr ["timestamp"];
		$nonce = $paramArr ["nonce"];
		
		$token = $this->token;
		$tmpArr = array (
				$token,
				$timestamp,
				$nonce 
		);
		sort ( $tmpArr );
		$tmpStr = implode ( $tmpArr );
		$tmpStr = sha1 ( $tmpStr );
		
		if ($tmpStr == $signature) {
			return true;
		} else {
			return false;
		}
	}
	
	/**
	 * CURL请求
	 */
	public function Curl($url) {
		$ch = curl_init ();
		// 设置选项，包括URL
		curl_setopt ( $ch, CURLOPT_URL, $url );
		curl_setopt ( $ch, CURLOPT_RETURNTRANSFER, 1 );
		curl_setopt ( $ch, CURLOPT_HEADER, 0 );
		// 执行并获取HTML文档内容
		$output = curl_exec ( $ch );
		// 释放curl句柄
		curl_close ( $ch );
		// 打印获得的数据
		return $output;
	}
	
	/**
	 * https post请求
	 *
	 * @param
	 *        	$url
	 * @param
	 *        	$data
	 * @return mixed
	 */
	public function https_post($url, $data = null) {
		$curl = curl_init ();
		curl_setopt ( $curl, CURLOPT_URL, $url );
		curl_setopt ( $curl, CURLOPT_SSL_VERIFYPEER, FALSE );
		curl_setopt ( $curl, CURLOPT_SSL_VERIFYHOST, FALSE );
		if (! empty ( $data )) {
			curl_setopt ( $curl, CURLOPT_POST, 1 );
			curl_setopt ( $curl, CURLOPT_POSTFIELDS, $data );
		}
		curl_setopt ( $curl, CURLOPT_RETURNTRANSFER, 1 );
		$output = curl_exec ( $curl );
		curl_close ( $curl );
		return $output;
	}
	
	/**
	 * http get请求
	 *
	 * @param
	 *        	$url
	 * @return mixed boolean
	 */
	public function http_get($url) {
		$oCurl = curl_init ();
		if (stripos ( $url, "https://" ) !== FALSE) {
			curl_setopt ( $oCurl, CURLOPT_SSL_VERIFYPEER, FALSE );
			curl_setopt ( $oCurl, CURLOPT_SSL_VERIFYHOST, FALSE );
		}
		curl_setopt ( $oCurl, CURLOPT_URL, $url );
		curl_setopt ( $oCurl, CURLOPT_RETURNTRANSFER, 1 );
		$sContent = curl_exec ( $oCurl );
		$aStatus = curl_getinfo ( $oCurl );
		curl_close ( $oCurl );
		if (intval ( $aStatus ["http_code"] ) == 200) {
			return $sContent;
		} else {
			return false;
		}
	}
	
	
	//判断微信消息是否有子消息
	private function wxMsgHasSubmenu($msgArr){
		$msgModel = Factory::loadModel('WxMessage');
		$count = $msgModel->getCount("`parent` = {$msgArr['id']}");
		if ($count > 0) {
			$paramsArr['openid'] = $this->fromUsername;
			$paramsArr['message_id'] = $msgArr['id'];
			$master = Factory::loadDB("master");
			$msgUserModel = Factory::loadModel('WxMessageUser');
			$result = $msgUserModel->getOne("`openid`='{$paramsArr["openid"]}'");
			if (!empty($result)) {
				$msgUserModel->updateItem($paramsArr);
			}else{
				$msgUserModel->addItem($paramsArr);
			}
		}
		return true;
	}



	
}
?>