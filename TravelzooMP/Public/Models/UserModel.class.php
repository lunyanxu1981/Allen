<?php

class UserModel extends Model
{
	public $prefix = MP;
	public $table  = 'user';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|id|openid|unionid|session_key|phone|user_pwd|realname|nickname|sex|birthday|user_province|user_city|user_area|img_name|idcard|idcard_img|idcard_check|check_time|country|province|city|headimgurl|subscribe|subscribe_time|isbind|bind_source|bind_time|point|level_point|level|create_time|update_time";

	/**
	 * 新增
	 * 
	 */
	public function addItem($postArr,$upFileArr=""){
		if($upFileArr){
			$postArr["img_name"] = date("Ym")."/".$upFileArr[0]["saveName"];
		}
		$postArr["create_time"] = date("Y-m-d H:i:s");
		$itemId = $this->addSel($postArr,$this->fieldStr);
		return $itemId;
	}
	/**
	 * 取列表
	 * 
	 */
	public function getItemList($whereArr,$limit,$orderBy){
		$itemList = $this->getList($whereArr,$limit,$orderBy);
		if (!empty($itemList)){
			// $imgObj = Factory::loadModel("CommentImage");
			foreach ($itemList as $key=>$val){
				// $imgArr = $imgObj->getOneItem("cid='{$val["id"]}'");
				// $itemList[$key]["httpPostImg"] = empty($val["img_name"]) ? "" : "http://".MY_VAN_DOMAIN."/images/upload/user/".$val["img_name"];
				// $itemList[$key]["httpIdcardImg"] = empty($val["idcard_img"]) ? "" : "http://".MY_VAN_DOMAIN."/images/upload/user/".$val["idcard_img"];
			}
		}
		return $itemList;
	}
	/**
	 * 取单个值
	 * 
	 */
	public function getOneItem($whereArr){
		$itemArr = $this->getOne($whereArr);
		if (!empty($itemArr)) {
			// $itemArr["httpPostImg"] = empty($itemArr["img_name"]) ? "" : "http://".MY_VAN_DOMAIN."/images/upload/user/".$itemArr["img_name"];
			// $itemArr["httpIdcardImg"] = empty($itemArr["idcard_img"]) ? "" : "http://".MY_VAN_DOMAIN."/images/upload/user/".$itemArr["idcard_img"];
			// $itemArr["birthdayYear"] = date("Y",strtotime($itemArr["birthday"]));
			// $itemArr["birthdayMonth"] = date("m",strtotime($itemArr["birthday"]));
			// $itemArr["birthdayDay"] = date("d",strtotime($itemArr["birthday"]));
			// $itemArr["nicknameMark"] = empty($itemArr["nickname"]) ? substr_replace($itemArr["phone"],"****",3,4) : $itemArr["nickname"];
		}
		return $itemArr;
	}
	/**
	 * 更新
	 * 
	 */
	public function updateItem($postArr,$whereArr,$upFileArr=""){
		if(!empty($upFileArr)){
			$postArr["img_name"] = date("Ym")."/".$upFileArr[0]["saveName"];
		}
		$affected = $this->updateSel($postArr,$this->fieldStr,$whereArr);
		return $affected;
	}
	/**
	 * 删除
	 * 
	 */
	public function delItem($itemId){
		// $nowTime = date("Y-m-d H:i:s");
		// $this->updateOne("is_del='1',delete_time='$nowTime'","id='$itemId'");
		$this->delId($itemId);
		return true;
	}


}
?>