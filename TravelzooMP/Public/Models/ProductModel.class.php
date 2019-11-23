<?php

class ProductModel extends Model
{
	public $prefix = MP;
	public $table  = 'product';
	public $pk     = 'id';
	public $chipD  = 0;//分库 0为不分，级别10，100，1000
	public $chip   = 0;//分表 0为不分，级别1，10，100，1000
	public $chipId = 0;// 分库分表id
	private $fieldStr = "|Id|type|banner_id|ParentDealId|CountryCode|CurrencyCode|InternalStatus|Title|Url|Category|LocationName|IsPurchasable|IsSoldOut|MaximumPurchases|MaximumPurchasesPerCustomer|PublishDateUtc|PurchaseExpiryDateUtc|VoucherExpiryDateUtc|NegotiatedBy|Price|Value|ByLine|Lead|WhyWeLoveIt|When|WhatIsIncluded|FinePrint|HowToRedeem|RedeemFinePrint|RedeemTermsAndConditions|SupportEmail|SupportPhone|SortNumber|Update|UpdateTextTimeStampUTC|ChildDeals|is_recommend|rank|banner_rank|noUpdate|update_time|create_time";

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
			//取分类名
			$recommendModel = Factory::loadModel("Recommend");
			$recommendList = $recommendModel->getItemList("status=1 AND is_del=0","","id ASC");
			$recoList = array();
			foreach ($recommendList as $k=>$v){
				$recoList[$v["id"]] = $v;
			}
			
			foreach ($itemList as $key=>$val){
				//分类名
				$itemList[$key]["typeName"] = $recoList[$val["type"]]["title"];
				$itemList[$key]["bannerName"] = $recoList[$val["banner_id"]]["title"];
				//酒店信息
				$storeModel = Factory::loadModel("Store");
				$storeArr = $storeModel->getOneItem("pid='{$val["Id"]}'");
				$itemList[$key]["storeArr"] = $storeArr;
				//图片列表
				$imgModel = Factory::loadModel("ProductImg");
				$imgList = $imgModel->getItemList("pid='{$val["Id"]}'","","rank ASC");
				$itemList[$key]["imgList"] = $imgList;
				//特殊字过滤
				$itemList[$key]["Title"] = $this->doContentSpe($val["Title"]);
				$itemList[$key]["Lead"] = $this->doContentSpe($val["Lead"]);
				$itemList[$key]["WhyWeLoveIt"] = $this->doContentSpe($val["WhyWeLoveIt"]);
				$itemList[$key]["WhatIsIncluded"] = $this->doContentSpe($val["WhatIsIncluded"]);
				$itemList[$key]["FinePrint"] = $this->doContentSpe($val["FinePrint"]);
				$itemList[$key]["HowToRedeem"] = $this->doContentSpe($val["HowToRedeem"]);
				$itemList[$key]["RedeemFinePrint"] = $this->doContentSpe($val["RedeemFinePrint"]);
				$itemList[$key]["RedeemTermsAndConditions"] = $this->doContentSpe($val["RedeemTermsAndConditions"]);
				if (!empty($val["UpdateTextTimeStampUTC"])) {
					$itemList[$key]["UpdateTextTimeStampUTC"] = date("Y-m-d H:i:s",(substr($val["UpdateTextTimeStampUTC"],6,10)+8*60*60));
				}
				//如果用户收藏或转发了某产品，下次再进来时就需要判断是否过期，下线的产品，统一设置IsPurchasable=0，即售罄
				$expiryTime = date("Y-m-d H:i:s",time()+24*60*60);
				if ( $val["IsSoldOut"]=="1" || $val["InternalStatus"]!="Published" || $val["PurchaseExpiryDateUtc"]<$expiryTime ) {
					$itemList[$key]["IsPurchasable"] = "0";
				}
				
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

			$itemArr["Title"] = $this->doContentSpe($itemArr["Title"]);
			$itemArr["shortTitle"] = msubstr($itemArr["Title"],56);

			$itemArr["Lead"] = $this->doContentSpe($itemArr["Lead"]);
			$itemArr["WhyWeLoveIt"] = $this->doContentSpe($itemArr["WhyWeLoveIt"]);
			$itemArr["WhatIsIncluded"] = $this->doContentSpe($itemArr["WhatIsIncluded"]);
			$itemArr["FinePrint"] = $this->doContentSpe($itemArr["FinePrint"]);
			$itemArr["HowToRedeem"] = $this->doContentSpe($itemArr["HowToRedeem"]);
			$itemArr["RedeemFinePrint"] = $this->doContentSpe($itemArr["RedeemFinePrint"]);
			$itemArr["RedeemTermsAndConditions"] = $this->doContentSpe($itemArr["RedeemTermsAndConditions"]);
			if (!empty($itemArr["UpdateTextTimeStampUTC"])) {
				$itemArr["UpdateTextTimeStampUTC"] = date("Y-m-d H:i:s",(substr($itemArr["UpdateTextTimeStampUTC"],6,10)+8*60*60));
			}
			//酒店信息
			$storeModel = Factory::loadModel("Store");
			$storeArr = $storeModel->getOneItem("pid='{$itemArr["Id"]}'");
			$itemArr["storeArr"] = $storeArr;
			//图片列表
			$imgModel = Factory::loadModel("ProductImg");
			$imgList = $imgModel->getItemList("pid='{$itemArr["Id"]}'","","rank ASC");
			$itemArr["imgList"] = $imgList;
			//如果用户收藏或转发了某产品，下次再进来时就需要判断是否过期，下线的产品，统一设置IsPurchasable=0，即售罄
			$expiryTime = date("Y-m-d H:i:s",time()+24*60*60);
			if ( $itemArr["IsSoldOut"]=="1" || $itemArr["InternalStatus"]!="Published" || $itemArr["PurchaseExpiryDateUtc"]<$expiryTime ) {
				$itemArr["IsPurchasable"] = "0";
			}

			//子产品
			$productModel = Factory::loadModel("Product");
			$childList = $productModel->getItemList("ParentDealId='{$itemArr["Id"]}'","","id ASC");
			if(empty($childList)){//子类为空的情况，信息重复方便前端获取
				$itemArr["childList"][] = $itemArr;
			}else{
				$itemArr["childList"] = $childList;
			}
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