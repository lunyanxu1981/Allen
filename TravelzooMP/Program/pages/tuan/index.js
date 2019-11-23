import { _timeFormat, _getSetting, _login } from '../../utils/util';
import servicesFn from '../../services/tuan';
const app = getApp();
const openid = wx.getStorageSync('openid');
const service = servicesFn(app);
const link = app.data.servsers;

Page({
  /*** 页面的初始数据*/
  data: {
    httpPostImg: '',
    tuanOrderList: [],
    titleNum: 1,
    cardList: [],
    cardListTemp1: [],
    cardListTemp2: [],
    actEndTimeList: [],
    actEndTimeListTemp1: [],
    actEndTimeListTemp2: [],
    countDownList: [],
    page: 1,
    gt: 1,
  },

  getList (gt, page, cb) {
    const that = this;
    service.getGroupProductList({ p: page || that.data.page, gt: gt || that.data.gt })
      .then(res => {
        const { data } = res.data;
        let { cardListTemp1, cardListTemp2, actEndTimeListTemp1, actEndTimeListTemp2 } = that.data;
        let cardList = that.data.gt === 1 ? cardListTemp1 : cardListTemp2,
          actEndTimeList = that.data.gt === 1 ? actEndTimeListTemp1 : actEndTimeListTemp2;
        data.forEach((v, k) => {
          cardList.push({
            img: v.imgList[0].img_name,
            title: v.title,
            cutTime: v.to_time,
            newPrice: v.price,
            oldPrice: v.value,
            tid: v.id,
          });
          actEndTimeList.push(v.to_time)
        });
        if (that.data.gt === 1 && that.data.page === 1) {
          that.setData({ cardList, actEndTimeList, cardListTemp1: cardList, actEndTimeListTemp1: actEndTimeList }, () => cb && cb());
        } else if (that.data.gt === 2 && that.data.page === 1) {
          that.setData({ cardList, actEndTimeList, cardListTemp2: cardList, actEndTimeListTemp2: actEndTimeList }, () => cb && cb());
        } else {
          that.setData({ cardList, actEndTimeList }, () => cb && cb());
        }
      });
  },

  getUserInfo (e) {
    const that = this;
    if(!that.data.isAuth){
      _login(e, that, link)
        .then(openid => console.log(openid))
        .catch(e => console.log(e))
    }
  },
  pageInit(){
    const that = this;
    service.getRecommendList()
      .then(res => {
        const { data } = res.data;
        data.forEach(v => {
          if (v.type === '4') {
            that.setData({ httpPostImg: v.httpPostImg });
          }
        })
      });
    service.getGroupWinList()
      .then(res => {
        const { data } = res.data;
        const tuanOrderList = [];
        data.forEach(v => {
          tuanOrderList.push(v.content);
        })
        that.setData({ tuanOrderList });
      });
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad (options) {
    const that = this;
    _getSetting(that);
    that.pageInit();
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady () {
    this._tabTuan(1);
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
    const that = this;
    let { page } = that.data;
    that.setData({ page: ++page }, () => {
      that.getList();
    })
  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },

  _tabTuan (e) {
    const that = this;
    const titleNum = typeof e === 'number' ? e : Number(e.currentTarget.dataset.num);
    let cardListTemp, actEndTimeListTemp;
    that.setData({
      page: 1,
      gt: titleNum,
      titleNum
    }, () => {
      if (that.data.cardListTemp1.length > 0 && titleNum === 1 && that.data.page === 1) {
        cardListTemp = that.data.cardListTemp1;
        actEndTimeListTemp = that.data.actEndTimeListTemp1;
      } else if (that.data.cardListTemp2.length > 0 && titleNum === 2 && that.data.page === 1) {
        cardListTemp = that.data.cardListTemp2;
        actEndTimeListTemp = that.data.actEndTimeListTemp2;
      } else {
        return that.getList(titleNum, 1, () => {
          that._cutTime();
        });
      }
      that.setData({
        cardList: cardListTemp,
        actEndTimeList: actEndTimeListTemp,
      }, () => {
        that._cutTime();
      });
    })

  },

  _cutTime () {
    const that = this;
    let newTime = new Date().getTime();
    let endTimeList = that.data.actEndTimeList;
    let countDownArr = [];

    // 对结束时间进行处理渲染到页面
    endTimeList.forEach(o => {
      o = o.replace(/\-/g, '/');

      let endTime = new Date(o).getTime();
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
      countDownArr.push(`${obj.day}天 ${obj.hou}:${obj.min}:${obj.sec}`);
    })
    that.setData({ countDownList: countDownArr })
    setTimeout(that._cutTime, 1000);
  },
})