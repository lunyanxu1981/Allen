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
        /// Method returns string value of all properties of Sofort (SofortTxType)
        /// </summary>
        /// <param name="oSofortItems">Array containing Sofort information objects (SofortTxType) to create string from</param>
        /// <returns>string value containing all Sofort (SofortTxType) values</returns>
        public static string SofortItemsToString(this Object[] oSofortItems)
        {
            if (oSofortItems == null || oSofortItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oSofortItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "soforttxtype":
                        {
                            result.Append(((SofortTxType)item).SofortTxTypeToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of SofortTxType object
        /// </summary>
        /// <param name="oSofortTxType">SofortTxType object to create string from</param>
        /// <returns>string value containing all SofortTxType values</returns>
        public static string SofortTxTypeToString(this SofortTxType oSofortTxType)
        {
            if (oSofortTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oSofortTxType.StoreId},
                { "Type", oSofortTxType.Type.ToString()}
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }
    }
}
