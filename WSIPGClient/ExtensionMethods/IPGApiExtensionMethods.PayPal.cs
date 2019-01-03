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
        /// Method returns string value of all properties of PayPal (PayPalTxType)
        /// </summary>
        /// <param name="oPayPalItems">Array containing PayPal information objects (PayPalTxType) to create string from</param>
        /// <returns>string value containing all PayPal (PayPalTxType) values</returns>
        public static string PayPalItemsToString(this Object[] oPayPalItems)
        {
            if (oPayPalItems == null || oPayPalItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oPayPalItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "paypaltxtype":
                        {
                            result.Append(((PayPalTxType)item).PayPalTxTypeToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of PayPalTxType object
        /// </summary>
        /// <param name="oPayPalTxType">PayPalTxType object to create string from</param>
        /// <returns>string value containing all PayPalTxType values</returns>
        public static string PayPalTxTypeToString(this PayPalTxType oPayPalTxType)
        {
            if (oPayPalTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oPayPalTxType.StoreId},
                { "Type", oPayPalTxType.Type.ToString()}
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }
    }
}
