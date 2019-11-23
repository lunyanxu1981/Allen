<?php

class UserSmsModel extends Model
{
	public $prefix = MP;
	public $table  = 'user_sms';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|type|phone|code|create_time";

	/**
	 * 新增
	 * 
	 */
	public function addItem($postArr,$upFileArr=""){
		// if($upFileArr){
		// 	$postArr["img_name"] = date("Ym")."/".$upFileArr[0]["saveName"];
		// }
		// $postArr["create_time"] = date("Y-m-d H:i:s");
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
			foreach ($itemList as $key=>$val){

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
			
		}
		return $itemArr;
	}
	/**
	 * 更新
	 * 
	 */
	public function updateItem($postArr,$whereArr,$upFileArr=""){
		// if(!empty($upFileArr)){
		// 	$postArr["img_name"] = date("Ym")."/".$upFileArr[0]["saveName"];
		// }
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