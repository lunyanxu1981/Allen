var app = getApp();
var link = app.data.servsers;

Page({
  data: {
    // userInfo:{}
  },
  onLoad: function () {
    var that = this;
    wx.login({
      success: function () {
        wx.getUserInfo({
          success: function (res) {
            that.setData({
              userInfo: res.userInfo
            })
            console.log(res.userInfo);
          }
        })

      }
    });
  }
})