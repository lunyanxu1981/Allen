import { _getSetting, _login } from '../../utils/util';
var app = getApp();
var link = app.data.servsers;
var purchase = function (that) {
  var openid2 = wx.getStorageSync('openid');
  wx.request({
    url: link + '/api_getMyOneSuccessOrder.html',//接口名称：获取我最近一次订单中的资料/判断是否购买过商品
    data: {
      openid: openid2,
    },
    method: 'POST',
    header: {
      'content-type': 'application/x-www-form-urlencoded' // 会将数据转换成 query string 
    },
    success: function (res) {
      console.log(res.data)
      console.log(res.data.data)
      if (res.data.couponArr != "") { //couponArr如果不为空，那说明这个人已经领取过优惠券了，就不用再弹窗了，如果为空，则弹窗
        wx.hideLoading();
      } else {
        if (res.data.data == false) {
          console.log(1);
          that.setData({ flag: false });//50元新人优惠券
        } else {
          console.log(2);
          // that.setData({ old_user: false })//68元老用户优惠券
        }
      }
    }
  })

}

Page({
  /**页面的初始数据*/
  data: {
    seeAllGoods: false,
    btnMsg: '展开全部',
    Id: 0,
    shopBuyCount: 0,
    productDetail: "",
    Price: "",
    Title: "",
    LocationName: "",
    storeArr: [{ DisplayName: '' }],
    WhyWeLoveIt: "",
    When: "",
    WhatIsIncluded: "",
    selected: true,
    selected1: false,
    selectbtn: false,
    total: 0,
    indexItem: 0,
    flag: true,
    old_user: true,
    transHeight: 300,
    numItem: [{
      count: 0,
      select: 1,
      add: true
    }]
  },
  //出现
  show: function () {
    this.setData({ flag: false })
    this.setData({ old_user: false })
  },

  //消失
  hide: function () {
    this.setData({ flag: true })
    this.setData({ old_user: true })
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
        console.log(res.data)
        console.log(res.data.data)
        wx.showModal({
          showCancel: false,
          content: res.data.data,
        })

        that.setData({ old_user: true })
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
        console.log(res.data)
        wx.showModal({
          showCancel: false,
          content: res.data.data,
        })
        that.setData({ flag: true })
      }
    })
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
  //查看全部
  goodsmore: function (event) {
    console.log(event);
    var that = this;
    var msg = '展开全部';
    if (that.data.seeAllGoods) {
      msg = '展开全部';
    } else {
      msg = '收起';
    }
    that.setData({
      btnMsg: msg,
      seeAllGoods: !that.data.seeAllGoods,
    })

  },
  //返回首页
  goHome: function () {
    console.log(1111)
    wx.switchTab({
      url: '/pages/index/index'
    })
  },

  calling: function (event) {
    console.log(event);
    wx.makePhoneCall({
      phoneNumber: event.currentTarget.dataset.phone,
      success: function () {
        console.log("拨打电话成功！")
      },
      fail: function () {
        console.log("拨打电话失败！")
      }
    })
  },

  //去结算
  goSet: function (e) {
    var indexItem = this.data.indexItem;
    var that = this;
    if (this.data.selectbtn) {
      wx.setStorage({
        key: 'addgoods',
        data: {
          "total": this.data.total,
          "numItem":
          {
            "name": this.data.numItem[indexItem].Title,
            "count": this.data.numItem[indexItem].count,
            "Id": this.data.numItem[indexItem].Id
          }
        },
      })
      wx.navigateTo({
        url: '/pages/firmOrder/firmOrder'
      })
    }
  },

  //商品数量--
  shopCountSub: function (e) {
    var index = e.target.dataset.id;
    var item = this.data.numItem[index];

    if (item.count > 0) {
      item.count = item.count - 1;
      var param = {};
      var str = "numItem[" + index + "].count";
      var select = "numItem[" + index + "].select";
      var price = this.data.numItem[index].Price;
      param[str] = item.count;
      this.setData(param);
      var totalPrice = param[str] * price;
      this.setData({
        total: totalPrice
      })
      if (param[str] == 0) {
        this.setData({
          selectbtn: false
        })
        this.setData({
          [select]: 0
        })
      }
    } else {
      return false;
    }
  },
  //商品数量++
  shopCountAdd: function (e) {
    //数字变化
    var index = e.target.dataset.id;
    this.setData({
      indexItem: index
    })
    var item = this.data.numItem[index];
    var itemStr = this.data.numItem;
    var countNow = item.count;//保存当前选择过的数字

    //禁用
    for (var i = 0; i < this.data.numItem.length; i++) {
      var str = "numItem[" + i + "].add";
      var stR = "numItem[" + i + "].count";
      var select = "numItem[" + i + "].select";
      this.setData({
        [str]: false,
        [stR]: 0,
        [select]: 0
      })
      var str2 = "numItem[" + index + "].add"
      var stR2 = "numItem[" + index + "].count";
      var select2 = "numItem[" + index + "].select";
      this.setData({
        [str2]: true,
        [stR2]: countNow,
        [select2]: 1
      })
    }
    item.count = item.count + 1;
    var param = {};
    var str = "numItem[" + index + "].count";
    var price = this.data.numItem[index].Price;
    param[str] = item.count;
    this.setData(param);
    var totalPrice = param[str] * price;
    this.setData({
      total: totalPrice,
      selectbtn: true
    })
  },
  showModal: function (e) {
    //console.log(this.data.numItem);
    var currentStatu = e.currentTarget.dataset.statu;
    //关闭  
    if (currentStatu == "close") {
      this.setData({
        isDialogShow: false,
        isScroll: true
      });
    }
    // 显示  
    if (currentStatu == "open") {
      this.setData({
        isDialogShow: true,
        isScroll: false
      });
    }
    const transHeight = this.data.transHeight;
    // 显示遮罩层
    var animation = wx.createAnimation({
      duration: 200,
      timingFunction: "linear",
      delay: 0
    });

    this.animation = animation
    animation.translateY(transHeight).step()
    this.setData({
      animationData: animation.export(),
      showModalStatus: true
    })
    setTimeout(function () {
      animation.translateY(0).step()
      this.setData({
        animationData: animation.export()
      })
    }.bind(this), 200)
  },
  hideModal: function () {
    // 隐藏遮罩层
    var animation = wx.createAnimation({
      duration: 200,
      timingFunction: "linear",
      delay: 0
    });
    const transHeight = this.data.transHeight;
    this.animation = animation;
    animation.translateY(transHeight).step()
    this.setData({
      animationData: animation.export(),
    })
    setTimeout(function () {
      animation.translateY(0).step()
      this.setData({
        animationData: animation.export(),
        showModalStatus: false
      })
    }.bind(this), 200)
  },
  /**
   * 生命周期函数--监听页面加载
   */
  getUserInfo (e) {
    const that = this;
    if (!that.data.isAuth) {
      _login(e, that, link)
        .then(openid => console.log(openid))
        .catch(e => console.log(e))
    }
  },
  onLoad: function (options) {
    // url参数中可以获取到gdt_vid、weixinadinfo参数值
    const that = this;
    _getSetting(that);
    const isIphoneX = app.globalData.isIphoneX;
    const transHeight = isIphoneX === 'btn-x' ? 334 : 300;
    that.setData({ isIphoneX, transHeight });
    let gdt_vid = options.gdt_vid;
    let weixinadinfo = options.weixinadinfo;
    // 取广告id
    let aid = 0;
    if (weixinadinfo) {
      let weixinadinfoArr = weixinadinfo.split('.');
      aid = weixinadinfoArr[0];
    }
    setTimeout(function () {
      purchase(that);
      /*app.getOpenid(function (openid) {
        console.log(openid);
        purchase(that, openid);
      });*/
    }, 1000)
    var pid = options.id;
    that.setData({
      gid: options.gid
    })
    wx.request({
      url: link + '/api_getProductDetail.html',
      data: {
        pid: pid
      },
      method: 'POST',
      header: {
        "content-type": "application/x-www-form-urlencoded"
      },
      success: function (res) {
        for (var i = 0; i < res.data.data.childList.length; i++) {
          res.data.data.childList[i].add = true;
          res.data.data.childList[i].count = 0;
          res.data.data.childList[i].select = 0;
        }
        that.setData({
          productDetail: res.data.data,
          numItem: res.data.data.childList
        })
      }
    })
    wx.request({
      url: link + '/api_getLikeProductList.html', //系统推荐产品2个
      data: {
        pid: pid
      },
      method: 'POST',
      header: {
        "content-type": "application/x-www-form-urlencoded"
      },
      success: function (res) {
        that.setData({
          goods: res.data.data
        })
      }
    })
    // wx.getStorage({
    //   key: 'href',
    //   success: function (res) {
    //     wx.hideToast();
    //     that.setData({
    //       Id: res.data
    //     })
    //     var pid = that.data.Id;
    //     console.log(pid);
    //     wx.request({
    //       url: link + '/api_getProductDetail.html',
    //       data: {
    //         pid: pid.index
    //       },
    //       method: 'POST',
    //       header: {
    //         "content-type":"application/x-www-form-urlencoded"
    //       },
    //       success: function (res) {
    //         console.log(res.data);
    //         for (var i = 0; i < res.data.data.childList.length; i++)  {
    //           res.data.data.childList[i].add = true;
    //           res.data.data.childList[i].count = 0;
    //           res.data.data.childList[i].select = 0;
    //         }
    //           that.setData({
    //             productDetail: res.data.data,
    //             numItem: res.data.data.childList
    //           })
    //       }
    //     })
    //   }
    // }) 
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