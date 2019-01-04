using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplicationTest1.IPGWebReference;

namespace WebApplicationTest1.FirstDataExtentions
{
    public static partial class IPGApiExtensionMethods
    {
        /// <summary>
        /// Method returns string value of all values of CustomerCardData object
        /// </summary>
        /// <param name="oCustomerCardData">CustomerCardData object to create string from</param>
        /// <returns>string value containing all CustomerCardData object values</returns>
        public static string CustomerCardDataToString(this CustomerCardData oCustomerCardData)
        {
            if (oCustomerCardData == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>();
            if (oCustomerCardData.Items != null && oCustomerCardData.ItemsElementName != null)
            {
                for (int i = 0; i < oCustomerCardData.Items.Length; i++)
                {
                    items.Add(oCustomerCardData.ItemsElementName[i].ToString(), oCustomerCardData.Items[i]);
                }
            }

            return items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all properties of CustomerCard (CustomerCardTxType, CustomerCardData)
        /// </summary>
        /// <param name="oCustomerCardItems">Array containing CustomerCard information objects (ClickandBuyTxType, ClickandBuyData) to create string from</param>
        /// <returns>string value containing all CustomerCard (CustomerCardTxType, CustomerCardData) values</returns>
        public static string CustomerCardItemsToString(this Object[] oCustomerCardItems)
        {
            if (oCustomerCardItems == null || oCustomerCardItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oCustomerCardItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "customercardtxtype":
                        {
                            result.Append(((CustomerCardTxType)item).CustomerCardTxTypeToString());
                            break;
                        }

                    case "customercarddata":
                        {
                            result.Append(((CustomerCardData)item).CustomerCardDataToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of ClickandBuyTxType object
        /// </summary>
        /// <param name="oCustomerCardTxType">CustomerCardTxType object to create string from</param>
        /// <returns>string value containing all ClickandBuyTxType values</returns>
        public static string CustomerCardTxTypeToString(this CustomerCardTxType oCustomerCardTxType)
        {
            if (oCustomerCardTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oCustomerCardTxType.StoreId},
                { "Type", oCustomerCardTxType.Type.ToString()}
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }
    }
}
