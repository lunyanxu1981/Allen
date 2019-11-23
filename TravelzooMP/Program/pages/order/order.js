var app = getApp();
var link = app.data.servsers;
var page = 1;
var openid = wx.getStorageSync('openid');
var GetList = function (that) {
  that.setData({
    hidden: false
  });
  wx.request({
    url: link + '/api_getMyOrderList.html',
    method: 'POST',
    data: {
      openid: openid,
      p: page
    },
    header: {
      "content-type": "application/x-www-form-urlencoded"
    },
    success: function (res) {
      var status = res.data.status;
      if (page == 1 && res.data.data.length == 0) {
        that.setData({
          nogoods: "空空如也赶快去买买买哦~"
        })
      } if (status == 200) {
        var l = that.data.carArray;
        for (var i = 0; i < res.data.data.length; i++) {
          l.push(res.data.data[i])
          console.log(res.data.data[i])
        }
        that.setData({
          carArray: l
        });
        page++;
      }
    }
  })
}
Page({
  /**
   * 页面的初始数据
   */
  data: {
    index: "",
    carArray: [],
    nogoods: "",
    courseUuid: "",
    isCompany: "",
    id: "",
    txtShow: true,
  },

  clickOrder: function (e) {
    wx.navigateTo({
      url: '/pages/conPon/conPon',
    })

  },

  showTxtToast () {
    wx.showToast({
      title: '您的订单未显示？请重启微信后，再点开看看',
      icon: 'none',
      duration: 1000
    });
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    console.log(options)
    // 页面初始化 options为页面跳转所带来的参数     
    var that = this;
    page = 1;
    GetList(that);
  },

  onPullDownRefresh: function () {
    //下拉  
    console.log("下拉");
    var that = this
    GetList(that);
    wx.stopPullDownRefresh();
    that.setData({ txtShow: false }, () => {
      setTimeout(() => {
        that.setData({ txtShow: true });
      }, 1000);
    });
  },

  onReachBottom: function () {
    //上拉
    console.log("上拉")
    var that = this;
    GetList(that)
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

})