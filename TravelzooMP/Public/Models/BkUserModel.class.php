<?php

class BkUserModel extends Model
{
	public $prefix = MP;
	public $table  = 'bk_user';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	
	
	/**
	 * 新增
	 * 
	 */
	public function addItem($postArr,$upFileArr=""){
		if($upFileArr){
			$postArr["img_name"] = $upFileArr[0]["saveName"];
		}
		$postArr["user_pwd"] = md5($postArr["user_pwd"]);
		$itemId = $this->addSel($postArr,"|nickname|user_pwd|realname|sex|email|phone|level");
		return $itemId;
	}
	/**
	 * 取列表
	 * 
	 */
	public function getItemList($whereArr,$limit,$orderBy){
		$itemList = $this->getList($whereArr,$limit,$orderBy);
		if (!empty($itemList)){
			foreach ($itemList as $key=>$val){
				//$itemList[$key]["httpPostImg"] = "http://".MY_VAN_DOMAIN."/upload/event_reco/".$val["img_name"];
			}
		}
		return $itemList;
	}
	/**
	 * 取单个值
	 * 
	 */
	public function getOneItem($itemId){
		$itemArr = $this->getOne("id='$itemId'");
		//$itemArr["httpPostImg"] = "http://".MY_VAN_DOMAIN."/upload/event_reco/".$itemArr["img_name"];
		return $itemArr;
	}
	/**
	 * 更新
	 * 
	 */
	public function updateItem($postArr,$upFileArr=""){
		$str = "|nickname|realname|sex|email|phone|level";
		if(!empty($upFileArr)){
			$postArr["img_name"] = $upFileArr[0]["saveName"];
			$str .= "|img_name";
		}
		$this->updateSel($postArr,$str,"id='{$postArr["eid"]}'");
		return true;
	}
	/**
	 * 删除
	 * 
	 */
	public function delItem($itemId){
		//$nowTime = date("Y-m-d H:i:s");
		//$this->updateOne("is_deleted='1',deleted_time='$nowTime'","id='$itemId'");
		$this->delOne("id='$itemId'");
		return true;
	}
	
	
}
?>