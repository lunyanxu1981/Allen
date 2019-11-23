var aldstat = require("./utils/ald-stat.js");
//app.js
App({
  data: {
    servsers: "https://devtravelzoo.m-int.cn",
  },
  code: null,
  onLaunch: function (options) {
    if (options && options.scene) {
      // 下一个发送的匹配就会带上微信场景信息
      // t.send(Hit) 
    }
    wx.showLoading({
      title: '正在加载',
      mask: true,
    })
    var that = this;
    var ajax = that.data.servsers;
    wx.getSystemInfo({
      success (res) {
        that.globalData.isIphoneX = res.model.indexOf('iPhone X') > -1 ? 'btn-x' : '';
      },
      fail (err) {
        console.log(err)
      }
    });
    wx.login({
      success: res => {
        if (res.code) {
          // 发起网络请求
          wx.request({
            url: ajax + '/api_userLogin.html',
            data: {
              code: res.code
            },
            method: "POST",
            header: {
              'content-type': 'application/x-www-form-urlencoded'
            },
            success: (res) => {
              wx.setStorageSync('openid', res.data.data);//存储openid 

              // wx.hideToast();//请求完毕自动隐藏
              wx.hideLoading();//请求完毕自动隐藏

              that.globalData.employId = res.employId;
              //由于这里是网络请求，可能会在 Page.onLoad 之后才返回
              // 所以此处加入 callback 以防止这种情况
              if (that.employIdCallback) {
                that.employIdCallback(res.employId);
              }




              wx.getUserInfo({
                success: function (res) {
                  var rawData = JSON.parse(res.rawData);//json字符串转换成对象
                  var openid = wx.getStorageSync('openid')
                  var nickname = rawData.nickName;
                  var sex = rawData.gender;
                  var country = rawData.country;
                  var province = rawData.province;
                  var city = rawData.city;
                  var headimgurl = rawData.avatarUrl;
                  var encryptedData = res.encryptedData;
                  var iv = res.iv;
                  // 获取网络请求
                  wx.request({
                    url: ajax + '/api_saveUserInfo.html',
                    data: {
                      openid: openid,
                      nickname: nickname,
                      sex: sex,
                      country: country,
                      province: province,
                      city: city,
                      headimgurl: headimgurl,
                      encryptedData: encryptedData,
                      iv: iv
                    },
                    method: 'POST',
                    header: {
                      "content-type": "application/x-www-form-urlencoded"
                    },
                    success: function (res) {
                    }
                  })
                }

              })


            }
          })

          that.code = res.code;
          console.log(that.code);
        } else {
          console.log('获取用户登录态失败！' + res.errMsg)
        }
      }
    });
    wx.checkSession({
      success: function () {
        //session 未过期，并且在本生命周期一直有效
      },
      fail: function () {
        //登录态过期
        wx.login() //重新登录
      }
    });


    // 展示本地存储能力
    // var logs = wx.getStorageSync('logs') || []
    // logs.unshift(Date.now())
    // wx.setStorageSync('logs', logs)

    // 登录
    // wx.login({
    //   success: res => {
    //     // 发送 res.code 到后台换取 openId, sessionKey, unionId

    //   }
    // })

    // 获取用户信息
    wx.getSetting({
      success: res => {
        if (res.authSetting['scope.userInfo']) {
          // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
          wx.getUserInfo({
            success: res => {
              // 可以将 res 发送给后台解码出 unionId
              this.globalData.userInfo = res.userInfo

              // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
              // 所以此处加入 callback 以防止这种情况
              if (this.userInfoReadyCallback) {
                this.userInfoReadyCallback(res)
              }
            }
          })
        }
      }
    })
  },

  getOpenid: function (callback) {
    var that = this;
    var ajax = that.data.servsers;
    if (this.globalData.openid) {
      callback(this.globalData.openid);
      return;
    }

    wx.login({
      success: res => {
        if (res.code) {
          // 发起网络请求
          wx.request({
            url: ajax + '/api_userLogin.html',
            data: {
              code: res.code
            },
            method: "POST",
            header: {
              'content-type': 'application/x-www-form-urlencoded'
            },
            success: (res) => {
              console.log(res.data);
              this.globalData.openid = res.data.data
              callback(this.globalData.openid);
            }
          })

          that.code = res.code;
          //console.log(that.code);
        }
      }
    });
  },

  globalData: {
    employId: '',
    userInfo: '',
    openid: ''
  }
})