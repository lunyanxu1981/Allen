
var showError = function(id,msg){
	$("#"+id).removeClass();
	$("#"+id).addClass("input_bad");
	$("#"+id).html(msg);
};
var showOk = function(id){
	$("#"+id).removeClass();
	$("#"+id).html("");
	$("#"+id).addClass("input_good");
};
var validOriPwd = function(id){
	var v = $.trim($("#"+id).val());
	if(v == ""){
		showError(id+"_info","原密码不能为空");
		return false;
	}else if(v.length < 6 || v.length > 20){
		showError(id+"_info","密码必须在6-20个字符之间");
		return false;
	}else{
		showOk(id+"_info");
		return true;
	}
};
var validNewPwd = function(id){
	var v = $.trim($("#"+id).val());
	if(v == ""){
		showError(id+"_info","新密码不能为空");
		return false;
	}else if(v.length < 6 || v.length > 20){
		showError(id+"_info","密码必须在6-20个字符之间");
		return false;
	}else{
		showOk(id+"_info");
		return true;
	}
};
var validRePwd = function(id,reid){
	var v = $.trim($("#"+id).val());
	var rev = $.trim($("#"+reid).val());
	if(rev != v){
		showError(reid+"_info","两次输入的密码不一样");
		return false;
	}else{
		showOk(reid+"_info");
		return true;
	}
};
var validName = function(id){
	var v = $.trim($("#"+id).val());
	if(v == ""){		
		showError(id+"_info","email不能为空");
		return false;
	}else{
		showOk(id+"_info");
		return true;
	}
};
var validEmail = function(id){
	var v = $.trim($("#"+id).val());
	if(v == ""){		
		showError(id+"_info","email不能为空");
		return false;
	}else if(!/.+@.+\.[a-zA-Z]{2,4}$/.test(v)){
		showError(id+"_info","email输入不合法");
		return false;
	}else{
		showOk(id+"_info");
		return true;
	}
};
var cutNum = function(id){
	var t_num = parseInt($("#"+id).text());
	$("#"+id).text(t_num-1);
};
var addNum = function(id){
	var t_num = parseInt($("#"+id).text());
	$("#"+id).text(t_num+1);
};




