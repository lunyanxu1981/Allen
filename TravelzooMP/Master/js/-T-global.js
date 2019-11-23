$(document).ready(function() {

// 收藏活动

	$('body').delegate('.T-fav','click',function()
	{
		var id = $(this).attr('id');

		$.post('/event_addFav.html',{'eid':id},function(d){
			if(d=='1')
			{
				newAlert('收藏成功');
			};
			if(d=='-1')
			{
				newAlert('尚未登录');
			};
			if(d=='-2')
			{
				newAlert('活动不存在');
			};
			if(d=='-3')
			{
				newAlert('活动已经收藏');
			};
		});
		
		
		return false;
	});

	
// 未完成提示

	$('body').delegate('.undone','click',function()
	{
		var id = $(this).parents('.T-e-event-v').attr('id').split('-')[1];
		$.get('/event_unfinished.html?eid='+id,function(d)
		{
			$.fancybox(
						d,
						{
							'autoDimensions': false,
							'width': 350,
							'height': 'auto',
							'transitionIn': 'none',
							'transitionOut': 'none',
							'hideOnOverlayClick': false
						}
					);
		});
		return false;
	});


// 发送私信

	$('body').delegate('#T-sendmsg-send','click',function()
	{
		var title = $('#sendmsg-title').val();
		var content = $('#sendmsg-content').val();
		var toUid = $('.T-sendmsg').attr('to_uid');
		title = title.replace(/(^\s*)|(\s*$)/g,'');
		content = content.replace(/(^\s*)|(\s*$)/g,'');
	
		if(title+content=='')
		{
			newAlert('标题或者内容不能都为空');
		}
		else if (title.length>20)
		{
			newAlert('标题不能超过20个字符');
		}
		else
		{
			var value = {
				title: title,
				msg: content,
				to_uid: toUid
			}
			$.post('/msg_add.html',value,function(d){
				if(d=='1')
				{
					// alert('消息发送成功');
					$.fancybox.close();
				};
				if(d=='-1')
				{
					newAlert('尚未登录');
				};
				if(d=='-2')
				{
					newAlert('不能给自己发送消息');
				};
			});
		};
	
	});

	$('body').delegate('#T-sendmsg-cancel','click',function()
	{
		$.fancybox.close();
	});
	
	
	$('.T-sendmsg').click(function()
	{
		var nickname = $(this).attr('nickname');
			$.fancybox(
				'<style>#fancybox-outer{border:10px solid #474747;border-radius:10px;}#fancybox-content{padding:0;border:0!important;}#fancybox-content caption{height:40px;line-height:40px;background-color:#f8f8f8;text-indent:15px;font-size:14px;color:#545454;}#fancybox-content table{width:100%}#fancybox-content td{padding:10px 0;}#fancybox-content th{width:15%}#fancybox-content input,#fancybox-content textarea{width:290px;resize:none}#fancybox-content button{width:50px;height:28px;border-radius:3px;background-color:#2c2c2c;color:#e8e8e8;}#fancybox-content button{margin-left:10px;}#fancybox-content td{text-align:right}</style><table><caption>发送消息给: '+nickname+'</caption><tr><th>标题</th><td><input type="text" id="sendmsg-title"></td></tr><tr><th>内容</th><td><textarea id="sendmsg-content"></textarea></td></tr><tfoot><td colspan="2"><button id="T-sendmsg-send">发送</button><button id="T-sendmsg-cancel">取消</button></td></tfoot></table>',
				{
					'autoDimensions': false,
					'width': 362,
					'height': '216',
					'transitionIn': 'none',
					'transitionOut': 'none',
					'hideOnOverlayClick': false
				}
			);
	});


	
	
// 显示更多

	$('#showmore').click(function()
	{
		$('#showmore_c').css('height','auto');
 	});
 
 
 
 
// 返回顶部

	function BackToTop(top){
		var toTop,toTop_T,backtotop = $('#backtop');
			if (!window.XMLHttpRequest) {
				backtotop.css({'position':'absolute','bottom':'auto','top': top + $(window).height() - 166});
			}
			clearTimeout(toTop_T);
			toTop_T = setTimeout(function (){
			top > 500? backtotop.show() : backtotop.hide();
			},500);
	};

	(function(){
		var top,totop,totop_a;
		$(window).scroll(function(){
		top = $(document).scrollTop();
		if(totop!==0 && top>500){
			BackToTop(top);
			totop_a=1;
			totop=0;
		}
		if(totop_a===1){
			BackToTop(top);
			}
		});
	})();

	$('#backtop').click(function(){
		$('html,body').animate(
		{
			scrollTop: 0
		}, 200);
	});

});


// 对话弹窗


	function newAlert(msg,url)
	{
		url=url||'';
		$.fancybox(
			'<style>#fancybox-outer{border:10px solid #474747;border-radius:10px;}#fancybox-outer .inner{padding:10px;font-size:16px;text-align:center;height:100px;}#fancybox-outer .inner span{vertical-align:middle;display:inline-block;}#fancybox-outer .inner s{display:inline-block;height:100%;vertical-align:middle}#fancybox-outer .inner i{width:18px;height:17px;margin-right:10px;background-position:0 -450px}#fancybox-content button{background-color: #2C2C2C;border-radius: 3px 3px 3px 3px;color:#E8E8E8;float:right;height:28px;}</style><div class="inner"><s></s><span><i></i>'+msg+'</span></div><button onclick="$.fancybox.close()">确认</button>',
			{
				'autoDimensions': false,
				'width': 400,
				'height': 150,
				'transitionIn': 'none',
				'transitionOut': 'none',
				'hideOnOverlayClick': false,
				'onClosed': function()
				{
					if(url!='')
					{
						window.location.href=url;
					}
				}
			}
		);
	};





	

