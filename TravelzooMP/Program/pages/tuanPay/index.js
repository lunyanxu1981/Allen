import servicesFn from '../../services/tuan';
const app = getApp();
const link = app.data.servsers;
const openid = wx.getStorageSync('openid');
const service = servicesFn(app);

Page({
  /**
   * 页面的初始数据
   */
  data: {
    agree: false,
    phone: '',
    email: '',
    username: '',
    pid: '',
    gid: '',
    array: [],
    index: 0,
    disabled: false,
    productDetail: {

    },
    endTime: '',
    con: null,
  },
  //事件处理函数
  agree: function () {
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
  goPay: function (e) {
    const that = this;
    const { formId } = e.detail;
    const { pid, username, phone, email, gid } = that.data;

    if (valid_name(that)) {
      if (valid_phone(that)) {
        if (valid_email(that)) {
          if (valid_agree(that)) {
            that.setData({
              disabled: !that.data.disabled
            })
            wx.request({
              url: link + '/api_addGroupOrder.html',
              data: {
                gid,
                formId,
                pid,
                username,
                phone,
                email,
                openid,
              },
              method: 'POST',
              header: {
                "content-type": "application/x-www-form-urlencoded"
              },
              success (res) {
                const con = that.data.productDetail.price;
                // 发起支付请求
                const data = res.data;
                const status = res.data.status;
                const oid = res.data.oid;
                if (status == 200) {
                  if (con == 0) {
                    wx.navigateTo({
                      url: '/pages/logs/logs?from=tuan&pay=' + con + '&gid=' + oid,
                    })
                  } else if (con > 0) {
                    let { timeStamp, nonceStr, signType, paySign } = data.data;
                    wx.requestPayment({
                      timeStamp,
                      nonceStr,
                      package: data.data.package,
                      signType,
                      paySign,
                      success (res) {
                        wx.navigateTo({
                          url: '/pages/logs/logs?from=tuan&pay=' + con + '&gid=' + oid,
                        })
                        return false
                      },
                      fail (res) {
                      }
                    })
                  }
                } else {
                  wx.showModal({
                    showCancel: false,
                    content: res.data.data,
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
    const isIphoneX = app.globalData.isIphoneX;
    const { pid, gid } = options;
    const gidInit = gid || 0;
    that.setData({ isIphoneX, pid, gid: gidInit });
    wx.request({
      url: link + '/api_getMyOneGroupOrder.html',
      data: {
        openid,
      },
      method: 'POST',
      header: {
        "content-type": "application/x-www-form-urlencoded"
      },
      success: function (res) {
        if (res.data.data != false) {
          that.setData({
            username: res.data.data.username,
            phone: res.data.data.phone,
            email: res.data.data.email
          })
        }
      }
    });

    service.getGroupProductDetail({ pid, openid })
      .then(res => {
        that.setData({ productDetail: res.data.data }, () => {
          that._cutTime();
        });
      });
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

  },
  _cutTime () {
    const that = this;
    let newTime = new Date().getTime();
    let { to_time } = that.data.productDetail;
    to_time = to_time.replace(/\-/g, '/');
    // 对结束时间进行处理渲染到页面
    let endTime = new Date(to_time).getTime();
    let obj = null;
    // 如果活动未结束，对时间进行处理
    if (endTime - newTime > 0) {
      let time = (endTime - newTime) / 1000;
      // 获取天、时、分、秒
      let day = parseInt(time / (60 * 60 * 24));
      let hou = parseInt(time % (60 * 60 * 24) / 3600);
      let min = parseInt(time % (60 * 60 * 24) % 3600 / 60);
      let sec = parseInt(time % (60 * 60 * 24) % 3600 % 60);
      obj = {
        // day: that._timeFormat(day),
        hou: day * 24 + hou,
        min: that._timeFormat(min),
        sec: that._timeFormat(sec)
      }
    } else {
      obj = {
        day: '00',
        hou: '00',
        min: '00',
        sec: '00'
      }
    }
    that.setData({ endTime: `${obj.hou}:${obj.min}:${obj.sec}` })
    setTimeout(that._cutTime, 1000);
  },
  _timeFormat (param) {
    return param < 10 ? '0' + param : param;
  },

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
  })
  return false
}
