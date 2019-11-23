var app = getApp();
var link = app.data.servsers;
console.log(link)
Page({
/*** 页面的初始数据*/
  data: {
    index: "",
    Id: 0,
    productDetail: "",
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    console.log(options)
    var did = options.did;
    var openid = wx.getStorageSync('openid');
    console.log(openid);
    var that = this;
    wx.request({//渲染兑换券页面
      url: link + '/api_getVoucherDetail.html',
      data: {
        openid: openid,
        did: did
      },
      method: 'POST',
      header: {
        "content-type": "application/x-www-form-urlencoded"
      },
      success: function (res) {
        console.log(res.data);
        console.log(res.data.data.id)
        that.setData({
          productDetail: res.data.data
        })
      }
    })
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

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {
  
  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {
  
  },

})