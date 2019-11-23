Page({
  data: {
    con: '',
    pay: '',
    ucid: 0,
    utitle: '',
    upic: 0,
    from: '',
    gid: '',
  },
  onLoad (option) {
    const that = this;
    that.setData({
      pay: option.pay,
      ucid: option.ucid,
      utitle: option.utitle,
      upic: option.upic,
      from: option.from,
      gid: option.gid,
    });
    wx.getStorage({
      key: 'addgoods',
      success: function (res) {
        that.setData({
          con: res.data.total
        })
      },
    })
  },
  lookDetil () {
    if (this.data.from === 'tuan') {
      wx.redirectTo({
        url: '/pages/tuanOrderDetails/index?gid=' + this.data.gid,
      })
    } else {
      wx.redirectTo({
        url: '/pages/order/order'
      })
    }
  }
});
