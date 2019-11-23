<?php

class EsteelauderModel extends Model
{
	public $prefix = '';
	public $table  = 'esteelauder';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 10;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|phone|name|city|ip";
	/**
	 * 新增
	 *
	 */
	public function addItem($postArr,$upFileArr=""){
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
			
		}
		return $itemList;
	}
	/**
	 * 更新
	 * 
	 */
	public function updateItem($postArr,$where){
		$affected = $this->updateSel($postArr, $this->fieldStr, $where);
		return true;
	}


}
?>