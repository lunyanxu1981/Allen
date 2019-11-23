<?php
/**
 * 后台推荐管理
 *
 */
class Recommend extends Controller
{    
	/**
	 * 查询
	 */
    public function index(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
        //得到参数
		$getArr = $this->request->get();
		$perPage = 30;
		$nowPage = (isset($getArr["p"]) && $getArr["p"]>=1) ? $getArr["p"] : 1;
		//取列表
		$whereArr = "is_del=0";
		
		$searchArr["item"] = $getArr["item"];
		$data["searchArr"] = $searchArr;
		if( !empty($getArr["item"]) ){
			$whereArr .= " AND type='{$getArr["item"]}'";
		}

		$itemObjR = Factory::loadModel("Recommend");
		$itemList = $itemObjR->getItemList($whereArr,($nowPage-1)*$perPage.",".$perPage,"status DESC,type ASC,rank DESC,id ASC");
		$data["itemList"] = $itemList;
		//得到总数
		$totalNum = $itemObjR->getCount($whereArr);
		//分页设置
		$page = new Page(array("total"=>$totalNum,"perpage"=>$perPage,"nowindex"=>$nowPage));
		$data["page"] = $page->show(99);
		
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("recommend_index.html",$data);
    }
	/**
	 * 新增
	 */
    public function add(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
		$data["frontURL"] = $this->request->frontURL();
		//处理提交
		if ($this->request->isPostBack()) {
			$postArr = $this->request->getPost();
			// dump($postArr);
			
			if($_FILES['img_arr']['name'] != ""){
				if($_FILES['img_arr']['error'] == 1){
					js_show("alert('上传失败，上传文件超过2M');");
					exit();
				}
				//建文件夹
				$photoDir = "../Web/images/upload/recommend/".date("Ym");
				File::mkDir($photoDir, 0777);
				//上传文件
				$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
				$upFileObj = new UpFile($photoDir, $fileFormat);
				$upFileObj->run("img_arr", 1);
				$upFileArr = $upFileObj->getInfo();
			}
			
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("Recommend",$master);
			$itemId = $itemObjW->addItem($postArr,$upFileArr);
			js_show("alert('新增成功！');parent.location.href='/recommend_index.html'");
			exit();
		}
		// $data["typeList"] = get_one_type_list(0);
		// 加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("recommend_add.html",$data);
    }
	/**
	 * 编辑
	 */
	public function edit(){
		//登录验证
		$psObj = new PublicService();
		$psObj->checkLoginBackend();
		$uid = Cookie::get("tybkud",true);
		$data["frontURL"] = $this->request->frontURL();
		// 处理提交
		if($this->request->isPostBack()){
			$postArr = $this->request->getPost();
			
			if($_FILES['img_arr']['name'] != ""){
				if($_FILES['img_arr']['error'] == 1){
					js_show("alert('上传失败，上传文件超过2M');");
					exit();
				}
				//建文件夹
				$photoDir = "../Web/images/upload/recommend/".date("Ym");
				File::mkDir($photoDir, 0777);
				//上传文件
				$fileFormat = array('gif', 'jpg', 'jpeg', 'png', 'bmp');
				$upFileObj = new UpFile($photoDir, $fileFormat);
				$upFileObj->run("img_arr", 1);
				$upFileArr = $upFileObj->getInfo();
			}
			
			//写DB
			$master = Factory::loadDB("master");
			$itemObjW = Factory::loadModel("Recommend",$master);
			$itemObjW->updateItem($postArr,"id='{$postArr["eid"]}'",$upFileArr);
			js_show("alert('修改成功！');parent.location.href='{$postArr["frontURL"]}'");
			exit();
		}
		$getArr = $this->request->get();
		$itemId = $getArr["eid"];
		//取单条值
		$itemObjR = Factory::loadModel("Recommend");
		$itemArr = $itemObjR->getOneItem("id='$itemId'");
		if(empty($itemArr) || $itemArr["is_del"] == "1"){
			js_msg_go("此信息不存在或已被删除",$data["frontURL"]);
			exit();
		}
		$data["itemArr"] = $itemArr;
		// $data["typeList"] = get_one_type_list(0);
		//加载视图
		$data['header'] = PublicService::getHeaderHtmlBackend();
		$data['footer'] = PublicService::getFooterHtmlBackend();
		return View::parse("recommend_edit.html",$data);
	}
	/**
	 * 接口名称：删除项目
	 * URL：	/recommend_del.html
	 * 方式：	POST
	 * 参数：	eid
	 * 返回：	-1 未登录，-2 参数错误，1 成功
	 */
	public function del(){
		//登录验证
		$psObj = new PublicService();
		$isLogin = $psObj->checkLoginBackend(true);
		$uid = Cookie::get("tybkud",true);
		if(!$isLogin) exit("-1");
		
		$postArr = $this->request->getPost();
		$itemId = $postArr["eid"];
		if(!is_numeric($itemId)) exit("-2");
		
		$master = Factory::loadDB("master");
		$itemObjW = Factory::loadModel("Recommend",$master);
		$itemObjW->delItem($itemId);
		exit("1");
	}
	 /**
	 * 接口名称：设置排序值
	 * URL：	/recommend_setRank.html
	 * 方式：	POST
	 * 参数：	eid
	 * 返回：	-1 未登录，-2 参数错误，1 成功
	 */
    public function setRank(){
		//登录验证
		$psObj = new PublicService();
		$isLogin = $psObj->checkLoginBackend(true);
		$uid = Cookie::get("tybkud",true);
		if(!$isLogin) exit("-1");
		
		$postArr = $this->request->getPost();
		$itemId = $postArr["eid"];
		if(!is_numeric($itemId)) exit("-2");
		
		$master = Factory::loadDB("master");
		$itemObjW = Factory::loadModel("Recommend",$master);
		$itemObjW->updateOne("rank={$postArr["rank"]}","id='$itemId'");
		exit("1");
    }
	
	
	
}
?>