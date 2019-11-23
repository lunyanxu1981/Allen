<?php
/**
 *
 */
class TravelZoo
{
	
	private $baseUrl = TZOO_BASE_URL;
	private $soapUrl = TZOO_SOAP_URL;
	private $apiVersion = TZOO_API_VERSION;
	private $apiKey = TZOO_API_KEY;
	private $apiAffiliateId = TZOO_API_AFFILIATEID;
	private $affiliateId = TZOO_AFFILIATEID;
	private $localeCode = "cn";
	private $tzlocale = 12;
	private $responseType = "json";

	public function __construct(){
		
	}

	//获取产品
	public function getDeals($paramsArr){
		$apiUrl = "/Deals?ApiVersion=".$this->apiVersion."&ApiKey=".$this->apiKey."&ApiAffiliateId=".$this->apiAffiliateId."&AffiliateId=".$this->affiliateId."&localeCode=".$this->localeCode."&Offset=".$paramsArr["offset"]."&Limit=".$paramsArr["limit"]."&ResponseType=".$this->responseType;
		// MpfLog::write("tzoo","get deals:\n".$this->baseUrl.$apiUrl);

		// $retJson = curl_get($this->baseUrl.$apiUrl);
		$retJson = file_get_contents($this->baseUrl.$apiUrl);
		$retList = json_decode($retJson,true);
		return $retList;
	}
	//购买产品
	public function buyVouchers($paramsArr){
		$apiUrl = "/Vouchers/Buy";
		$paramsArr["Tzlocale"] = $this->tzlocale;
		$paramsArr["ApiKey"] = $this->apiKey;
		$paramsArr["ApiVersion"] = $this->apiVersion;
		$paramsArr["ApiAffiliateId"] = $this->apiAffiliateId;
		$paramsArr["AffiliateId"] = $this->affiliateId;
		$paramsArr["ResponseType"] = $this->responseType;
		
		// $retJson = curl_post_data($this->baseUrl.$apiUrl,$paramsArr);
		$retJson = fgc_post_data($this->baseUrl.$apiUrl,$paramsArr);
		MpfLog::write("tzoo","buy vouchers:\n"."params:\n".json_encode($paramsArr)."\n return:\n".$retJson);

		$retList = json_decode($retJson,true);
		return $retList;
	}
	//获取兑换券状态
	public function getVoucherStatus($paramsArr){
		$apiUrl = "/Vouchers/Status?ApiVersion=".$this->apiVersion."&ApiKey=".$this->apiKey."&ApiAffiliateId=".$this->apiAffiliateId."&AffiliateId=".$this->affiliateId."&PaymentTransactionId=".$paramsArr["transactionId"]."&VoucherIdCSV=".$paramsArr["voucherId"]."&ResponseType=".$this->responseType;
		// MpfLog::write("tzoo","get voucher status:\n".$this->baseUrl.$apiUrl);

		// $retJson = curl_get($this->baseUrl.$apiUrl);
		$retJson = file_get_contents($this->baseUrl.$apiUrl);
		$retList = json_decode($retJson,true);
		return $retList;
	}
	//退款
	public function refundVouchers($pArr){
		$apiUrl = "/Vouchers/Refund";
		$paramsArr["Tzlocale"] = $this->tzlocale;
		$paramsArr["ApiKey"] = $this->apiKey;
		$paramsArr["ApiVersion"] = $this->apiVersion;
		$paramsArr["ApiAffiliateId"] = $this->apiAffiliateId;
		$paramsArr["AffiliateId"] = $this->affiliateId;
		$paramsArr["PaymentTransactionId"] = $pArr["transactionId"];
		$paramsArr["VoucherIdCSV"] = $pArr["voucherId"];
		$paramsArr["ResponseType"] = $this->responseType;
		
		$retJson = fgc_post_data($this->baseUrl.$apiUrl,$paramsArr);
		MpfLog::write("tzoo","refund vouchers:\n"."params:\n".json_encode($paramsArr)."\n return:\n".$retJson);

		$retList = json_decode($retJson,true);
		return $retList;
	}
	//用户注册
	public function signUp($paramsArr){
		if(MY_APP_DOMAIN=="devtravelzoo.m-int.cn") {
			$baseUrl = "https://beta.webservices1.travelzoo.com/SubscriptionService.svc/rest/CreateSubscription";
			$paramsArr["campaignId"] = "1564";
			$paramsArr["campaignSource"] = "wcmp_test";
			$paramsArr["campaignCode"] = "mpSignupAPI";
		}else{
			$baseUrl = "https://webservices1.travelzoo.com/SubscriptionService.svc/rest/CreateSubscription";
			$paramsArr["campaignId"] = "1564";
			$paramsArr["campaignSource"] = "wcmp_all";
			$paramsArr["campaignCode"] = "wcmpSignupAPI";
		}
		$paramsArr["username"] = "minutes";
		$paramsArr["pwd"] = "P@ssword123";
		$paramsArr["countryCode"] = "CN";
		$paramsArr["postalCode"] = "";
		$paramsArr["ip"] = get_client_ip();
		$paramsArr["optInTop20"] = "true";
		$paramsArr["optInNewsflash"] = "true";
		$paramsArr["optInPromo"] = "true";
		$apiUrl = $baseUrl."?Username={$paramsArr["username"]}&Password={$paramsArr["pwd"]}&CampaignId={$paramsArr["campaignId"]}&CampaignSource={$paramsArr["campaignSource"]}&EmailAddress={$paramsArr["email"]}&CountryCode={$paramsArr["countryCode"]}&PostalCode={$paramsArr["postalCode"]}&IPAddress={$paramsArr["ip"]}&OptInTop20={$paramsArr["optInTop20"]}&OptInNewsflash={$paramsArr["optInNewsflash"]}&OptInPromo={$paramsArr["optInPromo"]}&AdId={$paramsArr["adId"]}&CampaignCode={$paramsArr["campaignCode"]}&postalcode=021";
		$retJson = file_get_contents($apiUrl);
		MpfLog::write("tzoo","sign up:\n"."params:\n".$apiUrl."\n return:\n".$retJson);

		$retList = json_decode($retJson,true);
		return $retList;

	}



}
?>
