const app = getApp();
const link = app.data.servsers;
const isnew = app.data.isnew;
const openid = wx.getStorageSync('openid');
const tj = app.aldstat;
const MAX_PAGES = 10;

Page({
  /*** 页面的初始数据*/
  data: {
    p: 1,
    index: "",
    updateTime: "",
    indexItem: 0,
    topday: "",
    showloading: false,
    flag: true,
    product: [],
    old_user: true,
    albumDisabled: true,
    bindDisabled: false
  },
  goUrl (e) {
    const { url, banner, ty, title, id } = e.currentTarget.dataset;
    if(ty === 'banner'){
      tj.sendEvent(`banner${banner}`, {
        banner
      });
    }else if(ty === 'nav'){
      tj.sendEvent(`导航-${title}`, {
        title
      });
    }else if(ty === 'product'){
        tj.sendEvent(`产品-${id}`, {
          id
        });
    }
    wx.navigateTo({ url });
  },
  index_goods () {
    const that = this;
    let { p } = that.data;
    wx.request({
      showloading: true,
      url: link + '/api_getProductList.html',
      method: 'POST',
      data: {
        p,
      },
      header: { 'content-type': 'application/x-www-form-urlencoded' },
      success: function (res) {
        if (res.data.status == 200) {
          let { product } = that.data;
          product = product.concat(res.data.data)
          p++;
          that.setData({
            showloading: false,
            updateTime: res.data.updateTime,
            product,
            p
          });
        } else {
          that.setData({
            showloading: false
          })
        }
      }
    })
  },

  list () {
    const that = this;
    that.setData({
      hidden: false
    });
    wx.request({
      url: link + '/api_getRecommendList.html', // 接口名称：首页推荐分类列表
      method: 'POST',
      data: {},
      header: {
        'content-type': 'application/json'//会对数据进行 JSON 序列化
      },
      success: function (res) {
        that.setData({
          aid: res.data.data[0].aid,
          httpPostImg: res.data.data[0].httpPostImg,
          navs: res.data.data,
          PostImg: res.data.data[1].httpPostImg,
          PostPic: res.data.data[2].httpPostImg,
          tid1: res.data.data[1].id,
          tid2: res.data.data[2].id
        })
      }
    })
  },

  purchase () {
    const that = this;
    wx.request({
      url: link + '/api_getMyOneSuccessOrder.html',//接口名称：获取我最近一次订单中的资料/判断是否购买过商品
      data: {
        openid,
      },
      method: 'POST',
      header: {
        'content-type': 'application/x-www-form-urlencoded' // 会将数据转换成 query string 
      },
      success: function (res) {
        if (res.data.couponArr != "") { //couponArr如果不为空，那说明这个人已经领取过优惠券了，就不用再弹窗了，如果为空，则弹窗
          wx.hideLoading();
        } else {
          if (res.data.data == false) {
            that.setData({ flag: false });//50元新人优惠券
          } else {
            // that.setData({ old_user: false })//68元老用户优惠券
          }
        }
      }
    })
  },

  //出现
  show: function () {
    this.setData({ flag: false, old_user: false });
  },

  //消失
  hide: function () {
    this.setData({ flag: true, old_user: true });
  },

  //68元的立即领取
  receive_68: function () {
    var openid2 = wx.getStorageSync('openid');
    var that = this;
    wx.request({
      showloading: true,
      url: link + '/api_addCoupon.html',//接口名称：邻取优惠券68
      method: 'POST',
      data: {
        openid: openid2,
        cid: 2
      },
      header: { 'content-type': 'application/x-www-form-urlencoded' },
      success: function (res) {
        wx.showModal({
          showCancel: false,
          content: res.data.data,
        })

        that.setData({ old_user: true, showloading: false })
      }
    })
  },

  //50元立即领取
  receive_50: function () {
    var openid2 = wx.getStorageSync('openid');
    var that = this;
    //请求
    wx.request({
      showloading: true,
      url: link + '/api_addCoupon.html',//接口名称：邻取优惠券68
      method: 'POST',
      data: {
        openid: openid2,
        cid: 5
      },
      header: { 'content-type': 'application/x-www-form-urlencoded' },
      success: function (res) {
        wx.showModal({
          showCancel: false,
          content: res.data.data,
        })
        that.setData({ flag: true, showloading: false })
      }
    })
  },

  /**
   * 生命周期函数--监听页面加载
   */

  onLoad: function (options) {
    const that = this;
    let { scene } = options;
    if (scene) {
      scene = decodeURIComponent(scene);
      const gid = scene.split(',')[1];
      wx.navigateTo({
        url: `/pages/tuanOrderDetails/index?gid=${gid}`
      })
    }

    // url参数中可以获取到gdt_vid、weixinadinfo参数值
    let gdt_vid = options.gdt_vid;
    let weixinadinfo = options.weixinadinfo
    // 取广告id
    let aid = 0;
    if (weixinadinfo) {
      let weixinadinfoArr = weixinadinfo.split('.');
      aid = weixinadinfoArr[0];
    }
    setTimeout(function () {
      that.purchase();
      /*app.getOpenid(function (openid) {
        console.log(openid);
        purchase(that, openid);
      });*/
    }, 1000);
    that.index_goods();
    that.list();
    //判断是用户是否绑定了
    if (app.globalData.employId && app.globalData.employId != '') {
      that.setData({
        albumDisabled: false,
        bindDisabled: true
      });
    } else {
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.employIdCallback = employId => {
        if (employId != '') {
          that.setData({
            albumDisabled: false,
            bindDisabled: true
          });
        }
      }

    }

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
    var that = this;
    that.list();
    wx.stopPullDownRefresh();
  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {
    var that = this;
    const { p } = that.data;
    if (p <= MAX_PAGES) {
      that.index_goods();
    }
  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  }
})