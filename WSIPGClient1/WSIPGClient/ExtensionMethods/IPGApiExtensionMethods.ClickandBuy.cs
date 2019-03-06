using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.ExtensionMethods
{
    public static partial class IPGApiExtensionMethods
    {
        /// <summary>
        /// Method returns string value of all values of ClickandBuyData object
        /// </summary>
        /// <param name="oClickandBuyData">ClickandBuyData object to create string from</param>
        /// <returns>string value containing all ClickandBuyData object values</returns>
        public static string ClickandBuyDataToString(this ClickandBuyData oClickandBuyData)
        {
            if (oClickandBuyData == null || oClickandBuyData.orderDetails == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "Text", oClickandBuyData.orderDetails.text},
            };

            result.Append(items.NotNullDataToString());

            var detailsItemList = oClickandBuyData.orderDetails.itemList;
            int detailNum = 0;
            if (detailsItemList != null)
            {
                foreach (var detailItem in detailsItemList)
                {
                    detailNum++;
                    result.Append("OrderDetailItem " + detailNum + " : " + Environment.NewLine + Environment.NewLine);
                    result.Append(detailItem.OrderDetailItemToString());
                }
            }

            return items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all properties of ClickandBuy (ClickandBuyTxType, ClickandBuyData)
        /// </summary>
        /// <param name="oClickandBuyItems">Array containing ClickandBuy information objects (ClickandBuyTxType, ClickandBuyData) to create string from</param>
        /// <returns>string value containing all ClickandBuy (ClickandBuyTxType, ClickandBuyData) values</returns>
        public static string ClickandBuyItemsToString(this Object[] oClickandBuyItems)
        {
            if (oClickandBuyItems == null || oClickandBuyItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oClickandBuyItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "clickandbuytxtype":
                        {
                            result.Append(((ClickandBuyTxType)item).ClickandBuyTxTypeToString());
                            break;
                        }

                    case "clickandbuydata":
                        {
                            result.Append(((ClickandBuyData)item).ClickandBuyDataToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of ClickandBuyTxType object
        /// </summary>
        /// <param name="oClickandBuyTxType">ClickandBuyTxType object to create string from</param>
        /// <returns>string value containing all ClickandBuyTxType values</returns>
        public static string ClickandBuyTxTypeToString(this ClickandBuyTxType oClickandBuyTxType)
        {
            if (oClickandBuyTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oClickandBuyTxType.StoreId},
                { "Type", oClickandBuyTxType.Type.ToString()}
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }
    }
}
