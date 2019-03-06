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
        /// Method returns string value of TopUpTxType object
        /// </summary>
        /// <param name="oTopUpTxType">TopUpTxType object to create string from</param>
        /// <returns>string value containing all TopUpTxType values</returns>
        public static string TopUpTxTypeToString(this TopUpTxType oTopUpTxType)
        {
            if (oTopUpTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oTopUpTxType.StoreId},
            };

            if (oTopUpTxType.Item != null)
            {
                items.Add("MNSP", oTopUpTxType.Item.MNSP.ToString());
                items.Add("MSISDN", oTopUpTxType.Item.MSISDN);
                items.Add("PaymentType", oTopUpTxType.Item.PaymentType.ToString());
            }

            return items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all properties of TopUp (TopUpType)
        /// </summary>
        /// <param name="oTopUpItems">Array containing TopUp information objects (ClickandBuyTxType, ClickandBuyData) to create string from</param>
        /// <returns>string value containing all TopUp (TopUpType) values</returns>
        public static string TopUpItemsToString(this Object[] oTopUpItems)
        {
            if (oTopUpItems == null || oTopUpItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oTopUpItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "topuptxtype":
                        {
                            result.Append(((TopUpTxType)item).TopUpTxTypeToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }
    }
}
