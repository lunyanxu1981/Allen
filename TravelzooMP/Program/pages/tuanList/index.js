import servicesFn from '../../services/tuan';
const app = getApp();
const service = servicesFn(app);
const openid = wx.getStorageSync('openid');

// 支付状态 0待付款 1已完成 5交易关闭 6已退款 
// 拼团状态 0拼团中 1拼团失败 2拼团成功(等待开奖) 3成功未中奖 4成功已中奖 5成功已中奖(实物)
Page({
  data: {
    p: 1, // pageNum
    tStatus: ['拼团中', '拼团失败', '等待开奖', '立即开奖', '立即开奖', '立即开奖'],
    pStatus: ['待付款', '已完成', '', '', '', '交易关闭', '已退款'],
    tuanList: [],
    stateArr: [],
  },

  getList () {
    const that = this;
    service.getMyGroupOrderList({ openid, p: that.data.p })
      .then(res => {
        if(res.data.status == 200){
          const { data } = res.data;
          const stateArr = [];
          data.forEach((v, k) => {
            stateArr.push(v.group_status === 0 ? 1 : 2);
          });
          that.setData({ tuanList: data, stateArr });
        }
      });
  },

  /**
   * 生命周期函数--监听页面加载
   */

  onLoad: function () {
    this.getList();
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
    const that = this;
    const p = that.data.p + 1;
    that.setData({ p }, () => {
      this.getList();
    })
  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },

})