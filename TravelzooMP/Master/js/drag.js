var $table = $('.tableone');
var postType = location.pathname.indexOf('product') < 0 ? 'brand' : 'product';
$table.find('>tbody').sortable({
    update: function( event, ui ) {
    var arr = [];
    $table.find('tr:not(:first)').each(function(){
        var id = $(this).find('td:first').text();
        arr.push(id);
    });
    $.post('/Api_setRank.html', {
            pArr: arr,
            type: postType
        }, function(data){
        if(data == 1){
            console.log('success!')
        }
        })
    }
});
