// pages/test/test.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    array: ['七夕活动满1000减520', '新人优惠省50元新人优惠省50元', '新人优惠省50元新人优惠省50元巴西', '新人优惠省50元元新人优惠省50元新人优惠省50元'],
    flag: true,
    index: 0,
  },

  //出现
  show: function () {

    this.setData({ flag: false })

  },
  //消失

  hide: function () {
    
    this.setData({ flag: true })

  },

  receive:function(){
    wx.showToast({
      title: '领取成功',
      icon: 'success',
      duration: 2000
    })
    this.setData({ flag: true })
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    this.setData({ flag: false })
  },

  bindPickerChange: function (e) {
    console.log(e)
    console.log('picker发送选择改变，携带值为', e.detail.value)
    this.setData({
      index: e.detail.value
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

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {
  
  }
})