<?php
/**
 * 公用方法类
 *
 */
class PublicService{
	/**
	 * 返回当前页面的 URL 地址
	 *
	 * @return string
	 */
	public function getCurrentUrl(){
		$http = isset($_SERVER["HTTPS"])&&$_SERVER["HTTPS"] ? 'https' : 'http';
		$http .= '://';
		return $http.$_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
	}
	/**
	 * 判断是否登录然后跳转/返回是否登录
	 *
	 * @param boolean $needReturn 是否需要返回值
	 * @return boolean
	 */
	public function checkIsLogin($needReturn=false){
		$currentUrl	= encode_str($this->getCurrentUrl());
		$uid = Cookie::get("bsud",true);
		$au = Cookie::get("bsau");
		$authCode = get_auth_code($uid);
		$isLogin = (empty($uid) || $authCode != $au) ? false : true;
		if (!$needReturn) {
			if(!$isLogin){
				redirect("http://".MY_APP_DOMAIN."/main_login_f_".$currentUrl.".html");
				exit();
			}
			//$this->checkOnlineTime($uid);
		}else {
			//if($isLogin) $this->checkOnlineTime($uid);
			return $isLogin;
		}
	}
	/**
	 * 页面头部
	 */
	static function getHeaderHtml($data=''){
		//是否登录
		$psObj = new PublicService();
		$isLogin = $psObj->checkIsLogin(true);
		$data["isLogin"] = $isLogin;
		if($isLogin){
			$uid = Cookie::get("bsud",true);
			$userArr = get_user_info($uid);
			//判断注册ip是否为空
			if ($userArr['register_ip'] == ''){
				$register_ip = get_client_ip();
				//写DB
				$master = Factory::loadDB("master");
				$itemObjW = Factory::loadModel("User",$master);
				$itemId = $itemObjW->updateById($uid, array('register_ip'=>$register_ip));
			}
			$data["userArr"] = $userArr;
		}else{
			$data["userArr"] = array();
		}
		return View::showHtml("header.html" ,$data);
	}
	/**
	 * 页面底部
	 */
	static function getFooterHtml($data=''){
		
		return View::showHtml("footer.html" ,$data);
	}
	
	static function getIncludeHtml($pageName,$data=''){
		return View::showHtml($pageName,$data);
	}
	/**
	 * 页面上层导航菜单
	 */
	static function getTopMenuHtml($val=""){
		$data["menu"] = $val;
		return View::showHtml("top_menu.html" ,$data);
	}
	/**
	 * 用户管理页左边菜单
	 */
	static function getUserLeftMenuHtml($val){
		$data["menu"] = $val;
		return View::showHtml("user_left_menu.html" ,$data);
	}
	/**
	 * about页公共菜单
	 */
	static function getAboutMenuHtml($val=""){
		$data["menu"] = $val;
		return View::showHtml("about/about_menu.html" ,$data);
	}
	
	
	
	//后台公共方法
	/**
	 * 判断是否登录然后跳转/返回是否登录
	 *
	 */
	public function checkLoginBackend($needReturn=false){
		$currentUrl	= encode_str($this->getCurrentUrl());
		$uid = Cookie::get("tybkud",true);
		$au = Cookie::get("tybkau");
		$authCode = get_auth_code($uid);
		$isLogin = (empty($uid) || $authCode != $au) ? false : true;
		if (!$needReturn) {
			if(!$isLogin){
				redirect("http://".MY_APP_DOMAIN."/main_login_f_".$currentUrl.".html");
				exit();
			}
		}else {
			return $isLogin;
		}
	}
	/**
	 * 后台页面头部
	 */
	static function getHeaderHtmlBackend($data=''){
		if (empty($data)) {
			$data['no_visible_elements'] = 0;
		}	
		if (empty($_SESSION['bkUserArr'])) {
			$uid = Cookie::get("tybkud",true);
			$userModel = Factory::loadModel("BkUser");
			$_SESSION['bkUserArr'] = $userModel->getById($uid);
		}
		$data['bkUserArr'] = $_SESSION['bkUserArr'];
		return View::showHtml("init/header.html" ,$data);
	}
	/**
	 * 后台页面底部
	 */
	static function getFooterHtmlBackend($data=''){
		if (empty($data)) {
			$data['no_visible_elements'] = 0;
		}
		return View::showHtml("init/footer.html" ,$data);
	}
	
	
	/**
	 * 记录最后一次在线时间
	 * qm_lt:last time,每天最后一次操作时间
	 */
	public function checkOnlineTime($uid){
		if (!empty($_COOKIE["qm_lt@$uid"])) {
			$onlineTime = time() - $_COOKIE["qm_lt@$uid"];
			if ($onlineTime > 5*60) {//在线时间超过5分钟记录一次
				$this->setLastOnlineTime($uid);
				Cookie::set("qm_lt@$uid",time(),24*60*60);
			}
		}else{
			Cookie::set("qm_lt@$uid",time(),24*60*60);
		}
		// 更新用户在线时间
		if (!empty($_COOKIE["qm_ont@$uid"])) {
			$onlineTime = time() - $_COOKIE["qm_ont@$uid"];
			if ($onlineTime > 5*60) {//在线时间超过5分钟记录一次
				Cookie::set("qm_ont@$uid",time(),6*60);
				// 更新用户在线时间 tony 2011-07-05
				curl_post_data("http://api.".MY_ROOT_DOMAIN."/user_updateUserOnlineTime.html", array("uid"=>$uid, "au"=>substr(md5(MY_SAFE."_".$uid),0,8), "minute"=>intval($onlineTime/60)));
			}
		}else{
			Cookie::set("qm_ont@$uid",time(),10*60);
		}
	}
	/**
	 * 记录最后一次在线时间，写DB
	 *
	 */
	public function setLastOnlineTime($uid){
		file_get_contents("http://api.".MY_ROOT_DOMAIN."/user_setLastOnlineTime_u_".$uid."_au_".substr(md5(MY_SAFE."_".$uid),0,8));
	}
	//发送模板消息
	public function sendAppTemplateMsg($type,$openid,$url,$formId,$keywordArr){
		if($type=="1"){//拼团待成团通知
			$templateArr["openid"] = $openid;
			$templateArr["url"] = $url;
			$templateArr["formId"] = $formId;
			$templateArr["keyword1"] = "【抽奖团】| ".$keywordArr["keyword1"];
			$templateArr["keyword2"] = $keywordArr["keyword2"];
			$templateArr["keyword3"] = $keywordArr["keyword3"];
			$templateArr["keyword4"] = $keywordArr["keyword4"];
			$templateArr["keyword5"] = $keywordArr["keyword5"];
			$templateArr["keyword6"] = "赶紧邀请好友一起拼团吧！只有拼团成功的用户才有抽奖资格哦~";
			$wxApp = new WxApp();
			$retStr = $wxApp->sendPayTemplateMsg($type,$templateArr);
		}elseif ($type=="2") {//拼团成功通知
			$templateArr["openid"] = $openid;
			$templateArr["url"] = $url;
			$templateArr["formId"] = $formId;
			$templateArr["keyword1"] = "【抽奖团】| ".$keywordArr["keyword1"];
			$templateArr["keyword2"] = $keywordArr["keyword2"];
			$templateArr["keyword3"] = $keywordArr["keyword3"];
			$templateArr["keyword4"] = $keywordArr["keyword4"];
			$templateArr["keyword5"] = "恭喜您拼团成功，抽奖结果将于次日中午12点公布，您可至“我的拼团订单”查看！";
			$wxApp = new WxApp();
			$retStr = $wxApp->sendPayTemplateMsg($type,$templateArr);
		}elseif ($type=="3") {//拼团失败通知
			$templateArr["openid"] = $openid;
			$templateArr["url"] = $url;
			$templateArr["formId"] = $formId;
			$templateArr["keyword1"] = "【抽奖团】| ".$keywordArr["keyword1"];
			$templateArr["keyword2"] = $keywordArr["keyword2"];
			$templateArr["keyword3"] = "很遗憾，您的拼团因人数不足而失败";
			$templateArr["keyword4"] = $keywordArr["keyword4"];
			$templateArr["keyword5"] = $keywordArr["keyword5"];
			$templateArr["keyword6"] = "别气馁，再去看看其他商品吧！Travelzoo旅游族送您5元无门槛优惠券，点击立即领取~";
			$wxApp = new WxApp();
			$retStr = $wxApp->sendPayTemplateMsg($type,$templateArr);
		}elseif ($type=="4") {//开奖结果通知
			$templateArr["openid"] = $openid;
			$templateArr["url"] = $url;
			$templateArr["formId"] = $formId;
			$templateArr["keyword1"] = "【抽奖团】| ".$keywordArr["keyword1"];
			$templateArr["keyword2"] = $keywordArr["keyword2"];
			$templateArr["keyword3"] = "您购买的商品已经开奖啦，赶紧去“我的拼团列表”查看抽奖结果吧！";
			$wxApp = new WxApp();
			$retStr = $wxApp->sendPayTemplateMsg($type,$templateArr);
		}
		//修改formid表中使用数量加1，如果发送失败则修改此条formid为无效
		$formModel = Factory::loadModel("UserFormId");
		if($retStr["errcode"]==0){
			$formModel->updateOne("send_num=send_num+1","form_id='$formId'");
		}else{
			$formModel->updateOne("status=0","form_id='$formId'");
		}
		return $retStr;
	}
	/**
	 * 云片发短信
	 */
	public function yunpianSendSms($phone,$text){
		$apikey = YUN_SMS_KEY;
		$url="http://yunpian.com/v1/sms/send.json";
		$encoded_text = urlencode("$text");
		$post_string="apikey=$apikey&text=$encoded_text&mobile=$phone";
		return sock_post($url, $post_string);
	}
	/**
	 * 发送邮件
	 *
	 */
	public function sendMail($title,$body,$toEmailArr){
		if (is_array($toEmailArr)){
			$emailStr = implode(";",$toEmailArr);
		}else{
			$emailStr = $toEmailArr;
		}
		$url = 'http://sendcloud.sohu.com/webapi/mail.send.json';
        $API_USER = 'huixiang_test_oWjjZe';
        $API_KEY = 'CqoYgU0JmmMmaAMQ';
        $param = array(
            'api_user' => $API_USER, # 使用api_user和api_key进行验证
            'api_key' => $API_KEY,
            'from' => 'Tzoo@dKDBAhvLT5TF1FWZgaEkMbUSOFV0hAUH.sendcloud.org', # 发信人，用正确邮件地址替代
            'fromname' => 'Travelzoo小程序',
            'to' => $emailStr,# 收件人地址, 用正确邮件地址替代, 多个地址用';'分隔  
            'subject' => $title,
            'html' => $body,
            'resp_email_id' => 'true'
        );
        $data = http_build_query($param);
        $options = array(
            'http' => array(
                'method' => 'POST',
                'header' => 'Content-Type: application/x-www-form-urlencoded',
                'content' => $data
        ));
        $context  = stream_context_create($options);
        $result = file_get_contents($url, FILE_TEXT, $context);
        return $result;
	}


	/**
	 * 获取RSS数据
	 * @param string  $title :标题
	 * @param string  $link  :链接
	 * @param string  $description  :简介
	 * @param string  $content  :内容
	 * @return rss
	 */

	static function getRss($title='', $link='', $description='', $content='') {
		$rss  = '<?xml version="1.0" encoding="UTF-8"?><rss version="2.0"><channel><title>'.$title.'</title><link>'.$link.'</link><description>'.$description.'</description><language>zh-cn</language><copyright>Copyright (C) TIAO.COM. All rights reserved.</copyright><generator>www.tiao.com RSS Generator</generator>';
		$rss .= $content;
		$rss .= '</channel></rss>';
		return $rss;
	}
	
}
?>