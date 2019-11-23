import servicesFn from '../../services/tuan';
import { _timeFormat, _getSetting, _login } from '../../utils/util';
const app = getApp();
const openid = wx.getStorageSync('openid');
const service = servicesFn(app);
const { windowWidth } = wx.getSystemInfoSync();
const link = app.data.servsers;

// 支付状态 0待付款 1已完成 5交易关闭 6已退款 
// 拼团状态 0拼团中 1拼团失败 2拼团成功(等待开奖) 3成功未中奖 4成功已中奖 5成功已中奖(实物)
// winStatus 同拼团状态
// inGroup 0在此团中 1不在此团中
Page({
  data: {
    tStatus: ['拼团中', '拼团失败', '拼团成功(等待开奖)', '拼团成功', '拼团成功', '拼团成功'],
    pStatus: ['待付款', '已完成', '', '', '', '交易关闭', '已退款'],
    firstImg: '',
    group_status: '0',
    pay_status: '0',
    productDetail: {},
    gid: null,
    pid: null,
    myGid: null,
    timeObj: {
      hour: 0,
      min: 0,
      second: 0,
    },
    endTime: null,
    jiangWrap: true,
    selected: 1,
    seeAllGoods: false,
    btnMsg: '展开全部',
    userList: [],
    num: 0,
    cnum: 0,
    jiangResult: null,
    shareWrap: true,
    canvasWrap: false,
    httpQrcode: null,
    shareHeight: 0,
  },
  closeJiang() {
    this.setData({
      jiangWrap: true
    })
  },
  viewResult() {
    const that = this;
    that.setData({
      jiangWrap: false,
      jiangResult: that.data.winStatus,
    });
  },
  shared() {
    const that = this;
    that.setData({
      shareWrap: false,
    });
  },
  hideShareWrap(e) {
    const that = this;
    if (e.target.id === 'modal-box2') {
      that.setData({
        shareWrap: true,
        jiangWrap: true,
      });
    }
  },
  shared1() {
    const that = this;
    that.setData({ canvasWrap: false, shareWrap: true }, () => {
      setTimeout(() => { that.onShareAppMessage() }, 6000);
    });
  },
  shared2() {
    const that = this;
    that.setData({
      shareWrap: false,
      canvasWrap: true,
    });
    that.drawImage();
  },
  selected(e) {
    const selected = Number(e.currentTarget.dataset.index) === 1 ? true : false;
    this.setData({
      selected
    });
  },
  /**
   * 画图
   */
  drawImage() {
    const that = this;
    const canvasId = 'share-canvas';
    const ctx = wx.createCanvasContext(canvasId);
    const { rHeight, rWidth, scale } = that.data;
    const firstHeight = 336 * scale;
    ctx.drawImage('../../img/shareimg.png', 0, 0, rWidth, rHeight);

    wx.downloadFile({
      url: that.data.firstImg,
      success(res) {
        const firstTemp = res.tempFilePath;
        wx.downloadFile({
          url: that.data.httpQrcode,
          success(res2) {
            const qrCode = res2.tempFilePath;

            ctx.drawImage(firstTemp, 0, 0, rWidth, firstHeight);
            const { price, value, title } = that.data.productDetail;
            ctx.setTextAlign('center');
            ctx.setFillStyle('#cc4444');
            ctx.setFontSize(48 * scale);
            ctx.fillText(price, 338 * scale, 518 * scale);
            ctx.setFontSize(28 * scale);
            ctx.setFillStyle('#6a6868');
            ctx.fillText('￥' + value, 450 * scale, 518 * scale);
            ctx.setStrokeStyle('#888888');
            ctx.moveTo(408 * scale, 508 * scale);
            ctx.lineTo(500 * scale, 508 * scale);
            ctx.fill();

            ctx.setStrokeStyle('#888888');
            ctx.setFontSize(24 * scale);
            const title1 = title.substr(0, 24);
            const title2 = title.length > 52 ? title.substring(24, 52) + '...' : title.substring(24, 52);
            ctx.fillText(title1, windowWidth / 2.4, 560 * scale);
            ctx.fillText(title2, windowWidth / 2.4, 594 * scale);

            ctx.drawImage(qrCode, (windowWidth - 150) / 2, 640 * scale, 170 * scale, 170 * scale);

            ctx.stroke();
            ctx.draw();

            that.saveImg(canvasId);
          }
        });

      }
    });

  },
  /**
   * 保存图片
   */
  saveImg(canvasId) {
    const that = this;

    wx.canvasToTempFilePath({
      canvasId,
      success(res) {
        wx.saveImageToPhotosAlbum({
          filePath: res.tempFilePath,
          success() {
            wx.showToast({
              title: '保存到相册'
            });
          }
        })
      }
    });
  },
  goPay() {
    const { pid, gid } = this.data;
    wx.navigateTo({ url: `/pages/tuanPay/index?pid=${pid}&gid=${gid}` })
  },
  viewMyTuan() {
    const { myGid } = this.data;
    wx.navigateTo({ url: `/pages/tuanOrderDetails/index?gid=${myGid}` })
  },
  goTuan() {
    wx.switchTab({ url: `/pages/tuan/index` })
  },
  goHome() {
    wx.switchTab({ url: `/pages/index/index` })
  },
  goodsmore() {
    var that = this;
    const btnMsg = that.data.seeAllGoods ? '展开全部' : '收起';
    that.setData({
      btnMsg,
      seeAllGoods: !that.data.seeAllGoods,
    })
  },

  onLoad(options) {
    // canvas info
    const [oWidth, oHeight, scale, that] = [622, 920, windowWidth / 750, this];
    const [rWidth, rHeight] = [oWidth * scale, oHeight * scale];
    const [swidth, sheight, shareScale] = [746, 215, 746 / windowWidth];
    const shareHeight = 215 / shareScale;
    let { gid, scene } = options;
    if (scene) {
      scene = decodeURIComponent(scene);
      gid = scene.split(',')[1];
    }
    const isIphoneX = app.globalData.isIphoneX;
    _getSetting(that);
    that.setData({
      rWidth,
      rHeight,
      scale,
      shareHeight,
      isIphoneX,
      gid,
    });
    that.getProductInfo(openid, gid);
  },

  getProductInfo(openid, oid) {
    const that = this;
    openid = openid || that.data.openid;

    service.getMyGroupOrderDetail({ openid, oid })
      .then(res => {
        if (res.data.status == 200) {
          const { data, userList, inGroup, winStatus, inGroupProduct, myGid } = res.data;
          const { pay_status, productArr, httpQrcode, group_status } = data;
          const n = userList.length;
          const num = productArr.num;
          const cnum = num - n;
          if (n < 5 && num >= 5) {
            userList.length = 5;
            userList.fill({}, n, 5);
          } else if (num < 5 && cnum > 0) {
            userList.length = num;
            userList.fill({}, n, num);
          }
          let { to_time } = productArr;
          to_time = to_time.replace(/\-/g, '/');
          const endTime = new Date(to_time).getTime();
          const newTime = new Date().getTime();
          const eStatus = endTime - newTime > 0 ? 1 : 0;
          let expiredStatus = 1;
          if (eStatus === 0 || productArr.status == 0) {
            expiredStatus = 0
          }

          that.setData({
            productDetail: productArr,
            firstImg: productArr.httpShareImg,
            pid: data.pid,
            group_status,
            httpQrcode,
            pay_status,
            userList,
            num,
            cnum,
            inGroup,
            httpQrcode,
            winStatus,
            inGroupProduct,
            myGid,
            expiredStatus,
          }, () => {
            that._cutTime();
          });
        }
      });
  },

  onGotUserInfo(e) {
    const that = this;
    _login(e, that, link).then(openid_ => {
      that.getProductInfo(openid_, that.data.gid);
    });
  },

  onReady: function () {

  },

  onShow: function () {

  },

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
  onShareAppMessage() {
    const that = this;
    return {
      title: '一起来拼团吧！',
      // desc: '',
      // imageUrl: '../../img/2.jpg',
      path: `/pages/tuanOrderDetails/index?gid=${that.data.gid}`,
      complete() {
        that.setData({ shareWrap: true });
      }
    }
  },
  _cutTime() {
    const that = this;
    let newTime = new Date().getTime();
    let { to_time } = that.data.productDetail;
    to_time = to_time.replace(/\-/g, '/');
    // 对结束时间进行处理渲染到页面
    let endTime = new Date(to_time).getTime();
    let obj = {};

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
    that.setData({
      endTime: `${obj.day}天 ${obj.hou}:${obj.min}:${obj.sec}`,
      timeObj: {
        day: obj.day,
        hour: obj.hou,
        min: obj.min,
        second: obj.sec,
      }
    })
    setTimeout(that._cutTime, 1000);
  },

})
