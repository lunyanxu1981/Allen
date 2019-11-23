<?php

class UserOrderGroupModel extends Model
{
	public $prefix = MP;
	public $table  = 'user_order_group';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|id|gid|uid|order_no|pid|total_price|username|phone|email|group_status|pay_status|wx_result_code|wx_transaction_id|wx_form_id|out_refund_no|wx_refund_id|refund_fee|refund_time|qrcode|message|is_del|delete_time|pay_time|create_time";

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
			
			foreach ($itemList as $key=>$val){
				//订单超过3小时未付款自动关闭
				$stayTime = time()-strtotime($val["create_time"]);
				if ($val["pay_status"]==0 && $stayTime>3*60*60 )  {
					$this->updateOne("pay_status=5","id='{$val["id"]}'");
					$itemList[$key]["pay_status"] = 5;
				}
				//产品详情
				$productModel = Factory::loadModel("ProductGroup");
				$productArr = $productModel->getOneItem("id='{$val["pid"]}'");
				$itemList[$key]["productArr"] = $productArr;
				//用户详情
				$userModel = Factory::loadModel("User");
				$userArr = $userModel->getOneItem("id='{$val["uid"]}'");
				$itemList[$key]["userArr"] = $userArr;
				
				$itemList[$key]["httpQrcode"] = empty($val["qrcode"]) ? "" : "https://".MY_VAN_DOMAIN."/images/upload/miniqrcode/".$val["qrcode"];
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
			$productModel = Factory::loadModel("ProductGroup");
			$productArr = $productModel->getOneItem("id='{$itemArr["pid"]}'");
			$itemArr["productArr"] = $productArr;
			//用户详情
			$userModel = Factory::loadModel("User");
			$userArr = $userModel->getOneItem("id='{$itemArr["uid"]}'");
			$itemArr["userArr"] = $userArr;

			$itemArr["httpQrcode"] = empty($itemArr["qrcode"]) ? "" : "https://".MY_VAN_DOMAIN."/images/upload/miniqrcode/".$itemArr["qrcode"];
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
		$nowTime = date("Y-m-d H:i:s");
		$this->updateOne("is_del='1',delete_time='$nowTime'","id='$itemId'");
		// $this->delId($itemId);
		return true;
	}


}
?>