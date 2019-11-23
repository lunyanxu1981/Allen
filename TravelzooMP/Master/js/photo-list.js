var myApp = angular.module('benshiApp', ['angularFileUpload', 'ui.sortable'])
.controller('mainCtrl', function($scope){

})
.controller('MyCtrl', [ '$scope', '$upload', function($scope, $upload) {
    $scope.items = [];
    var initItems = (function(){
        if(!ENV.imgArrStr) return;
        var imgArr = ENV.imgArrStr.split(',');
        imgArr.pop();
        $scope.items = _.map(imgArr, function(str){
            return str.split('*');
        });
    }());

    $scope.setCover = function(items, idx) {
        _.each(items, function(item, i){
            if(i==idx){
                item[2] = 1;
            } else{
                item[2] = 0;
            }
        });

    };

    $scope.deleItem = function(idx) {
        $scope.items.splice(idx, 1);
    };

    $scope.onFileSelect = function($files) {
    //$files: an array of files selected, each file has name, size, and type.
    for (var i = 0; i < $files.length; i++) {
        var file = $files[i];
        $scope.upload = $upload.upload({
        url: '/api_uploadPhoto.html', //upload.php script, node.js route, or servlet url
        method: 'POST',
        // headers: {'header-key': 'header-value'},
        // withCredentials: true,
        data: {
            myObj: $scope.myModelObj
            },
        file: file, // or list of files: $files for html5 only
        // fileName: 'doc.jpg' or ['1.jpg', '2.jpg', ...] // to modify the name of the file
        /* customize file formData name ('Content-Desposition'), server side file variable name.
        Default is 'file' */
        fileFormDataName: 'img_arr', //or a list of names for multiple files (html5).
        /* customize how data is added to formData. See #40#issuecomment-28612000 for sample code */
        //formDataAppender: function(formData, key, val){}
        }).progress(function(evt) {
        console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        }).success(function(data, status, headers, config) {
        // file is uploaded successfully
        var imgArr = data.split('*');
        var set_default = $scope.items.length == 0 ? 1 : 0;
        $scope.items.push([imgArr[0], imgArr[1], set_default]);
        console.log(data);
        });
        //.error(...)
        //.then(success, error, progress);
        //.xhr(function(xhr){xhr.upload.addEventListener(...)})// access and attach any event listener to XMLHttpRequest.
    }
    /* alternative way of uploading, send the file binary with the file's content-type.
    Could be used to upload files to CouchDB, imgur, etc... html5 FileReader is needed.
    It could also be used to monitor the progress of a normal http post/put request with large data*/
    // $scope.upload = $upload.http({...})  see 88#issuecomment-31366487 for sample code.
    };
}]);
