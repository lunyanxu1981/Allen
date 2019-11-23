const formatTime = date => {
  const year = date.getFullYear()
  const month = date.getMonth() + 1
  const day = date.getDate()
  const hour = date.getHours()
  const minute = date.getMinutes()
  const second = date.getSeconds()

  return [year, month, day].map(formatNumber).join('/') + ' ' + [hour, minute, second].map(formatNumber).join(':')
}

const formatNumber = n => {
  n = n.toString()
  return n[1] ? n : '0' + n
}


const _timeFormat = (param) => {
  return param < 10 ? '0' + param : param;
}

const _phoneTrans = (phone) => {
  if (!phone) return;
  phone = String(phone);
  if (phone.length === 11) return phone.substr(0, 3) + '****' + phone.substr(8, 3);
}

const _getSetting = that => {
  if (wx.getSetting) {
    wx.getSetting({
      success (res) {
        const auth = res.authSetting['scope.userInfo'];
        that.setData({ isAuth: auth });
      }
    })
  }
}

const _login = (e, that, link) => {
  const { rawData } = e.detail;
  const rawData_ = JSON.parse(rawData);
  return new Promise((resolve, reject) => {
    if (wx.login) {
      wx.login({
        success: res => {
          if (res.code) {
            wx.request({
              url: link + '/api_userLogin.html',
              data: {
                code: res.code
              },
              method: "POST",
              header: {
                'content-type': 'application/x-www-form-urlencoded'
              },
              success: (res) => {
                const openid_ = res.data.data;
                wx.setStorageSync('openid', openid_);
                that.setData({ openid: openid_ });
                resolve(openid_);

                const nickname = rawData_.nickName;
                const sex = rawData_.gender;
                const country = rawData_.country;
                const province = rawData_.province;
                const city = rawData_.city;
                const headimgurl = rawData_.avatarUrl;
                const encryptedData = res.encryptedData;
                const iv = res.iv;

                wx.request({
                  url: link + '/api_saveUserInfo.html',
                  data: {
                    openid: openid_,
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
          } else {
            console.log('获取用户登录态失败！' + res.errMsg)
          }
        },
        fail (e) {
          reject(e)
        }
      });
    }
  })
}

module.exports = {
  formatTime,
  _timeFormat,
  _phoneTrans,
  _getSetting,
  _login,
}
