import { _timeFormat } from '../../utils/util';
import servicesFn from '../../services/tuan';
const app = getApp();
const service = servicesFn(app);
const openid = wx.getStorageSync('openid');

Page({
  /**页面的初始数据*/
  data: {
    seeAllGoods: false,
    btnMsg: '展开全部',
    shopBuyCount: 0,
    box2Status: true,
    areaValue: '',
    productDetail: {
    },
    selected: true,
    selectbtn: false,
    total: 0,
    indexItem: 0,
    flag: true,
    endTime: '',
  },
  showForm () {
    this.setData({
      box2Status: false,
      areaValue: '',
    });
  },
  hideBox2 () {
    this.setData({
      box2Status: true
    });
  },
  fkSubmit () {
    const that = this;
    service.addFeedback({ openid, content: this.data.areaValue })
      .then(res => {
        if (res.data.status === 200) {
          wx.showToast({
            title: '提交成功',
            icon: 'success',
            duration: 1000
          })
          that.hideBox2();
        } else {
          wx.showToast({
            title: res.data.data,
            icon: 'none',
            duration: 1000
          })
        }
      });
  },
  inputAreaEvent (e) {
    this.setData({ areaValue: e.detail.value });
  },
  //出现
  show: function () {
    this.setData({ flag: false })
  },
  //消失
  hide: function () {
    this.setData({ flag: true })
  },

  selected: function (e) {
    const selected = Number(e.currentTarget.dataset.index) === 1 ? true : false;
    this.setData({
      selected
    });
  },
  //查看全部
  goodsmore (event) {
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
  goHome () {
    wx.switchTab({
      url: '/pages/index/index'
    })
  },
  goPay () {
    const that = this;
    const gid = that.data.gid
    if (that.data.state == 1) {
      if (gid == '0') {
        wx.navigateTo({
          url: '/pages/tuanPay/index?gid=0&pid=' + that.data.pid
        })
      } else {
        wx.navigateTo({
          url: '/pages/tuanOrderDetails/index?gid=' + gid
        })
      }
    }
  },

  calling: function (event) {
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

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    const that = this;
    const isIphoneX = app.globalData.isIphoneX;
    let { state, pid } = options;
    state = !state ? 1 : state;
    that.setData({ state, isIphoneX, pid })
    service.getGroupProductDetail({ pid, openid })
      .then(res => {
        const { data, gid } = res.data;
        let { to_time } = data;
        to_time = to_time.replace(/\-/g, '/');
        const endTime = new Date(to_time).getTime();
        const newTime = new Date().getTime();
        const expiredStatus = endTime - newTime > 0 ? 1 : 0;
        that.setData({ productDetail: data, gid, expiredStatus }, () => {
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
        day: _timeFormat(day),
        hou: _timeFormat(hou),
        min: _timeFormat(min),
        sec: _timeFormat(sec)
      }
    } else {
      obj = {
        day: '00',
        hou: '00',
        min: '00',
        sec: '00'
      }
    }
    that.setData({ endTime: `${obj.day}天 ${obj.hou}:${obj.min}:${obj.sec}` })
    setTimeout(that._cutTime, 1000);
  },
})