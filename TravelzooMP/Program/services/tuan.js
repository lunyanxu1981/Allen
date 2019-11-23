import { post } from '../utils/wxRequest';

export default app => {
  const prefix = app.data.servsers;
  return {
    getGroupProductList: (data) => post(`${prefix}/api_getGroupProductList.html`, data),
    getGroupProductDetail: (data) => post(`${prefix}/api_getGroupProductDetail.html`, data),
    getMyGroupOrderList: (data) => post(`${prefix}/api_getMyGroupOrderList.html`, data),
    getMyOneGroupOrder: (data) => post(`${prefix}/api_getMyOneGroupOrder.html`, data),
    addFeedback: (data) => post(`${prefix}/api_addFeedback.html`, data),
    getRecommendList: (data) => post(`${prefix}/api_getRecommendList.html`, data),
    getMyGroupOrderDetail: (data) => post(`${prefix}/api_getMyGroupOrderDetail.html`, data),
    getGroupWinList: (data) => post(`${prefix}/api_getGroupWinList.html`, data),
    addGroupOrder: (data) => post(`${prefix}/api_addGroupOrder.html`, data),
  }
}

// --------------------------------------------------------------------------------------------------
/**
	 * 接口名称：下团购订单
	 * URL：	/api_addGroupOrder.html
	 * 方式：	POST
	 * 参数：openid ，gid 拼团id(如果是发起者值为0)，pid 产品id，username 联系人，phone 电话，email 邮箱，formid 表单id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:无此产品，请重新下单
	 *		405:请关闭小程序重试一次
	 * 		{"status":"200","data":array}
	 */
  // 增加formid表格单子

/**
 * 接口名称：首页推荐分类列表
 * URL：	/api_getRecommendList.html
 * 方式：	POST
 * 参数：
 * 返回：200:成功
 * 		{"status":"200","data":array}
 */
// 这个接口是获取首页banner的，这是原来的一个老接口，可以直接拿过来用，得到的是一个列表，你只取里面type=4的一条值就可以，图片地址用的是httpPostImg这个字段。

/**
	 * 接口名称：获取我的团购订单详情
	 * URL：	/api_getMyGroupOrderDetail.html
	 * 方式：	POST
	 * 参数：openid ，oid 订单id
	 * 返回：200:成功
	 *		401:参数不完整
	 *		402:参数不合法
	 *		405:请关闭小程序重试一次
	 *      {"status":"200","data":array,"userList":array,"inGroup":0或1,"gid":拼团id,"winStatus":中奖情况,"qrcode":小程序码}
	 */
// 这是拼团页的接口，这里返回值中的data里是一个数组，展示这个订单的详情，含产品信息productArr。
// userList是下面的参与团购的用户列表。其中的gid=0即为团长。
// 页面上显示还差几人参团，这个数字，你直接在页面上减一下吧，data里面的productArr中的num就是这个产品限制的总团购人数，减去userList数组中的人数即为还差几人。

/**
 * 接口名称：获取参与团购的中奖名单
 * URL：	/api_getGroupWinList.html
 * 方式：	POST
 * 参数：
 * 返回：200:成功
 *      {"status":"200","data":array}
 */
// 这是拼团首页的中奖名单，用data里面的userArr中的smartPhone 这个字段就行。

// --------------------------------------------------------------------------------------------------

/**
 * 接口名称：团购产品列表
 * URL：	/api_getGroupProductList.html
 * 方式：	POST
 * 参数：p 页码(默认1)，gt 团购时间（1今日 2明日）
 * 返回：200:成功
 * 		{"status":"200","data":array}
 */

/**
 * 接口名称：获取团购产品详情
 * URL：	/api_getGroupProductDetail.html
 * 方式：	POST
 * 参数：openid pid 产品id
 * 返回：200:成功
 *		401:参数不完整
 *      {"status":"200","data":array}
 */

/**
 * 接口名称：我的团购订单列表
 * URL：	/api_getMyGroupOrderList.html
 * 方式：	POST
 * 参数：openid ，p 页码(默认1)
 * 返回：200:成功
 *		401:参数不完整
 *		405:请关闭小程序重试一次
 * 		{"status":"200","data":array}
 */

/**
 * 接口名称：获取我最近一次团购订单中的资料（下订单时读取）
 * URL：	/api_getMyOneGroupOrder.html
 * 方式：	POST
 * 参数：openid
 * 返回：200:成功
 *		401:参数不完整
 *		405:请关闭小程序重试一次
 *      {"status":"200","data":array}
 */

/**
 * 接口名称：填写反馈信息
 * URL：	/api_addFeedback.html
 * 方式：	POST
 * 参数：openid ，content 反馈内容
 * 返回：200:成功
 *		401:参数不完整
 *		405:请关闭小程序重试一次
 * 		{"status":"200","data":"OK"}
 */