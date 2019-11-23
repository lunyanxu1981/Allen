function shownoty(msg,type){
	var str = '{"text":"'+msg+'","layout":"center","type":"'+type+'"}';
    var options = $.parseJSON(str);
    noty(options);

}
