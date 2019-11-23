<?php
/**
 *
 * @author rogerzhao
 *        
 */
class WeChat {
	private $fromUsername;
	private $toUsername;
	private $times;
	private $keyword;
	private $app_id;
	private $app_secret;
	private $token;
	private $config;
	private $paramArr;
	private $mem_suffix;
	static public function getInstence($params = array()) {
		return new WeChat ( $params );
	}
	
	/**
	 * 构造方法
	 * 定义token串，需与微信后台token字符串相同
	 * 定义app_id和app_secret
	 */
	public function __construct() {
		$this->token = WECHAT_APP_TOKEN;
		$this->mem_suffix = WECHAT_MEM_SUFFIX;
		$this->app_id = WECHAT_APP_ID;
		$this->app_secret = WECHAT_APP_SECRET;
	}
	
	/**
	 * 验证url合法性
	 *
	 * @param array $paramArr        	
	 */
	public function valid($paramArr) {
		$echoStr = $paramArr ["echostr"];
		if ($this->checkSignature ( $paramArr )) {
			echo $echoStr;
			exit ();
		}
	}
	
	/**
	 * 运行程序
	 *
	 * @param string $value
	 *        	[description]
	 */
	public function Run() {
		$this->responseMsg ();
		$arr [] = "";
		echo $this->make_xml ( "text", $arr );
	}
	
	/**
	 * 日志
	 *
	 * @param string $text        	
	 */
	private function logdebug($text) {
		file_put_contents ( 'logwechat.txt', $text . "\n", FILE_APPEND );
	}
	
	/**
	 * 事件函数
	 */
	public function responseMsg() {
		$postStr = $GLOBALS ["HTTP_RAW_POST_DATA"]; // 返回回复数据
		if (! empty ( $postStr )) {
			$postObj = simplexml_load_string ( $postStr, 'SimpleXMLElement', LIBXML_NOCDATA );
			$this->fromUsername = $postObj->FromUserName; // 发送消息方ID
			$this->toUsername = $postObj->ToUserName; // 接收消息方ID
			$this->keyword = trim ( $postObj->Content ); // 用户发送的消息
			$keyword = trim ( $postObj->Content ); // 获得用户发送的消息<<<此用法是否正确？？？
			$this->times = time (); // 发送时间
			$MsgType = $postObj->MsgType; // 消息类型
			//记录用户
			$this->saveUser($postObj);
			
			switch ($MsgType)
			{
				case "location":
					$this->receiveLocation($postObj);
					break;
				case "event":
					$this->receiveEvent($postObj);
					break;
				case "text":
					$this->receiveText($postObj);
					break;
				case "image":
					$this->receiveImage($postObj);
					break;
				default:
					$this->defaultMsg();
					break;
			}
			
			
		} else {
			echo "";
			exit ();
		}
	}
	
	
	//接受事件消息
	private function receiveEvent($postObj){
	
		switch ($postObj->Event){
			case 'subscribe'://关注				
				$arr [] = array (
						"欢迎关注加州健身！",
						"欢迎关注加州健身！
Welcome to Californiafitness

地址
上海市卢湾区淮海中路138号上海广场5楼及6楼 (上海大时代广 场对面)

开放时间
週一至週五:
早上6:00至午夜12:00
星期六，星期日及法定节假日*:
早上8:00至晚上10:00 

* 法定节假期开放时间可能会有更改，请向会所查询.",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRA0Gyw86D3YRyrGQGXzkgsfXA0UzWftGLQqyIYeFL9XOUQpSphI1IxkTB6tdWJKpWAlGOV0IqLIA/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=207165010&idx=1&sn=112c467fc462123c62c20de8450ad859#rd"
				);
				
				echo $this->make_xml ( "news", $arr, array (
						1,
						0
				) );
				break;
	
			case 'SCAN'://二维码							
				$arr [] = array (
						"欢迎关注加州健身！",
						"欢迎关注加州健身！
Welcome to Californiafitness
				
地址
上海市卢湾区淮海中路138号上海广场5楼及6楼 (上海大时代广 场对面)
				
开放时间
週一至週五:
早上6:00至午夜12:00
星期六，星期日及法定节假日*:
早上8:00至晚上10:00
				
* 法定节假期开放时间可能会有更改，请向会所查询.",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRA0Gyw86D3YRyrGQGXzkgsfXA0UzWftGLQqyIYeFL9XOUQpSphI1IxkTB6tdWJKpWAlGOV0IqLIA/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=207165010&idx=1&sn=112c467fc462123c62c20de8450ad859#rd"
				);
				
				echo $this->make_xml ( "news", $arr, array (
						1,
						0
				) );
				break;
	
			case 'unsubscribe'://取消关注
				$userArr = $this->getUserInfoByOpenID($this->fromUsername);
				$master = Factory::loadDB("master");
				$userModel = Factory::loadModel("User",$master);
				$userModel->updateItem($userArr);
				echo "success";
				break;
	
			case 'CLICK'://菜单事件
				$this->receiveClick($postObj);
				break;
			
			case 'LOCATION':
				echo "success";
				break;
		}
				
	}
	
	//记录用户
	private function saveUser($postObj){
		$userArr = array();
		$userArr = $this->getUserInfoByOpenID($this->fromUsername);
		if (isset ( $postObj->EventKey ) && $postObj->Event == 'subscribe') {//存在key值为二维码扫描关注事件
			$keyArr = explode("_", $postObj->EventKey);
			$userArr["from"] = $keyArr[1];
		}
		$master = Factory::loadDB("master");
		$userModel = Factory::loadModel("User",$master);
		$result = $userModel->getOne("`openid`='{$userArr["openid"]}'");
		if (!empty($result)) {
			//修改用户记录
			$userModel->updateItem($userArr);
		}else{
			//插入新记录
			if ($userArr['openid'])
				$userModel->addItem($userArr);
		}
	}
	
	
	//接受文本消息
	private function receiveText($postObj){
		$keyword = $this->keyword;
		if (!empty ( $keyword )) {
			if(strstr($keyword, '预约')){
				$arr [] = "您好，了解预约课程详情，请拨打客服热线4008 100 988，感谢您的关注与支持！";
				echo $this->make_xml ( "text", $arr );
				exit;
			}

			if(strstr($keyword, '课程表')){
				$arr [] = '掌上课程表，查阅更方便，点击<a href="http://'.MY_APP_DOMAIN.'/course_showlist.html">查看详情</a>';
				echo $this->make_xml ( "text", $arr );
				exit;
			}
			//用于那些不会绑定的用户，获取其openid后手动帮其在数据库绑定
			if(strstr($keyword, '绑定')){
				$postArr["openid"] = $this->fromUsername;
				$postArr["keyword"] = $keyword;
				$unbindModel = Factory::loadModel("UserUnbindTemp");
				$unbindModel->addItem($postArr);
				$arr [] = '感谢您的回复！我们的工作人员在完成绑定后会与您电话联系，请保持手机畅通！';
				echo $this->make_xml ( "text", $arr );
				exit;
			}
			
			if (preg_match('/^[0-9a-z]{12}$/i',$keyword)){
				$keyModel = Factory::loadModel("LotteryKey");
	    			$returnArr = $keyModel->getOne("randkey = '{$keyword}'");
	    			if (empty($returnArr)) {
	    				$arr [] = "兑奖码错误，请再次核对";
	    			}else{
	    				if ($returnArr["status"] == 1) {
	    					$arr [] = "兑奖码已使用";
	    				}else{
	    					switch ($returnArr["level"]){
	    						case 1: $prize = "加州健身一个月VIP贵宾卡一张，价值1800元。";
	    						break;
	    						case 2: $prize = "价值1000元的adidas运动礼包一个。";
	    						break;
	    						case 3: $prize = "价值199元的adidas运动礼品一份。";
	    						break;
	    						case 4: $prize = "限量版加州小熊一个。";
	    						break;
	    						case 5: $prize = "价值59元的adidas运动礼品一份。";
	    						break;
	    						case 6: $prize = "加州健身七天VIP贵宾卡一张，价值699元。";
	    						break;
	    					}
	    					$arr [] = "恭喜您，获得了".$prize."请带上消费小票、兑奖券以及本条兑奖回复，至上海广场5楼加州健身前台领奖。";	    					
	    				}	 
	    			}
	    			$log = date("Y-m-d H:i:s")."\t".$this->fromUsername."\t".$keyword."\r";
	    			@file_put_contents('log/QueryLotteryKeyRecords.txt', $log,FILE_APPEND);
	    			echo $this->make_xml ( "text", $arr );
			}
			
			switch ($keyword) {				
				default :		
					$arr [] = array (
						"欢迎关注加州健身！",
						"欢迎关注加州健身！
Welcome to Californiafitness
				
地址
上海市卢湾区淮海中路138号上海广场5楼及6楼 (上海大时代广 场对面)
				
开放时间
週一至週五:
早上6:00至午夜12:00
星期六，星期日及法定节假日*:
早上8:00至晚上10:00
				
* 法定节假期开放时间可能会有更改，请向会所查询.",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRA0Gyw86D3YRyrGQGXzkgsfXA0UzWftGLQqyIYeFL9XOUQpSphI1IxkTB6tdWJKpWAlGOV0IqLIA/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=207165010&idx=1&sn=112c467fc462123c62c20de8450ad859#rd"
				);
				
				echo $this->make_xml ( "news", $arr, array (
						1,
						0
				) );
					//$this->transmitService($postObj);
					break;
			}
	
		}
	}
	
	//接受菜单事件
	private function receiveClick($postObj){// 菜单的自定义的key值，可以根据此值判断用户点击了什么内容，从而推送不同信息
	
		switch ($postObj->EventKey) {
			case "M1_001" :
				$arr [] = array (
						"关于加州健身 ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7orkBtnyDhRkpBILydEjJicCqWeJ6Fks2E7kwuQ4WjUXXicoFTXjTqxpyA/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209148072&idx=1&sn=f2cf0ae4f016067b0bff35f9ebef7db9#rd"
				);
				$arr [] = array (
						"加州健身——亚洲连锁健身品牌 ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7ofK44IPZ61QKnPKva8XcL6To7EhXV7uDibksmB5TlXbP9AWVibfVEVn8w/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209148072&idx=2&sn=6567906e94e0bd123d031d0a0727d7b0#rd"
				);
				
				echo $this->make_xml ( "news", $arr, array (
						2,
						0
				) );
				break;
			case "M2_001" :
				$arr [] = "敬请期待！";
				echo $this->make_xml ( "text", $arr );
				break;
			case "M2_002" :
				$arr [] = array (
						"【加州教练风采】90后泰拳教练Jason，带你揭秘泰拳！",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7o21OWC7EONqwDGz0kyAnbx1hicSsFZibrel3oich7UgwpJZVgociax7pxiaA/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209202223&idx=1&sn=f864fc8f0ed9c3b325a5d4944d9cbd16#rd"
				);
				$arr [] = array (
						"【加州教练风采】Your Body is Your Gym ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7oo6Ey2ib9FqeFwibeVicQtSxjZ1vgtcPOHMDU3aYic05g4Cmzl3w9xeibtoQ/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209202223&idx=2&sn=802634b18ffdc782f88d41f7b8c9c674#rd"
				);
				
				echo $this->make_xml ( "news", $arr, array (
						2,
						0
				) );
				break;
			case "M2_003" :
				$arr [] = array (
						"团体课程 ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKTGWx77iccn4uic2dstCmaV4DNWqStU8G2jCM6WC3e6G7ke6o4wAw4qdZ0qutPXTpx0mmUMsAtiaTv0Q/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209269947&idx=1&sn=efb29c2f04959428606ae0f0ff0fcb67#rd"
				);
				$arr [] = array (
						"Les Mills 莱美运动课程 ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKTGWx77iccn4uic2dstCmaV4DpqxeQMicXGxJ6r82EHtHK3LiaaoCRz0nibuI1fhslGoYpJwQnt7Ywz2xQ/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209269947&idx=2&sn=3b62f5a9f7a97f7f0cfa4b91e73bbac0#rd"
				);
				$arr [] = array (
						"瑜伽及身心灵课程 ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKTGWx77iccn4uic2dstCmaV4D3zHA2IjkSBTSOCI6zr30MHY6AU46HGTyBusVa6YWQO7mpeYC9tG8VQ/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209269947&idx=3&sn=e8c78436ee01b7a9f99929fd0b5441bd#rd"
				);
				$arr [] = array (
						"心肺锻炼 ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKTGWx77iccn4uic2dstCmaV4Du47xyrveVcsC1aIS1BqIia5ChGvqLvUXSCLJzNkcLuIicDabDccuzNPg/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209269947&idx=4&sn=d322576bd0570f63e729de45299f3922#rd"
				);
				$arr [] = array (
						"室内团体单车课 ",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKTGWx77iccn4uic2dstCmaV4DBjzqJQl5xT8eKPegnJP4FBhZxnE7Cx3wUjicOtZpt6icFuvLyUoqUI4w/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209269947&idx=5&sn=6880988c93c00063e2a53e686a4a7eef#rd"
				);				
				echo $this->make_xml ( "news", $arr, array (
						5,
						0
				) );
				break;								
			case "M3_003" :
				$arr [] = array (
						"会籍介绍",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7od6Y0gTD5jQ28WbjHsIKNOqXFq6cjibrR52yJ5JDmmn2hictWzvcd0zMQ/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209203078&idx=1&sn=d0ffc565be8fc5e1aaf0bb0c40006e8b#rd"
				);
				$arr [] = array (
						"加州健身上海单一会籍",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7oUpFH04giaW8iat8icKCtHic9AyMutmMRCE3jpf2mXNZJzLibSgMRIeT4SCg/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209203078&idx=2&sn=d25f1fd5cc13c24c815c9fde6161595e#rd"
				);
				$arr [] = array (
						"加州健身亚洲会籍",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7oBrAOZo7S9drACp1TzExdY5asaMX7HP2hv2QxNJUv7OYU4n6gHkxAtw/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209203078&idx=3&sn=8d487d85c7a1f0e153de50f3cb0f274c#rd"
				);
				$arr [] = array (
						"Zenith会籍",
						"",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRkIMP9V6uwYku6IPiaMyz7oIulUY1kt1adibpZL0WSFjWiahZ2iaVfhmib0PYICothcfaz0tia0YLBGeZw/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=209203078&idx=4&sn=e591f313cdd5062c33d144b24b7019dd#rd"
				);
				echo $this->make_xml ( "news", $arr, array (
						4,
						0
				) );
				break;
	
			default :
				$arr [] = "敬请期待！";
				echo $this->make_xml ( "text", $arr );
				break;
		}
	}
	
	private function receiveImage($postObj){
		$arr [] = array (
						"欢迎关注加州健身！",
						"欢迎关注加州健身！
Welcome to Californiafitness
				
地址
上海市卢湾区淮海中路138号上海广场5楼及6楼 (上海大时代广 场对面)
				
开放时间
週一至週五:
早上6:00至午夜12:00
星期六，星期日及法定节假日*:
早上8:00至晚上10:00
				
* 法定节假期开放时间可能会有更改，请向会所查询.",
						"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRA0Gyw86D3YRyrGQGXzkgsfXA0UzWftGLQqyIYeFL9XOUQpSphI1IxkTB6tdWJKpWAlGOV0IqLIA/0?wx_fmt=jpeg",
						"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=207165010&idx=1&sn=112c467fc462123c62c20de8450ad859#rd"
				);
				
		echo $this->make_xml ( "news", $arr, array (1,0) );
		exit();
	}
	
	private function defaultMsg(){
		$arr [] = array (
				"欢迎关注加州健身！",
				"欢迎关注加州健身！
Welcome to Californiafitness
	
地址
上海市卢湾区淮海中路138号上海广场5楼及6楼 (上海大时代广 场对面)
	
开放时间
週一至週五:
早上6:00至午夜12:00
星期六，星期日及法定节假日*:
早上8:00至晚上10:00
	
* 法定节假期开放时间可能会有更改，请向会所查询.",
				"https://mmbiz.qlogo.cn/mmbiz/A8YngaajHKRA0Gyw86D3YRyrGQGXzkgsfXA0UzWftGLQqyIYeFL9XOUQpSphI1IxkTB6tdWJKpWAlGOV0IqLIA/0?wx_fmt=jpeg",
				"http://mp.weixin.qq.com/s?__biz=MzA3MzAyNTAzOA==&mid=207165010&idx=1&sn=112c467fc462123c62c20de8450ad859#rd"
		);
	
		echo $this->make_xml ( "news", $arr, array (1,0) );
		exit();
	}
	
	//多客服事件
	private function transmitService($postObj){
		$xmlTpl = "<xml>
<ToUserName><![CDATA[%s]]></ToUserName>
<FromUserName><![CDATA[%s]]></FromUserName>
<CreateTime>%s</CreateTime>
<MsgType><![CDATA[transfer_customer_service]]></MsgType>
</xml>";
		$result = sprintf($xmlTpl, $postObj->FromUserName, $postObj->ToUserName, time());
		echo $result;
	}
	
	//定位事件
	private function receiveLocation($postObj){		
		$this->defaultMsg();
	}
	
	/**
	 * 获取access_token
	 * access_token缓存有效期7100秒
	 */
	private function get_access_token() {
		$mem = new Memcached ();
		$mem->addServer ( 'localhost', 11211 );
		$access_token = $mem->get ( 'access_token_'.$this->mem_suffix );
		if (empty ( $access_token )) {
			$url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" . $this->app_id . "&secret=" . $this->app_secret;
			$data = json_decode ( file_get_contents ( $url ), true );
			$access_token = $data ["access_token"];
			$mem->set ('access_token_'.$this->mem_suffix , $access_token, 7100 );
		}
		return $access_token;
	}
	
	/**
	 * 客服接口发送文本消息
	 */
	public function sendTextMsg($msg,$openid) {
		$txt = '{
		    "touser":"'.$openid.'",
		    "msgtype":"text",
		    "text":
		    {
		      "content":"'.$msg.'"
		    }
		}';
		$access_token = $this->get_access_token ();
		$url = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" . $access_token;
		$result = $this->https_post($url,$txt);
	}
	
	/**
	 * 创建自定义菜单
	 *
	 * @return mixed
	 */
	public function createMenu($menuJson) {
		$access_token = $this->get_access_token ();
		$url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" . $access_token;
		// echo '<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">';
		
		$xjson = $menuJson;
		
		$ch = curl_init ();
		iconv ( "gb2312", "utf-8", $xjson );
		curl_setopt ( $ch, CURLOPT_URL, $url );
		curl_setopt ( $ch, CURLOPT_RETURNTRANSFER, 1 );
		curl_setopt ( $ch, CURLOPT_POST, 1 );
		curl_setopt ( $ch, CURLOPT_POSTFIELDS, $xjson );
		
		$tmpInfo = curl_exec ( $ch ); // 执行操作
		curl_close ( $ch );
		return $tmpInfo; // 返回数据
	}
	
	/**
	 * 查询菜单
	 *
	 * @param $access_token 已获取的ACCESS_TOKEN        	
	 */
	public function getMenu() {
		$access_token = $this->get_access_token ();
		$url = "https://api.weixin.qq.com/cgi-bin/menu/get?access_token=" . $access_token;
		$data = file_get_contents ( $url );
		return $data;
	}
	
	/**
	 * 获取二维码凭证Ticket
	 *
	 * @param
	 *        	自定义参数scene_id
	 * @return mixed
	 */
	public function getTicket($params) {
		$access_token = $this->get_access_token ();
		$qrcode = '{"action_name": "QR_LIMIT_SCENE", "action_info": {"scene": {"scene_id": ' . $params . '}}}';
		
		$url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" . $access_token;
		$result = $this->https_post ( $url, $qrcode );
		$jsoninfo = json_decode ( $result, true );
		$ticket = $jsoninfo ["ticket"];
		
		// file_put_contents("images/url.txt",$jsoninfo["url"]."\r\n",FILE_APPEND);
		
		return $ticket;
	}
	
	/**
	 * 生成二维码图片
	 *
	 * @param
	 *        	params 自定义参数scene_id
	 *        	imgName 图片名称
	 * @return mixed
	 */
	public function getQRCode($params, $imgName) {
		$ticket = $this->getTicket ( $params );
		// echo $ticket;
		$url = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" . urlencode ( $ticket );
		$imageInfo = $this->downloadImage ( $url );
		// var_dump($imageInfo);die();
		$filename = 'images/qrcode/' . $imgName . '.jpg';
		$local_file = fopen ( $filename, 'w' );
		if (false !== $local_file) {
			if (false !== fwrite ( $local_file, $imageInfo ["body"] )) {
				fclose ( $local_file );
			}
		}
	}
	
	/**
	 * 获取微信js的签名ticket
	 * 
	 * @return
	 *
	 */
	private function getJsApiTicket() {
		$access_token = $this->get_access_token ();
		$mem = new Memcached ();
		$mem->addServer ( 'localhost', 11211 );
		$jsapi_ticket = $mem->get ( 'jsapi_ticket_'.$this->mem_suffix );
		if (empty ( $jsapi_ticket )) {
			$url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?type=jsapi&access_token=$access_token";
			$res = json_decode ( $this->http_get ( $url ) );
			$jsapi_ticket = $res->ticket;
			$mem->set ( 'jsapi_ticket_'.$this->mem_suffix, $jsapi_ticket, 7100 );
		}
		
		return $jsapi_ticket;
	}
	
	/**
	 * 生成微信js签名并返回配置信息
	 * @ruturn 微信js需要的配置信息
	 */
	public function getJsConfig() {
		// 随机字符串
		$nonceStr = "minutes";
		// 时间戳
		$timestamp = time ();
		// 当前页面
		$url = "http://$_SERVER[HTTP_HOST]$_SERVER[REQUEST_URI]";
		// 获取ticket
		$ticket = $this->getJsApiTicket ();
		// 拼接字符串
		$string = "jsapi_ticket={$ticket}&noncestr={$nonceStr}&timestamp={$timestamp}&url={$url}";
		// 生成签名
		$signature = sha1 ( $string );
		
		$returnArr = array (
				'url' => $url,
				'appId' => $this->app_id,
				'nonceStr' => $nonceStr,
				'timestamp' => $timestamp,
				'signature' => $signature 
		);
		return $returnArr;
	}
	
	/**
	 * 通过openid获取关注者信息
	 * @param $openid 
	 * @return user数组
	 */
	public function getUserInfoByOpenID($openid){
		$access_token = $this->get_access_token ();
		$url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={$access_token}&openid={$openid}&lang=zh_CN";
		$response = file_get_contents ( $url );
		if (strpos ( $response, "callback" ) !== false) {
			$lpos = strpos ( $response, "(" );
			$rpos = strrpos ( $response, ")" );
			$response = substr ( $response, $lpos + 1, $rpos - $lpos - 1 );
			$msg = json_decode ( $response );
			if (isset ( $msg->errcode )) {
				echo "<h3>error:</h3>" . $msg->errcode;
				echo "<h3>msg  :</h3>" . $msg->errmsg;
				exit ();
			}
		}
		$userArr = json_decode ( $response,true );
		return $userArr;
	}
	
	/**
	 * 通过media_id下载多媒体资源
	 * @param $media_id 资源ID
	 * 		  $fileType 文件类型 1图片 2语音
	 * @return 
	 */
	public function downloadMediaByMediaID($media_id,$fileDir,$fileType = 1){
		$access_token = $this->get_access_token ();
		$url = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={$access_token}&media_id={$media_id}";
		$fileInfo = $this->downloadImage($url);
		if ($fileType == 1) {
			$dirName = 'images';
			$fileSuffix = '.jpg';
		}elseif ($fileType == 2){
			$dirName = 'audio';
			$fileSuffix = '.mp3';
		}
		$uploadDir = "../Public/".$dirName."/upload/".$fileDir."/";
		File::mkDir($uploadDir,0777);
		$fileName = date("Ymd").getRand(12).$fileSuffix;
		saveFile($uploadDir.$fileName, $fileInfo['body']);
		return $fileName;
	}
	
	/**
	 * 网页授权获取用户信息
	 * @param $oauthType 授权方式，默认1
	 * @return user对象
	 */
	public function webOauth($my_url ,$oauthType = 1) {
		
		// 应用授权作用域(snsapi_base;snsapi_userinfo )
		if ($oauthType == 1)
			$scope = "snsapi_base";
		else
			$scope = "snsapi_userinfo";
			
		// Step1：获取Authorization Code
		$code = $_REQUEST ["code"];
		if (empty ( $code )) {
			// state参数用于防止CSRF攻击，成功授权后回调时会原样带回
			$_SESSION ['state'] = md5 ( uniqid ( rand (), TRUE ) );
			// 拼接URL
			$dialog_url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" . $this->app_id . "&redirect_uri=" . urlencode ( $my_url ) . "&response_type=code&scope=" . $scope . "&state=" . $_SESSION ['state'] . "#wechat_redirect";
			redirect($dialog_url);
			//echo ("<script> top.location.href='" . $dialog_url . "'</script>");
		}
		
		//echo $code."tirami_1";
		
		// Step2：通过Authorization Code获取Access Token,OpenID
		if ($_REQUEST ['state'] == $_SESSION ['state']) {
			// 拼接URL
			$token_url = "https://api.weixin.qq.com/sns/oauth2/access_token?" . "appid=" . $this->app_id . "&secret=" . $this->app_secret . "&code=" . $code . "&grant_type=authorization_code";
			
			$response = file_get_contents ( $token_url );
			if (strpos ( $response, "callback" ) !== false) {
				$lpos = strpos ( $response, "(" );
				$rpos = strrpos ( $response, ")" );
				$response = substr ( $response, $lpos + 1, $rpos - $lpos - 1 );
				$msg = json_decode ( $response );
				if (isset ( $msg->errcode )) {
					echo "<h3>error:</h3>" . $msg->errcode;
					echo "<h3>msg  :</h3>" . $msg->errmsg;
					exit ();
				}
			}
			
			// 获取返回的user对象
			$userArr = json_decode ( $response ,true);			
			$openid = $userArr["openid"];			
			//echo $openid;
			
			// Step3：通过refresh_token刷新access_token，非必要，暂时略过
			//$refresh_token = $user->refresh_token;		
				
			// Step4：使用Access Token、OpenID来获取用户的昵称,限$oauthType = 2
			if ($oauthType == 2) {								
				$access_token = $userArr["access_token"];				
				$url = "https://api.weixin.qq.com/sns/userinfo?access_token={$access_token}&openid={$openid}&lang=zh_CN";
				$response = file_get_contents ( $url );
				if (strpos ( $response, "callback" ) !== false) {
					$lpos = strpos ( $response, "(" );
					$rpos = strrpos ( $response, ")" );
					$response = substr ( $response, $lpos + 1, $rpos - $lpos - 1 );
					$msg = json_decode ( $response );
					if (isset ( $msg->errcode )) {
						echo "<h3>error:</h3>" . $msg->errcode;
						echo "<h3>msg  :</h3>" . $msg->errmsg;
						exit ();
					}
				}
				$userArr = json_decode ( $response,true );
				
			}
			$_SESSION['userArr']=$userArr;
			return true;
		} 
	}
	
	/**
	 *
	 * @param
	 *        	type: text 文本类型, news 图文类型
	 * @param
	 *        	value_arr array(内容),array(ID)
	 * @param
	 *        	o_arr array(array(标题,介绍,图片,超链接),...小于10条),array(条数,ID)
	 */
	private function make_xml($type, $value_arr, $o_arr = array(0)) {
		// =================xml header============
		$con = "<xml>  
                    <ToUserName><![CDATA[{$this->fromUsername}]]></ToUserName>  
                    <FromUserName><![CDATA[{$this->toUsername}]]></FromUserName>  
                    <CreateTime>{$this->times}</CreateTime>  
                    <MsgType><![CDATA[{$type}]]></MsgType>";
		
		// =================type content============
		switch ($type) {
			
			case "text" :
				$con .= "<Content><![CDATA[{$value_arr[0]}]]></Content>";
				break;
			
			case "news" :
				$con .= "<ArticleCount>{$o_arr[0]}</ArticleCount>  
                     <Articles>";
				foreach ( $value_arr as $id => $v ) {
					if ($id >= $o_arr [0])
						break;
					else
						null; // 判断数组数不超过设置数
					$con .= "<item>  
                         <Title><![CDATA[{$v[0]}]]></Title>   
                         <Description><![CDATA[{$v[1]}]]></Description>  
                         <PicUrl><![CDATA[{$v[2]}]]></PicUrl>  
                         <Url><![CDATA[{$v[3]}]]></Url>  
                         </item>";
				}
				$con .= "</Articles>  
                     <FuncFlag>{$o_arr[1]}</FuncFlag>";
				break;
		} // end switch
		  
		// =================end return============
		$con .= "</xml>";
		
		return $con;
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
	
	/**
	 * 下载图片
	 *
	 * @param
	 *        	$url
	 * @return Array
	 */
	public function downloadImage($url) {
		$ch = curl_init ( $url );
		curl_setopt ( $ch, CURLOPT_HEADER, 0 );
		curl_setopt ( $ch, CURLOPT_NOBODY, 0 ); // 只取body头
		curl_setopt ( $ch, CURLOPT_SSL_VERIFYPEER, FALSE );
		curl_setopt ( $ch, CURLOPT_SSL_VERIFYHOST, FALSE );
		curl_setopt ( $ch, CURLOPT_RETURNTRANSFER, 1 );
		$package = curl_exec ( $ch );
		$httpinfo = curl_getinfo ( $ch );
		curl_close ( $ch );
		return array_merge ( array (
				'body' => $package 
		), array (
				'header' => $httpinfo 
		) );
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