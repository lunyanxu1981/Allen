var subUrl = "/esteelauder_saveInfo.html";
var phone="";
var name="";
var city="";
var bh = document.body.offsetHeight;
$(function() {
	$(".submit").on("click",function(){
		phone = $("#phone").val();
		name = $("#name").val();
		city = $("#city").val();
		if(name == "" || city.length < 2){
			showLayer('信息不完整');
            return false;
		}
		if(!phone.match(/^1[3|4|5|7|8][0-9]\d{8}$/)){
			showLayer("手机号格式错误");
			return false;
		}		
		$.post(subUrl, {
			phone: phone,
			name:name,
			city:city
	    }, function(data) {
	        var data = JSON.parse(data);
	        // data.status = 405;
	        //if (data.status == '405') pages.yilin();
	        showLayer(data.data);
	    })
	})
	
	$(".popup-button").on("click",function(){
		$(".popup").removeClass("active");
		//move();
	});
});

function showLayer(txt){
	$(".popup-content").text(txt);
	$(".popup").addClass("active");
	$(".popup-box").addClass("zoomIn");
	//stop();
}


//实现滚动条无法滚动
var mo=function(e){e.preventDefault();};

/***禁止滑动***/
function stop(){
        document.body.style.overflow='hidden';        
        document.addEventListener("touchmove",mo,false);//禁止页面滑动
}

/***取消滑动限制***/
function move(){
        document.body.style.overflow='';//出现滚动条
        document.removeEventListener("touchmove",mo,false);        
}