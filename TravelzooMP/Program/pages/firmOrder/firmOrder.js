var app = getApp();
var link = app.data.servsers;
// var openid = wx.getStorageSync('openid');
// console.log(openid)

Page({
  /**
   * 页面的初始数据
   */
  data: {
    agree: false,
    phone: "",
    email: "",
    username: "",
    name1: '',
    num: '',
    con: '',
    cost: '',
    pid: '',
    array: [],
    index: 0,
    disabled: false,
    ucid: 0,
    utitle: '',
    upic: '',
  },
  //选择优惠券
  bindPickerChange: function (e) {
    //console.log(e)
    //console.log(this.data.array);
    var that = this;
    //console.log('picker发送选择改变，携带值为', e.detail.value);
    //console.log('ucid', this.data.array[e.detail.value].id);
    //console.log('upic', this.data.array[e.detail.value].price);
    var pic = 0;
    if (this.data.array[e.detail.value].id > 0) {
      pic = Math.round(that.data.cost * 100 - that.data.array[e.detail.value].price * 100) / 100;
    } else {
      pic = that.data.cost;
    }
    that.setData({
      index: e.detail.value,
      ucid: that.data.array[e.detail.value].id,
      upic: that.data.array[e.detail.value].price,
      utitle: that.data.array[e.detail.value].title,
      con: pic
    })
    if (pic < 0) {
      that.setData({
        con: 0,
      })
    }
    console.log(pic);
  },
  //事件处理函数
  agree: function () {
    var bol = this.data.boolean;
    this.setData({
      agree: !this.data.agree
    })
  },
  get_name: function (e) {
    let username = e.detail.value;//获取输入框中的值
    this.setData({
      username: username
    })
  },
  get_phone: function (e) {
    let phone = e.detail.value;
    this.setData({
      phone: phone
    })
  },
  get_email: function (e) {
    let email = e.detail.value;
    this.setData({
      email: email
    })
  },

  // 去支付
  goPay: function () {
    var code = app.code;
    var pid = this.data.pid;//商品Id
    var num = this.data.num;//商品数量
    var username = this.data.username;//联系人姓名
    var phone = this.data.phone;//联系人手机号
    var email = this.data.email;//邮箱
    var con = this.data.con;//价格
    var ucid = this.data.ucid;//所使用的优惠券Id
    var upic = this.data.upic;
    var utitle = this.data.utitle;
    var openid = wx.getStorageSync('openid')

    // console.log(code);
    // console.log(pid);
    // console.log(num);
    // console.log(username);
    // console.log(phone);
    // console.log(email);
    // console.log(openid);
    // console.log(con);
    // console.log(ucid);

    if (valid_name(this)) {
      if (valid_phone(this)) {
        if (valid_email(this)) {
          if (valid_agree(this)) {
            this.setData({
              disabled: !this.data.disabled
            })
            console.log(12828);
            wx.request({
              url: link + '/api_addOrder.html',
              data: {
                pid: pid,
                num: num,
                username: username,
                phone: phone,
                email: email,
                openid: openid,
                ucid: ucid
              },
              method: 'POST',
              header: {
                "content-type": "application/x-www-form-urlencoded"
              },
              success: function (res) {
                console.log(res);
                console.log(res.data);
                console.log(res.data.data);
                console.log(res.data.status);

                // 发起支付请求
                var data = res.data;
                var status = res.data.status;
                console.log(data.data.timeStamp);
                if (status == 200) {

                  if (con == 0) {
                    wx.navigateTo({
                      url: '/pages/logs/logs?pay=' + con + '&upic=' + upic + '&ucid=' + ucid + '&utitle=' + utitle
                    })
                    console.log(money);
                  } else if (con > 0) {
                    wx.requestPayment({
                      'timeStamp': data.data.timeStamp,
                      'nonceStr': data.data.nonceStr,
                      'package': data.data.package,
                      'signType': 'MD5',
                      'paySign': data.data.paySign,
                      'success': function (res) {
                        console.log("支付成功");
                        wx.navigateTo({
                          url: '/pages/logs/logs?pay=' + con + '&upic=' + upic + '&ucid=' + ucid + '&utitle=' + utitle
                        })
                        return false
                      },
                      'fail': function (res) {
                      }
                    })
                  }
                } else if (status == 402) {
                  wx.showModal({
                    showCancel: false,
                    content: '无此产品，请重新下单!',
                  })
                } else if (status == 403) {
                  wx.showModal({
                    showCancel: false,
                    content: '此产品每人限购一份',
                  })
                } else if (status == 404) {
                  wx.showModal({
                    showCancel: false,
                    content: '此产品每人限购一份，您已购买过',
                  })
                } else if (status == 405) {
                  wx.showModal({
                    showCancel: false,
                    content: '请关闭小程序重试一次',
                  })
                }

              }
            })
          }
        }
      }
    }
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;
    var openid = wx.getStorageSync('openid')
    const isIphoneX = app.globalData.isIphoneX;
    that.setData({ isIphoneX });
    wx.request({
      url: link + '/api_getMyOneOrder.html',
      data: {
        openid: openid
      },
      method: 'POST',
      header: {
        "content-type": "application/x-www-form-urlencoded"
      },
      success: function (res) {
        console.log(res.data);
        that.setData({
          username: res.data.data.username,
          phone: res.data.data.phone,
          email: res.data.data.email
        })
        console.log(that.data.username);
      }
    })
    wx.request({
      url: link + '/api_getMyCouponList.html', //我的优惠券列表
      data: {
        status: 0,
        openid: openid,
      },
      method: 'POST',
      header: {
        'content-type': 'application/x-www-form-urlencoded' // 默认值
      },
      success: function (res) {
        console.log(res.data)
        //console.log(res.data.data[0])
        //console.log(res.data.data[0].couponArr.title)
        //console.log(res.data.data[0].couponArr)
        //console.log(that.data.array);

        var newArr = [{
          "id": "0",
          "uid": "0",
          "cid": "0",
          "status": "0",
          "expiration_time": "0",
          "create_time": "0",
          "couponArr": {
            "id": "0",
            "title": "不使用优惠券",
            "desc": "",
            "price": "0",
            "create_time": "0"
          },
          "title": "不使用优惠券",
          "price": "0",
          "smartCreateTime": "",
          "smartExpirationTime": "0"
        }];

        that.setData({
          array: newArr.concat(res.data.data),
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
    var that = this;
    wx.getStorage({
      key: 'addgoods',
      success: function (res) {
        that.setData({
          name1: res.data.numItem.name,
          num: res.data.numItem.count,
          con: res.data.total,
          cost: res.data.total,
          pid: res.data.numItem.Id
        })
        console.log(res.data.numItem.Id)

      },

    })
    wx.getStorage({
      key: 'openid',
      success: function (res) {
        console.log(res.data)
        that.setData({
          openid: res.data.openid
        })
      }
    })
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

//验证手机号
function valid_phone (_this) {
  let phone = _this.data.phone;
  let reg = /^1(2|3|4|5|8|7)\d{9}$/;
  if (reg.test(phone)) {
    return true
  }
  wx.showModal({
    showCancel: false,
    content: '手机号格式不正确!',
  })
  return false
}
//验证邮箱
function valid_email (_this) {
  let email = _this.data.email;
  // let reg = /^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+\.[a-zA-Z0-9]+/;
  let reg = /\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}/;
  if (reg.test(email)) {
    return true
  }
  wx.showModal({
    showCancel: false,
    content: '邮箱格式不正确!',
  })
  return false
}
//验证姓名
function valid_name (_this) {
  let string = _this.data.username;
  console.log(string);
  if (_this.data.username) {
    return true
  }
  if (string.length == 0) {
    wx.showModal({
      showCancel: false,
      content: '姓名不能为空哦!',
    })
  }
  return false
}
//验证 是否同意协议
function valid_agree (_this) {
  if (_this.data.agree) {
    return true
  }
  wx.showModal({
    showCancel: false,
    content: '请阅读并同意《服务条款》《销售条款》《隐私权政策》',
    // success: function (res) {
    //   _this.setData({
    //     agree: true
    //   })
    // }
  })
  return false
}
