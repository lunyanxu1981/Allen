<?php

class ProductImgModel extends Model
{
	public $prefix = MP;
	public $table  = 'product_img';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|id|pid|img_name|rank";

	/**
	 * 新增
	 * 
	 */
	public function addItem($postArr,$upFileArr=""){
		if($upFileArr){
			$postArr["img_name"] = date("Ym")."/".$upFileArr[0]["saveName"];
		}
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
				$itemList[$key]["img_name"] = strpos($val["img_name"],"https://")!==false ? $val["img_name"] : "https://".MY_VAN_DOMAIN."/images/upload/product/".$val["img_name"];
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
			$itemArr["img_name"] = strpos($itemArr["img_name"],"https://")!==false ? $itemArr["img_name"] : "https://".MY_VAN_DOMAIN."/images/upload/product/".$itemArr["img_name"];
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