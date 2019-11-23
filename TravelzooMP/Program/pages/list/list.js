// pages/test1/test1.js
var app = getApp();
var link = app.data.servsers;
var page =1;
Page({

  /**
   * 页面的初始数据
   */
  data: {
    showloading: false,
    title:""
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    // url参数中可以获取到gdt_vid、weixinadinfo参数值
    let gdt_vid = options.gdt_vid;
    let weixinadinfo = options.weixinadinfo
    // 取广告id
    let aid = 0;
    if (weixinadinfo) {
      let weixinadinfoArr = weixinadinfo.split('.');
      aid = weixinadinfoArr[0];
    }
    // 页面初始化 options为页面跳转所带来的参数
    console.log(options)
    console.log(page)
    var rid = options.rid;
    var tid = options.tid;
    console.log(rid)
    console.log(tid)
    // this.setData({
    //   title: options.type
    // });

    showloading: true
    var that = this;
    //初始化请求数据
    wx.request({
      showloading: true,
      url: link + '/api_getProductListByType.html',//获取分类产品列表
      method: 'POST',
      data: {
        p:page,
        rid: rid,
        tid:tid
      },
      header: {
        "content-type": "application/x-www-form-urlencoded"
        },
      success: function (res) {
        console.log(res)
        console.log(res.data);
        console.log(res.data.data[0].Id);
        console.log(res.data.title);
        that.setData({
          product: res.data.data,
          showloading: false,
          title: res.data.title,
        })
        wx.setNavigationBarTitle({ title: res.data.title })
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

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {
  
  }
})