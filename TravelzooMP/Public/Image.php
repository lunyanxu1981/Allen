<?php

/**使用方法
 * 	在图片名后面跟-尺寸
 *	http://xxx.com/image/17-200x100.jpg 按照200x100比例从中间开始裁切
 *	http://xxx.com/image/17-200.jpg     按照宽高200从中间裁剪正方形
 *	http://xxx.com/image/17-200x0.jpg   按照宽200等比例缩小图片
 *	http://xxx.com/image/17-0x200.jpg   按照高200等比例缩小图片
 */
require_once 'ThumbHandler.php';

class Image
{
	protected static $_i;
	
	/* ThumbHandler Class */
	protected $_t;
	
	/* ImageSrc; */
	protected $_src;
	
	/* ImageName; */
	protected $_name;
	
	/* cutImgDir; */
	protected $_cutImgDir;
	
	public static function getInstence()
	{
		return (self::$_i instanceof Image) ? self::$_i : self::$_i = new Image();
	}
	
	public function __construct()
	{
		$this->_t = new ThumbHandler();
	}
	
	/**
	 *  压缩图片;
	 * @param string $src
	 * @param int $widthDeprecated
	 * @param int $height
	 */
	public function Thumb($src, $width, $height)
	{
		$this->_t->setSrcImg($src);
		$this->_t->setCutType(1);
		$this->_t->createImg($width, $height);
	}
	
	public function ThumbUrl($detailArr)
	{
		list($src, $width, $height) = $detailArr;
		
		//echo $width.','.$height;die();
		if (!file_exists($src))
			 $this->_error();
		else
		{
			list($cutWidth, $cutHeight,$startX,$startY) = $this->_getOrCutSize($src, $width, $height,1);
			//echo $cutWidth.','.$cutHeight.','.$startX.','.$startY;die();
			$this->_t->setSrcImg($src);
			$this->_t->setCutType(2);
			$this->_t->setSrcCutPosition($startX, $startY);
			$this->_t->setRectangleCut($cutWidth, $cutHeight);
			$this->_t->setDstImg($this->_cutImgDir);
			if ($width == 1 && $height == 1){
				$this->_t->createImg($cutWidth, $cutHeight);
			}else{
				$this->_t->createImg($width, $height);
			}
			
		}
	}
	
	public function ThumbUrlSquare($detailArr)
	{
		
		list($src, $width, $height) = $detailArr;
		if (!file_exists($src))
			 $this->_error();
		else
		{
			$this->_t->setSrcImg($src);
			$this->_t->setCutType(1);
			$this->_t->setDstImg($this->_cutImgDir);
			$this->_t->createImg($width, $height);
		}
	}
	
	
	public function SmallImg($detailArr){
		list($src, $width, $height) = $detailArr;
		
		//echo $src.$width.','.$height;die();
		
		if (!file_exists($src))
			$this->_error();
		else
		{
			list($w, $h) = $this->_getOrSamllSize($src, $width, $height);
			$this->_t->setSrcImg($src);
			$this->_t->setDstImg($this->_cutImgDir);
			$this->_t->createImg($w, $h);
				
		}
	}
		
	
	public function getExistImage(){
		$detailArr = $this->_getUrlDetail();	
		//list($src, $width, $height) = $detailArr;
		if (!file_exists($this->_cutImgDir)){
			//$this->ThumbUrlSquare($detailArr);
			if ($detailArr[1] == 0 || $detailArr[2] == 0){
				$this->SmallImg($detailArr);
			}else{
				$this->ThumbUrl($detailArr);
			}
			
		} 
		//die();
			
		$this->_outputFile($this->_cutImgDir);
	
	}
	
	protected function _outputFile($src){
		$imgType = $this->_t->_getImgType($src);
		$modifiedTime = filemtime($src);
	
		if(isset($_SERVER['HTTP_IF_MODIFIED_SINCE']))
		{
			header('HTTP/1.1 304 Not Modified');
			exit;
		}
		header("Content-type:$imgType");
		header("Last-Modified: ".gmdate("D, d M Y H:i:s",$modifiedTime)." GMT");
		readfile($src);
	}
	
	protected function _getImageDir(){
		return substr(md5($this->_name), 0,2);
	}
	
	//获取Url;
	protected function _getUrl()
	{
		return isset($_SERVER['REQUEST_URI']) ? $_SERVER['REQUEST_URI'] : '';
	}
	
	//辨识Url
	protected function _getUrlDetail($url=null)
	{
		$url = empty($url) ? $this->_getUrl() : $url;
		$imageName = basename($url);
		$this->_name = $imageName;		
		$path = dirname($url);
		list($name,$width, $height, $type) = $this->_getImageInfo($imageName);
		$src = substr($path,1) . '/' . $name . $type;
		$this->_getUrlDir($width, $height);
		return array($src, $width, $height);
	}
	
	//生成存储路径
	protected function _getUrlDir($width, $height){
		$dir = $this->_getImageDir();
		if ($width == 0 || $height == 0){
			$this->_cutImgDir = "smallImages/{$dir}/{$this->_name}";
		} else{
			$this->_cutImgDir = "cutImages/{$dir}/{$this->_name}";
		}
		
	}
	
	protected function _getImageInfo($imageName)
	{
		list($name, $size, $type) = $this->_split('[-|.]', $imageName);
		list($width, $height) = $this->_rules($size);
		return array($name, $width, $height, !empty($type) ? '.' .$type : '');
	}
	
	protected function _rules($size)
	{
		if (stripos($size, 'x')> -1)
			list($w, $h) = explode('x', $size);
		else if (is_numeric($size) && $size > 0)
			$w = $h = $size;
			
		return array($w, $h);
	}
	
	protected function _split($pattern, $string)
	{
		if (function_exists('preg_split'))
		{
			$pattern = '/' . $pattern . '/';
			return preg_split($pattern, $string);
		}
		else if (function_exists('split'))
			return split($pattern, $string);
	}
	
	protected function _error()
	{
		header("HTTP/1.1 404 Not Found");  
		header("Status: 404 Not Found");
		exit;  
	}
	
	//获取要切图的长宽比列，
	/**
	 * 
	 * @param unknown_type $src
	 * @param unknown_type $width
	 * @param unknown_type $height
	 */
	protected function _getOrCutSize($src, $width, $height,$cutType = 0)
	{
		$imageInfo = @getimagesize($src);
		list($orWidth, $orHeight) = $imageInfo;
		$orRatio = $orWidth/$orHeight;
		$reRatio = $width/$height;
		
		$startX = $startY = 0;
		
		//原始比列 > 新比列时 : 固定高度Height;
		if ($orRatio > $reRatio)
		{
			$newHeight = $orHeight;
			$newWidth = $reRatio * $orHeight;
			if ($cutType == 1){
				$startX = intval((($orWidth - $newWidth)/2));
			}
		}
		//原始比列 < 新比列时: 固定宽度 Widht;
		elseif ($orRatio < $reRatio)
		{
			$newWidth = $orWidth;
			$newHeight = $orWidth / $reRatio;
			if ($cutType == 1){
				$startY = intval((($orHeight - $newHeight)/2));
			}
		}else if ($orRatio == $reRatio)
		{
			$newWidth = $orWidth;
			$newHeight = $orHeight;
		}
		
		if ($width == 1 && $height == 1){
			$newWidth = $orWidth;
			$newHeight = $orHeight;
			$startX = $startY = 0;
		}
		return array($newWidth, $newHeight,$startX,$startY);
	}
	
	/**
	 * 计算缩略图的长宽比
	 */
	protected function _getOrSamllSize($src,$width,$height){
		$imageInfo = @getimagesize($src);
		list($orWidth, $orHeight) = $imageInfo;
				
		$max = ($width == 0) ? $height : $width;
		
		if ($width == 0){
			$h = ($max > $orHeight) ? $orHeight : $max;
			$w= ($max > $orHeight) ? $orWidth : $orWidth*($max/$orHeight);
		}
		else {
			$w= ($max > $orWidth) ? $orWidth : $max;
			$h= ($max > $orWidth) ? $orHeight : $orHeight*($max/$orWidth);
		}
		/* if($orWidth > $orHeight){
			$w= ($max > $orWidth) ? $orWidth : $max;
			$h= ($max > $orWidth) ? $orHeight : $orHeight*($max/$orWidth);
		}else{
			$h= ($max > $orHeight) ? $orHeight : $max;
			$w= ($max > $orHeight) ? $orWidth : $orWidth*($max/$orHeight);;
		} */
		//echo $w.','.$h;die();
		return array($w, $h);
	}
	
	
	
	
	
	
	
	
	
	
	
	
}