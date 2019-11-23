<?php

class ProductGroupModel extends Model
{
	public $prefix = MP;
	public $table  = 'product_group';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|id|tzoo_pid|type|category|title|from_time|to_time|num|location_name|price|value|lead|why_we_love|valid_date|what_is_included|fine_print|share_img|win_percent|status|rank|is_del|delete_time|update_time|create_time";

	/**
	 * 新增
	 * 
	 */
	public function addItem($postArr,$upFileArr=""){
		if($upFileArr){
			$postArr["share_img"] = date("Ym")."/".$upFileArr[0]["saveName"];
		}
		$postArr["create_time"] = date("Y-m-d H:i:s");
		$itemId = $this->addSel($postArr,$this->fieldStr);
		return $itemId;
	}
	//小程序内容特殊处理
	public function doContentSpe($str){
		$str = htmlspecialchars_decode($str,ENT_QUOTES);
		$str = str_replace(array("&nbsp;","="), array(" "," "), $str);
		$str = strip_tags(br2nl($str));
		return $str;
	}
	/**
	 * 取列表
	 * 
	 */
	public function getItemList($whereArr,$limit,$orderBy){
		$itemList = $this->getList($whereArr,$limit,$orderBy);
		if (!empty($itemList)){
			
			foreach ($itemList as $key=>$val){
				$pid = ($val["type"]=="1") ? $val["tzoo_pid"] : $val["id"];
				//酒店信息
				$storeModel = Factory::loadModel("Store");
				$storeArr = $storeModel->getOneItem("pid='$pid'");
				$itemList[$key]["storeArr"] = $storeArr;
				//图片列表
				$imgModel = Factory::loadModel("ProductImg");
				$imgList = $imgModel->getItemList("pid='$pid'","","rank ASC");
				$itemList[$key]["imgList"] = $imgList;
				//特殊字过滤
				$itemList[$key]["title"] = $this->doContentSpe($val["title"]);
				$itemList[$key]["lead"] = $this->doContentSpe($val["lead"]);
				$itemList[$key]["why_we_love"] = $this->doContentSpe($val["why_we_love"]);
				$itemList[$key]["what_is_included"] = $this->doContentSpe($val["what_is_included"]);
				$itemList[$key]["fine_print"] = $this->doContentSpe($val["fine_print"]);

				$itemList[$key]["httpShareImg"] = empty($val["share_img"]) ? "" : "https://".MY_VAN_DOMAIN."/images/upload/productGroup/".$val["share_img"];
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
			$pid = ($itemArr["type"]=="1") ? $itemArr["tzoo_pid"] : $itemArr["id"];
			//酒店信息
			$storeModel = Factory::loadModel("Store");
			$storeArr = $storeModel->getOneItem("pid='$pid'");
			$itemArr["storeArr"] = $storeArr;
			//图片列表
			$imgModel = Factory::loadModel("ProductImg");
			$imgList = $imgModel->getItemList("pid='$pid'","","rank ASC");
			$itemArr["imgList"] = $imgList;
			//特殊字过滤
			$itemArr["title"] = $this->doContentSpe($itemArr["title"]);
			$itemArr["lead"] = $this->doContentSpe($itemArr["lead"]);
			$itemArr["why_we_love"] = $this->doContentSpe($itemArr["why_we_love"]);
			$itemArr["what_is_included"] = $this->doContentSpe($itemArr["what_is_included"]);
			$itemArr["fine_print"] = $this->doContentSpe($itemArr["fine_print"]);

			$itemArr["httpShareImg"] = empty($itemArr["share_img"]) ? "" : "https://".MY_VAN_DOMAIN."/images/upload/productGroup/".$itemArr["share_img"];
		}
		return $itemArr;
	}
	/**
	 * 更新
	 * 
	 */
	public function updateItem($postArr,$whereArr,$upFileArr=""){
		if(!empty($upFileArr)){
			$postArr["share_img"] = date("Ym")."/".$upFileArr[0]["saveName"];
		}
		$affected = $this->updateSel($postArr,$this->fieldStr,$whereArr);
		return $affected;
	}
	/**
	 * 删除
	 * 
	 */
	public function delItem($itemId){
		$nowTime = date("Y-m-d H:i:s");
		$this->updateOne("is_del='1',delete_time='$nowTime'","id='$itemId'");
		// $this->delId($itemId);
		return true;
	}


}
?>