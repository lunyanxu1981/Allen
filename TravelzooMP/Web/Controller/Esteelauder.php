<?php
/**
 * 雅诗兰黛活动
 */
class Esteelauder extends Controller
{
	
	//首页
	public function index() {
		//$wechatObj = new WeChat();
		$data["cnzz"] = getCNZZHtml(1262082725);
		$data["myDomain"] = MY_APP_DOMAIN;		
		return View::parse('esteelauder/esteelauder_index.html',$data);
		
	}
	
	
	/**
	 * 接口名称：记录活动用户信息
	 * URL：	/esteelauder_saveInfo.html
	 * 方式：	POST
	 * 参数：	phone 手机（必填）
	 * 		name  姓名 (必填)
	 * 		city     市
	 * 返回：200:成功
	 * 		401:参数错误
	 * 		402:手机已存在
	 *      403:数据库错误
	 *      404:第三方接口错误
	 *       {"status":"200","data":"success"}
	 */
	public function saveInfo(){
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			$postArr["name"] = trim($postArr["name"]);
			if(empty($postArr["phone"]) || empty($postArr["name"]) || empty($postArr["city"])) {
				echo json_encode(array("status"=>401,"data"=>"信息不完整"));
				exit();
			}
			//获取用户ip
			$postArr['ip'] = get_client_ip();
			//查询手机记录
			$master = Factory::loadDB("master");
			$itemModel = Factory::loadModel("Esteelauder",$master);
			$result = $itemModel->getOne("`phone`='{$postArr['phone']}'");
			if (!empty($result)) {
				echo json_encode(array("status"=>402,"data"=>"该手机已参与过活动哦"));
				exit();
			}
			
			//发送信息接口
			$url = 'http://meganu.esteelauderclub.cn/interface/travelzooapply';
			$key = 'VDHfpFfcdfdW9lbMYTyrgvSacpqWgEhq';
			$mobile = $postArr["phone"];
			$city = $postArr["city"];
			$timestamp = time();
			$sign = md5("mobile=".$mobile."&city=".$city."&timestamp=".$timestamp."&key=".$key);		
			$params = array(
					'mobile' => $mobile,
					'city' => $city,
					'timestamp' => $timestamp,
					'signature' => $sign,
			);
			$paramsBody = json_encode($params);
			$data = curl_post_data($url, $paramsBody);
			$returnDate = json_decode($data,true);
			//$returnDate["code"] = 1;
			if ($returnDate["code"] == 1) {
				//记录数据
				$recordId = $itemModel->addItem($postArr);
				if ($recordId > 0) {
					echo json_encode(array("status"=>200,"data"=>"预约信息提交成功"));
					exit();
				}else {
					echo json_encode(array("status"=>403,"data"=>"网络不稳定，请稍后再试"));
					exit();
				}				
			}else if ($returnDate["code"] == -2) {
				echo json_encode(array("status"=>403,"data"=>"手机号格式错误"));
				exit();
			}else if ($returnDate["code"] == -3) {
				echo json_encode(array("status"=>403,"data"=>"该手机已参与过活动哦"));
				exit();
			}else{
				echo json_encode(array("status"=>404,"data"=>"网络不稳定，请稍后再试"));
				exit();
			}
		}
		
		
		
	}
	
	
}
?>