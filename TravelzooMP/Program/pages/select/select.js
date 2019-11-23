// pages/select/select.js
var app = getApp();
var link = app.data.servsers;
var page = 1;

Page({

  /**
   * 页面的初始数据
   */
  data: {
    showloading: false,
    updateTime: "",
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
    console.log(options);
    var rid = options.rid;
    var tid = options.tid;
    console.log(link);
    showloading: true;
    var that = this;
    //初始化请求数据
    wx.request({
      showloading: true,
      url: link + '/api_getProductListByType.html',//获取分类产品列表
      method: 'POST',
      data: {
        p: page,
        rid: rid,
        tid: tid
      },
      header: {
        "content-type": "application/x-www-form-urlencoded"
      },
      success: function (res) {
        console.log(res.data);
        console.log(res.data.data[0].Id);
        console.log(res.data.desc);

        that.setData({
          product: res.data.data,
          title: res.data.title,
          desc: res.data.desc,
          showloading: false,
        })
        wx.setNavigationBarTitle({ title: res.data.title })
      }
    })

    wx.request({
      showloading: true,
      url: link + '/api_getProductListByType.html',//获取分类产品列表
      method: 'POST',
      data: {
        p: page,
        rid: rid,
        tid: tid
      },
      header: {
        "content-type": "application/x-www-form-urlencoded"
      },
      success: function (res) {
        console.log(res.data);
        console.log(res.data.data[0].Id);
        console.log(res.data.desc);

        that.setData({
          product: res.data.data,
          title: res.data.title,
          desc: res.data.desc,
          showloading: false,
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