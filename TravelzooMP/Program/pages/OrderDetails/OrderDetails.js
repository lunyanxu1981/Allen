var app = getApp();
var link = app.data.servsers;
console.log(link)
Page({
  /**
   * 页面的初始数据
   */
  data: {
    index: "",
    id: 0,
    selected: true,
    selected1: false,
    productDetail:""
  },
  selected: function (e) {
    this.setData({
      selected1: false,
      selected: true
    })
  },
  selected1: function (e) {
    this.setData({
      selected: false,
      selected1: true
    })
  },
  lookCou:function(e){
    console.log(e)
    var index = e.currentTarget.dataset.id;
    console.log(index)
    wx.setStorage({
      key: 'order',
      data: {
        index: index
      }
    })
    wx.redirectTo({
      url: '/pages/conPon/conPon'
    })
  },
  /**
   * 生命周期函数--监听页面加载
   */

  onLoad: function (options) {
    console.log('11', options)
    var did = options.did;
    console.log(111,did)
    var openid = wx.getStorageSync('openid');
    console.log(211,openid);
    var that = this;
    wx.request({
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
        console.log(222,res.data);
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