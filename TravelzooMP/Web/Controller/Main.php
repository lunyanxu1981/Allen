<?php
/**
 *
 */
class Main extends Controller
{
	
	
	public function debug(){
		
		    
	}
	
	//读memcache数据
	public function getMem() {
		$paramArr = $this->request->get();
		$mem = new Memcached ();
		$mem->addServer ( 'localhost', 11211 );
		$result = $mem->get($paramArr["key"]);
		var_dump($result);
	}
	//删除memcache数据
	public function cleanMem() {
		$paramArr = $this->request->getGet();
		$mem = new Memcached ();
		$mem->addServer ( 'localhost', 11211 );
		if (empty($paramArr)) {
			$result = $mem->flush();
		}else{
			$result = $mem->delete($paramArr["key"]);
		}
		dump($result);
	}
	
	//读memcache数据
	public function test() {
		$paramArr = $this->request->get();
		$mem = new Memcached ();
		$mem->addServer ( 'localhost', 11211 );
		$keys = $mem->getAllKeys();
		dump($keys);
		
	}
	
	public function phpinfo(){
		phpinfo();
	}

	public function chanel(){
		$query = empty($_SERVER["QUERY_STRING"]) ? "" : "?".$_SERVER["QUERY_STRING"];
		redirect("http://chfa.minuteschina.com/service_regDp.html".$query);
	}


}
?>
