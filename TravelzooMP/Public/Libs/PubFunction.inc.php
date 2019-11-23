<?php
//获取验证加密串
function get_auth_code($str) {
	$str = substr(md5(md5($str.MY_SAFE)),0,8);	
	return $str;	 
}
//过滤html中的危险代码
function do_filter_html($str){
	$fromArr = array("'<link.*?>'si","'<script[^>]*?>.*?</script>'si","'<iframe[^>]*?>.*?</iframe>'si","/on[^(error)][a-zA-Z]*=|ON[^(ERROR)][a-zA-Z]*=/");
	$toArr = array("","","","no=");
	$str = preg_replace($fromArr,$toArr,$str);
	return $str;
}
//敏感词过滤
function drop_illegal_char($str){
	//在做过滤时,应将原有字符做替换,而不是直接删掉,因为如果直接删掉,有些地方的返回值就有可能为空
	return $str;
}
//取用户资料
function get_user_info($uid) {
	if (empty($uid)) return false;
	//$userArr = MC::get("user_".$uid);
	if(empty($userArr)){
		$userObjR = Factory::loadModel("User");
		$userArr = $userObjR->getById($uid);
		if (!empty($userArr)){
			$imgDir = get_user_img_dir($uid);
			if(!empty($userArr["img_name"])){
				$imageInfo = @getimagesize("http://".MY_VAN_DOMAIN."/images/upload/user/$imgDir/".$userArr["img_name"]);
				list($orWidth, $orHeight) = $imageInfo;
				$userArr["imgWidth"] = $orWidth;
				$userArr["imgHeight"] = $orHeight;
				$imgName = substr($userArr["img_name"],0,strrpos($userArr["img_name"],"."));
				$tailName = substr($userArr["img_name"],strrpos($userArr["img_name"],"."));
				$userArr["httpPostImg"] = "http://".MY_VAN_DOMAIN."/images/upload/user/$imgDir/".$imgName."-800".$tailName;
				// $userArr["httpPostImg"] = "http://".MY_VAN_DOMAIN."/images/upload/user/$imgDir/".$userArr["img_name"];
			}else{
				$userArr["httpPostImg"] = "http://".MY_VAN_DOMAIN."/images/none.gif";
			}
			//if(!empty($userArr["idcard_img"]))
			//	$userArr["httpUserIdcard"] = "http://".MY_VAN_DOMAIN."/images/upload/user/idcard/".$userArr["idcard_img"];
			if($userArr["birthday"] != "0000-00-00"){
				list($year,$month,$day) = explode("-",$userArr["birthday"]);
				$userArr["constellation"] = get_constellation($month,$day);
			}
			//MC::set("user_".$uid,$userArr);
		}
	}
	return $userArr;
}
//后台取用户资料
function get_user_info_bk($uid) {
	if (empty($uid)) return false;
	//$userArr = MC::get("user_".$uid);
	if(empty($userArr)){
		$userObjR = Factory::loadModel("BkUser");
		$userArr = $userObjR->getOneItem($uid);
		if (!empty($userArr)){
			//MC::set("user_".$uid,$userArr);
		}
	}
	return $userArr;
}
//生成二维码图片
function setUserQrcode($uid){
	$chl = "http://".MY_APP_DOMAIN."/user_detail_fid_$uid.html";
	$qrcodeUrl = "http://chart.apis.google.com/chart?chs=186x186&cht=qr&chld=L|0&chl=".$chl;
	//建文件夹
	$imgDir = get_user_img_dir($uid);
	$photoDir = MY_PUBLIC_ROOT."images/upload/user/".$imgDir."/";
	File::mkDir($photoDir, 0777);
	
	$putStr = file_get_contents($qrcodeUrl);
	$qrcodeName = date("YmdHis").md5(uniqid(rand())).".png";
	file_put_contents($photoDir.$qrcodeName,$putStr);
	return $qrcodeName;
}
//取得用户头像
function get_user_img($uid,$size="48") {
	$userArr = get_user_info($uid);
	$updateStr = strtotime($userArr["update_time"]);
	if(in_array($userArr["level"],array("10","14"))){//球队用户
		$teamArr = get_team_info($userArr["team_id"]);
		$imgStr = "upload/team/48_48/{$teamArr["domain"]}.gif";
	}elseif(in_array($userArr["level"],array("11","12","13"))){//球员用户
		$playerArr = get_player_info($userArr["player_id"]);
		$imgStr = "upload/player/48_48/{$playerArr["domain"]}.jpg";
	}else{//真正用户
		if($userArr["avatar_update"] == "1"){
			$imgStr = ($size == "48") ? "user/avatar/{$userArr["img_name"]}_48.jpg?$updateStr" : "user/avatar/{$userArr["img_name"]}_220.jpg?$updateStr";
		}else{
			$imgStr = ($size == "48") ? "user/{$userArr["sex"]}/{$userArr["img_name"]}.jpg" : "user/{$size}/{$userArr["sex"]}/{$userArr["img_name"]}.jpg";
		}
	}
	return "http://".MY_VAN_DOMAIN."/images/".$imgStr;
}
//cookie 解密(取不到cookie值时用到)
function cookie_decode($str){
	$replace = array('=','+','/');
	$search = array('_','-','|');
	$str = str_replace($search,$replace,$str);
	return base64_decode($str);
}
// 根据用户id调取用户昵称
function get_nickname_by_id($uid) {
	$user = get_user_info($uid);
	return $user['nickname'];
}
//根据TAG名取TAG的id，先到内存中取，如果没有再到DB里查，DB如果没有就写入
function get_tag_id($tagName,$writeDb="1"){
	//$tagId = MC::get("tag_name_".$tagName);
	if(empty($tagId)){
		$tagObjR = Factory::loadModel("Tag");
		$tagArr = $tagObjR->getOne("tag='$tagName'");
		if(empty($tagArr)){
			if($writeDb){
				$master = Factory::loadDB("master");
				$tagObjW = Factory::loadModel("Tag",$master);
				$tagId = $tagObjW->add(array("tag"=>$tagName));
				//MC::set("tag_name_".$tagName,$tagId,0,0);
			}
		}else{
			$tagId = $tagArr["id"];
			//MC::set("tag_name_".$tagName,$tagId,0,0);
		}
	}
	return $tagId;
}
//根据TAG id取TAG名
function get_tag_name($tagId){
	//$tagName = MC::get("tag_id_".$tagId);
	if(empty($tagName)){
		$tagObjR = Factory::loadModel("Tag");
		$tagArr = $tagObjR->getOne("id='$tagId'");
		if(!empty($tagArr)){
			$tagName = $tagArr["tag"];
			//MC::set("tag_id_".$tagId,$tagName,0,0);
		}
	}
	return $tagName;
}
//弹出消息并跳转，消息可为空
function js_msg_go($msg,$loc){
	echo "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />";
	echo "<script>".(!empty($msg) ? "alert('$msg');" : "")."window.location.href='$loc';</script>";
}
//弹出消息并返回，消息可为空
function js_msg_back($msg){
	echo "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />";
	echo "<script>".(!empty($msg) ? "alert('$msg');" : "")."history.go(-1);</script>";
}
//执行任意js
function js_show($msg){
	echo "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />";
	echo "<script>$msg</script>";
}
//弹出消息并关闭窗口，消息可为空
function js_close($msg){
	echo "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />";
	echo "<script>".
	" alert('$msg');".
	" if (window.name=='') window.name=\"qiumi\";".
	" window.open(\"\",window.name);".
	" window.opener = null;".
	" window.close()".
	"</script>";
}
//转址
function redirect($data){
	header("Location: $data");
	exit();
}
// qmi短网址
function shortUrl($str) {
	preg_match_all('/(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)[-A-Z0-9+&@#\/%=~_|$?!:,.]*[A-Z0-9+&@#\/%=~_|$]/i',$str, $out);
	if (!empty($out)) {
		foreach ($out as $val) {
			$url = $val[0];
			$rep = file_get_contents("http://qmi.cc/yourls-api.php?signature=861cf76201&action=shorturl&url=".$url."&format=simple");
			//$link = '<a href="'.$rep.'" target="_blank">'.$rep.'</a>';;
			$str = str_replace($url, $rep , $str);
		}
		return $str;
	}
	return false;
}
// url自动加链接
function autoUrl($str) {
	// 判断a标签当中是否有sign="tiao"
	$str = htmlspecialchars_decode($str,ENT_QUOTES);
	$isA = stripos($str, '</a>');
	preg_match('/<a(.*?)sign=[\'|"](.*?)[\'|"]/i', $str, $con);
	$str = htmlspecialchars($str, ENT_QUOTES);
	if(strtolower($con[2]) != 'tiao' && $isA==false){
		preg_match_all('/(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)[-A-Z0-9+&@#\/%=~_|$?!:,.]*[A-Z0-9+&@#\/%=~_|$]/i',$str, $out);
		if (!empty($out)) {
			foreach ($out as $val) {
				$url = $val[0];
				$link = '<a href="'.$url.'" target="_blank" class="qmi_link">'.$url.'</a>';;
				$str = str_replace($url, $link , $str);
			}
		}
	}
	if(strtolower($con[2]) == 'tiao'){
		$str = htmlspecialchars_decode($str,ENT_QUOTES);
	}
	$homeUrl = "http://www.".MY_ROOT_DOMAIN;
	$str = preg_replace("/#(.*?)#/is","<a href=\"".$homeUrl."/st/$1\" target=\"_blank\">#$1#</a>",$str);
	$str = str_replace(unserialize(FACEICON_FROM),unserialize(FACEICON_TO),$str);
	return $str;
}


//加密字符串
function encode_str($str){
	return str_replace(array("=","+","/"),array("_","-","|"),base64_encode($str));
}
//解密字符串
function decode_str($str){
	return base64_decode(str_replace(array("_","-","|"),array("=","+","/"),$str));
}
//gbk编码转为utf8
function get_utf8_char($str){
	if(!extension_loaded("iconv"))	dl("iconv.so");
	$rtn_value = "";
	preg_match_all("/[\x80-\xff]?./",$str,$matches);
	foreach($matches[0] as $value){
		$tmp_str  = @iconv("gbk","utf-8",$value);
		if($tmp_str != "")	$rtn_value .= $tmp_str;
	}
	return $rtn_value;
}
//utf8编码转为gbk
function get_gbk_char($str){
	if(!extension_loaded("iconv"))	dl("iconv.so");
	$rtn_value = "";
	$rtn_value = @iconv("utf-8","gbk",$str);
	return $rtn_value;
}
// 判断是否是联赛
function is_league($team) {
	$league = array('nba','cba','en','it','esp','de','cn','eur','uel','cou','f1','other');
	if (in_array($team, $league)) {
		return true;
	}
	return false;
}
// 根据数字输出星期几
function get_week_by_num($num) {
	switch ($num) {
		case 1:
			return  "一";
			break;
		case 2:
			return  "二";
			break;
		case 3:
			return  "三";
			break;
		case 4:
			return  "四";
			break;
		case 5:
			return  "五";
			break;
		case 6:
			return  "六";
			break;
		case 7:
			return  "日";
			break;
	}
}
//转化人性化的时间
function get_smart_time($timeStr) {
	$oriStr = strtotime($timeStr);
    $yearsdaySec=strtotime(date("Y-m-d"),time()) - (24*60*60);
	$predaySec=strtotime(date("Y-m-d"),time()) - (2*24*60*60);
    $seconds = time() - $oriStr;
	if($seconds < (20*60)){//20分钟以内
		return "刚刚";
	}elseif($seconds >= (20*60) && $seconds < (60*60)){//20分钟~1小时
		return ceil($seconds/60)."分钟之前";
	}elseif($seconds >= (60*60) && $seconds < (24*60*60)){//1~24小时
		return ceil($seconds/(60*60))."小时之前";
	}elseif($oriStr > $yearsdaySec){//昨天
		return "昨天".date("H点i分",$oriStr-24*60*60);
	}elseif($oriStr > $predaySec){//前天
		return "前天".date("H点i分",$oriStr-2*24*60*60);
	}elseif($oriStr < $predaySec && $seconds < (365*24*60*60)){//一年之内
		return date("m月d日",$oriStr);
	}elseif($seconds >= (365*24*60*60)){//一年之外
		return date("Y年m月d日",$oriStr);
	}
}
//curl方法post数据
function curl_post_data($url,$postData,$headerArr=""){
	if(!extension_loaded("curl")){
         dl("curl.so");
    }
	/* 
	$curl = curl_init($url);
	curl_setopt($curl, CURLOPT_POST, 1);
	curl_setopt($curl, CURLOPT_POSTFIELDS, $postData);
	curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($curl, CURLOPT_HTTP_VERSION,CURL_HTTP_VERSION_1_0);
	$result = curl_exec($curl);
	curl_close($curl);
	return $result;
	 */
	
	$ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPAUTH, CURLAUTH_BASIC);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'POST');
    curl_setopt($ch, CURLOPT_URL, $url);
    if(!empty($headerArr)){
	    curl_setopt($ch, CURLOPT_HTTPHEADER, $headerArr);
	}
    curl_setopt($ch, CURLOPT_POST, true);
    if (is_array($postData)) {
    		curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($postData));
    }else{
    		curl_setopt($ch, CURLOPT_POSTFIELDS,$postData);
    }
    
    curl_setopt($ch, CURLOPT_RETURNTRANSFER,true);
   
	$result = curl_exec($ch);
	curl_close($ch);
    return $result;
	
}
// curl GET
function curl_get($url,$headerArr="", $gzip=false){
	$curl = curl_init($url);
	curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($curl, CURLOPT_CONNECTTIMEOUT, 10);
	if(!empty($headerArr)){
	    curl_setopt($curl, CURLOPT_HTTPHEADER, $headerArr);
	}
	if($gzip) curl_setopt($curl, CURLOPT_ENCODING, "gzip");
	$content = curl_exec($curl);
	curl_close($curl);
	return $content;
}
//用file_get_content来发送post数据
function fgc_post_data($url,$postData){
	if (is_array($postData)) {
    	$content = http_build_query($postData);
    }else{
    	$content = $postData;
    }
	$length = strlen($content);
	$options = array(
		'http' => array(
			'method' => 'POST',
			'header' => "Content-type: application/x-www-form-urlencoded\r\n"."Content-length: $length \r\n",
			'content' => $content
		)
	);
	$result = file_get_contents($url, false, stream_context_create($options));
	return $result;
}
/**
 * url 为服务的url地址
 * query 为请求串
 */
function sock_post($url,$query){
	$info=parse_url($url);
	$fp=fsockopen($info["host"],80,$errno,$errstr,30);
	$head="POST ".$info['path']." HTTP/1.0\r\n";
	$head.="Host: ".$info['host']."\r\n";
	$head.="Referer: http://".$info['host'].$info['path']."\r\n";
	$head.="Content-type: application/x-www-form-urlencoded\r\n";
	$head.="Content-Length: ".strlen(trim($query))."\r\n";
	$head.="\r\n";
	$head.=trim($query);
	$write=fputs($fp,$head);
	$header = "";
	while ($str = trim(fgets($fp,4096))) {
		$header.=$str;
	}
	$data = "";
	while (!feof($fp)) {
		$data .= fgets($fp,4096);
	}
	return $data;
}

//根据uid取活动图片存放目录(eg："375/014/14375")
function get_event_img_dir($uid){
	$str = sprintf('%07d', $uid);
	return substr($str, -3, 3)."/".substr($str, -6, 3)."/".$uid;
}
//根据uid取头像图片存放目录(eg："375/014")
function get_user_img_dir($uid){
	$str = sprintf('%07d', $uid);
	return substr($str, -3, 3)."/".substr($str, -6, 3);
}

// 侃球是否存在屏蔽词
function is_unword($content){
	$file = MC::get("talk_unword_txt");
	if(empty($file)){
		$file = file_get_contents("../Admin/Views/default/txt/pingbi.txt");
		MC::set("talk_unword_txt", $file, 0, 0);
	}
	$str = explode(',', $file);
	$total = 0;
	foreach($str as $key=>$val){
		$num = strstr($content, strtolower($val));
		if(!empty($num)){
			$total = 1;
			break;
		}
	}
	return $total;
}

//判断是否VIP
function is_vip($level){
	$vip=array("1","5","6","9");
	if (in_array($level, $vip)){
		return 1;
	}else {
		return 0;
	}
}
function replace_unword($content){
	$file = MC::get("talk_unword_txt");
	if(empty($file)){
		$file = file_get_contents("../Admin/Views/default/txt/pingbi.txt");
		MC::set("talk_unword_txt", $file, 0, 0);
	}
	$str = explode(',', $file);
	foreach ($str as $key=>$val){
		$rArr[$key]="***";
	}
	$content = str_ireplace($str,$rArr,$content);
	return $content;
}

// 计算中英文混合字符串的长度
function cc_str_length($str){
	$ccLen = (strlen($str) + mb_strlen($str,'UTF8'))/2;
	return $ccLen;
}
//取品牌列表
function get_brand_list(){
	$whereArr = "status=0";
	$itemObjR = Factory::loadModel("Brand");
	$itemList = $itemObjR->getItemList($whereArr,"","rank ASC,id DESC");
	return $itemList;
}
//取尺码的子分类列表
function get_one_size_list($pid){
	$whereArr = "pid='$pid'";
	// if($pid == 0){
		// $whereArr .= " AND id<100";
	// }
	$itemObjR = Factory::loadModel("Size");
	$itemList = $itemObjR->getItemList($whereArr,"","id ASC");
	return $itemList;
}
//取产品各类的子分类列表
function get_one_type_list($pid){
	$whereArr = "pid='$pid' AND status=0";
	// if($pid == 0){
		// $whereArr .= " AND id<100";
	// }
	$itemObjR = Factory::loadModel("ProductTree");
	$itemList = $itemObjR->getItemList($whereArr,"","id ASC");
	return $itemList;
}
//根据某一分类id取其子分类的id str
function get_one_type_str($pid){
	$itemList = getOneTypeList($pid);
	$idStr = "";
	if( !empty($itemList) ){
		foreach ($itemList as $key=>$val){
			if( !empty($idStr) ) $idStr .= ",";
			$idStr .= $val["id"];
		}
	}
	return $idStr;
}
//根据分类id查名称
function get_type_name($id){
	$itemObjR = Factory::loadModel("ProductTree");
	$itemArr = $itemObjR->getOneItem($id);
	return $itemArr["title"];
}
//根据月日得出星座
// 魔羯座(12/22 – 1/19)、水瓶座(1/20 – 2/18)、双鱼座(2/19 – 3/20)、白羊座(3/21 – 4/20)、
// 金牛座(4/21 – 5/20)、双子座(5/21 – 6/21)、巨蟹座(6/22 – 7/22)、狮子座(7/23 – 8/22)、
// 处女座(8/23 – 9/22)、天秤座(9/23 – 10/22)、天蝎座(10/23 – 11/21)、射手座(11/22 – 12/21)
function get_constellation($month, $day){
    $signs = array(
            array('20'=>'水瓶座'), array('19'=>'双鱼座'),array('21'=>'白羊座'), array('20'=>'金牛座'),
            array('21'=>'双子座'), array('22'=>'巨蟹座'),array('23'=>'狮子座'), array('23'=>'处女座'),
            array('23'=>'天秤座'), array('24'=>'天蝎座'),array('22'=>'射手座'), array('22'=>'摩羯座')
    );
    $key = (int)$month - 1;
    list($startSign, $signName) = each($signs[$key]);
    if( $day < $startSign ){
        $key = $month - 2 < 0 ? $month = 11 : $month -= 2;
        list($startSign, $signName) = each($signs[$key]);
    }
    return $signName;
}
//表情文字图片转换
function get_face_img($str){
	$fromArr = array(
	'[呵呵]','[嘻嘻]','[哈哈]','[可爱]','[可怜]','[挖鼻屎]','[吃惊]','[害羞]','[挤眼]','[闭嘴]','[鄙视]','[爱你]','[泪]','[偷笑]','[亲亲]','[生病]','[太开心]','[懒得理你]','[右哼哼]','[左哼哼]','[嘘]','[衰]','[委屈]','[吐]','[打哈欠]','[抱抱]','[怒]','[馋嘴]','[疑问]','[拜拜]','[思考]','[汗]','[困]','[钱]','[睡觉]','[失望]','[酷]','[花心]','[哼]','[鼓掌]','[悲伤]','[晕]','[抓狂]','[黑线]','[阴险]','[怒骂]','[心]','[伤心]','[猪头]','[ok]','[耶]','[good]','[不要]','[来]','[赞]','[弱]','[蜡烛]','[钟]','[话筒]','[蛋糕]'
	);
	$u = "<img src='http://".MY_VAN_DOMAIN."/images/face/q/";
	$toArr = array(
	$u."1.gif'>",$u."2.gif'>",$u."3.gif'>",$u."4.gif'>",$u."5.gif'>",$u."6.gif'>",$u."7.gif'>",$u."8.gif'>",$u."9.gif'>",$u."10.gif'>",
	$u."11.gif'>",$u."12.gif'>",$u."13.gif'>",$u."14.gif'>",$u."15.gif'>",$u."16.gif'>",$u."17.gif'>",$u."18.gif'>",$u."19.gif'>",$u."20.gif'>",
	$u."21.gif'>",$u."22.gif'>",$u."23.gif'>",$u."24.gif'>",$u."25.gif'>",$u."26.gif'>",$u."27.gif'>",$u."28.gif'>",$u."29.gif'>",$u."30.gif'>",
	$u."31.gif'>",$u."32.gif'>",$u."33.gif'>",$u."34.gif'>",$u."35.gif'>",$u."36.gif'>",$u."37.gif'>",$u."38.gif'>",$u."39.gif'>",$u."40.gif'>",
	$u."41.gif'>",$u."42.gif'>",$u."43.gif'>",$u."44.gif'>",$u."45.gif'>",$u."46.gif'>",$u."47.gif'>",$u."48.gif'>",$u."49.gif'>",$u."50.gif'>",
	$u."51.gif'>",$u."52.gif'>",$u."53.gif'>",$u."54.gif'>",$u."55.gif'>",$u."56.gif'>",$u."57.gif'>",$u."58.gif'>",$u."59.gif'>",$u."60.gif'>",
	);
	$str = str_replace($fromArr,$toArr,$str);
	return $str;
}

function getRand($len=6){
	$storage = range(0, 9);
	$rand = array();
	for($i=0; $i<$len; $i++)
	{
	$rand[] = array_rand($storage, 1);
	}
	return implode('', $rand);
}

function getSn($uid){
	return date('YmdHis') . $uid . getRand(4);
}

//转换金额格式
function account_format($account,$decimal = 2){
	if ($account == '0.00')
		$account = 0;
	else
		$account = number_format( $account ,$decimal );
	return $account;
}

function saveFile($fileName,$fileContent){
	$localFile = fopen($fileName, 'w');
	if (false !== $localFile) {
		if (false !== fwrite($localFile, $fileContent)) {
			fclose($localFile);
		};
	}
}
function http_get($url) {
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
 * 
 * @param unknown $imgVaule
 * @param unknown $uploadDir
 * @param number $thumbW
 * @param number $thumbH
 * @return boolean|string
 */
function imageUploadBase64($imgVaule,$uploadDir,$thumbW=0,$thumbH=0){
	set_time_limit(30);
	preg_match('/^data:\s*image\/(\w+);base64,([\S|\s]+)/',$imgVaule,$stream);
	$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
	$type = $stream[1];
	if (!in_array($type, $fileFormat)) {
		return false;
	}

	$img_stream = base64_decode($stream[2]);

	File::mkDir($uploadDir,0777);
	$fileName = date("Ymd").getRand(6).".".$type;
	$fileFullPath = $uploadDir.$fileName;
	saveFile($fileFullPath, $img_stream);
	
	if ($thumbW!=0||$thumbH!=0) {
		//$imgInfo = getimagesize($fileFullPath);
		//压缩图片
		Image::thumbImg($fileFullPath,$fileFullPath,$thumbW,$thumbH);
	}

	return  date("Ymd")."/".$fileName;
}

/*移动端判断*/
function isMobile()
{
	// 如果有HTTP_X_WAP_PROFILE则一定是移动设备
	if (isset ($_SERVER['HTTP_X_WAP_PROFILE']))
	{
		return true;
	}
	// 如果via信息含有wap则一定是移动设备,部分服务商会屏蔽该信息
	if (isset ($_SERVER['HTTP_VIA']))
	{
		// 找不到为flase,否则为true
		return stristr($_SERVER['HTTP_VIA'], "wap") ? true : false;
	}
	// 脑残法，判断手机发送的客户端标志,兼容性有待提高
	if (isset ($_SERVER['HTTP_USER_AGENT']))
	{
		$clientkeywords = array ('nokia',
				'sony',
				'ericsson',
				'mot',
				'samsung',
				'htc',
				'sgh',
				'lg',
				'sharp',
				'sie-',
				'philips',
				'panasonic',
				'alcatel',
				'lenovo',
				'iphone',
				'ipod',
				'blackberry',
				'meizu',
				'android',
				'netfront',
				'symbian',
				'ucweb',
				'windowsce',
				'palm',
				'operamini',
				'operamobi',
				'openwave',
				'nexusone',
				'cldc',
				'midp',
				'wap',
				'mobile'
		);
		// 从HTTP_USER_AGENT中查找手机浏览器的关键字
		if (preg_match("/(" . implode('|', $clientkeywords) . ")/i", strtolower($_SERVER['HTTP_USER_AGENT'])))
		{
			return true;
		}
	}
	// 协议法，因为有可能不准确，放到最后判断
	if (isset ($_SERVER['HTTP_ACCEPT']))
	{
		// 如果只支持wml并且不支持html那一定是移动设备
		// 如果支持wml和html但是wml在html之前则是移动设备
		if ((strpos($_SERVER['HTTP_ACCEPT'], 'vnd.wap.wml') !== false) && (strpos($_SERVER['HTTP_ACCEPT'], 'text/html') === false || (strpos($_SERVER['HTTP_ACCEPT'], 'vnd.wap.wml') < strpos($_SERVER['HTTP_ACCEPT'], 'text/html'))))
		{
			return true;
		}
	}
	return false;
}

//中文字取姓和名
function split_cn_name($fullname){  
	$hyphenated = array('欧阳','太史','端木','上官','司马','东方','独孤','南宫','万俟','闻人','夏侯','诸葛','尉迟','公羊','赫连','澹台','皇甫',  
	'宗政','濮阳','公冶','太叔','申屠','公孙','慕容','仲孙','钟离','长孙','宇文','城池','司徒','鲜于','司空','汝嫣','闾丘','子车','亓官',  
	'司寇','巫马','公西','颛孙','壤驷','公良','漆雕','乐正','宰父','谷梁','拓跋','夹谷','轩辕','令狐','段干','百里','呼延','东郭','南门',  
	'羊舌','微生','公户','公玉','公仪','梁丘','公仲','公上','公门','公山','公坚','左丘','公伯','西门','公祖','第五','公乘','贯丘','公皙',  
	'南荣','东里','东宫','仲长','子书','子桑','即墨','达奚','褚师');  
	$vLength = mb_strlen($fullname, 'utf-8');  
	$lastname = '';  
	$firstname = '';//前为姓,后为名  
	if($vLength > 2){  
	    $preTwoWords = mb_substr($fullname, 0, 2, 'utf-8');//取命名的前两个字,看是否在复姓库中  
	    if(in_array($preTwoWords, $hyphenated)){  
	        $lastname = $preTwoWords;  
	        $firstname = mb_substr($fullname, 2, 10, 'utf-8');  
	    }else{  
	        $lastname = mb_substr($fullname, 0, 1, 'utf-8');  
	        $firstname = mb_substr($fullname, 1, 10, 'utf-8');  
	    }  
	}else if($vLength == 2){//全名只有两个字时,以前一个为姓,后一下为名  
	    $lastname = mb_substr($fullname ,0, 1, 'utf-8');  
	    $firstname = mb_substr($fullname, 1, 10, 'utf-8');  
	}else{  
	    $lastname = $fullname;  
	}  
	return array($lastname, $firstname);  
}

function br2nl($string){
    return preg_replace('/\<br(\s*)?\/?\>/i', "\n", $string);
}
//产生随机字符串，不长于32位
function get_nonce_str($length = 32) {
	$chars = "abcdefghijklmnopqrstuvwxyz0123456789";  
	$str ="";
	for ( $i = 0; $i < $length; $i++ )  {  
		$str .= substr($chars, mt_rand(0, strlen($chars)-1), 1);  
	} 
	return $str;
}

















?>