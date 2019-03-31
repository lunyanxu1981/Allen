$(function() {
    $('#demo').popover({      
        animation: true,
        placement: 'bottom',
        delay: { "show": 100, "hide": 100 },   
        content: function() {
          return $('#popover_content_taxrate').html();
        }
      }).tooltip();

      $('.panel-button').on('click',function () {
        if($(this).text()=='收起'){
          $(this).text('展开查看');
        }else{
          $(this).text('收起');
        }
      });
});