var app = getApp();
var link = app.data.servsers;
var page = 1;

var discount = function (that){
  var openid2 = wx.getStorageSync('openid');
  wx.request({
    url: link + '/api_getMyCouponList.html', // 接口名称：我的优惠券列表
    data: {
      p: page,
      openid: openid2,
      status:-1
    },
    method: 'POST',
    header: {
      'content-type': 'application/x-www-form-urlencoded' // 默认值
    },
    success: function (res) {
      console.log(res.data)
      that.setData({
        discount: res.data.data
      })
      
    }
  })
}

Page({
  /**
   * 页面的初始数据
   */
  data: {
    num:3,
    discount:[{
      disId:"1",
      disName:"新人优惠券",
      coupou:"全部商品通用",
      dispic:"50",
      disShowtime:"2018.03.07",
      disEnd:"即将过期"
    },
    {
      disId: "2",
      disName: "通用红包",
      coupou: "仅部分商品通用",
      dispic: "0.6",
      disShowtime: "2018.03.07",
      disEnd: "即将过期"
    },
    {
      disId: "3",
      disName: "鲜果超市红包",
      coupou: "全部商品通用",
      dispic: "8",
      disShowtime: "2018.03.07",
      disEnd: "即将过期"
    }]
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;
    discount(that);
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
    wx.stopPullDownRefresh();
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