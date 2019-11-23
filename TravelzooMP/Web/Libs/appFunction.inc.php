<?php
/**
 * 应用程序自定义的函数可以放到这里,其它任何控制器或模式都可以直接引用
 */
require_once 'cnzz_cs.php';
include_once 'phpqrcode.php';

function getCNZZHtml($code){	
	$data["img"] = _cnzzTrackPageView($code);
	return View::showHtml("_cnzz.html" ,$data);
}

//生成二维码图片
function create_QRcode_img($fileDir,$fileName,$content){	
	File::mkDir($fileDir,0777);
	if (!file_exists($fileDir.$fileName)) {
		QRcode::png($content,$fileDir.$fileName,"H","6","2");
	}
}















?>