<?php

class UserOrderDetailModel extends Model
{
	public $prefix = MP;
	public $table  = 'user_order_detail';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|id|oid|uid|pid|voucher_id|bar_code|pin|status|refund_percent|refund_reason|qrcode_name|expiration_time|transaction_id|create_time";

	/**
	 * 新增
	 * 
	 */
	public function addItem($postArr,$upFileArr=""){
		// if($upFileArr){
		// 	$postArr["img_name"] = date("Ym")."/".$upFileArr[0]["saveName"];
		// }
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
			$productModel = Factory::loadModel("Product");
			foreach ($itemList as $key=>$val){
				$productArr = $productModel->getOneItem("Id='{$val["pid"]}'");
				$itemList[$key]["productArr"] = $productArr;

				$itemList[$key]["smartExpirationTime"] = date("Y-m-d",strtotime($val["expiration_time"]));
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
			//产品详情
			$productModel = Factory::loadModel("Product");
			$productArr = $productModel->getOneItem("Id='{$itemArr["pid"]}'");
			$itemArr["productArr"] = $productArr;
			//父类产品资料
			if ($productArr["ParentDealId"] != 0) {
				$parentProductArr = $productModel->getOneItem("Id='{$productArr["ParentDealId"]}'");
			}else{//这个重复数据仅仅为了前端方便
				$parentProductArr = $productArr;
			}
			$itemArr["parentProductArr"] = $parentProductArr;
			//总订单资料
			$orderModel = Factory::loadModel("UserOrder");
			$orderArr = $orderModel->getOneItem("id='{$itemArr["oid"]}'");
			$itemArr["orderArr"] = $orderArr;

			$itemArr["QRcodeUrl"] = empty($itemArr["qrcode_name"]) ? "" : "https://".MY_VAN_DOMAIN."/images/upload/qrcode/".$itemArr["qrcode_name"];
			$itemArr["smartExpirationTime"] = date("Y-m-d",strtotime($itemArr["expiration_time"]));
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