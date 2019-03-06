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
        /// Method returns string value of all values of UK_DebitCardData object
        /// </summary>
        /// <param name="oUK_DebitCardData">UK_DebitCardData object to create string from</param>
        /// <returns>string value containing all UK_DebitCardData object values</returns>
        public static string UK_DebitCardDataToString(this UK_DebitCardData oUK_DebitCardData)
        {
            if (oUK_DebitCardData == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "CardNumber", oUK_DebitCardData.CardNumber},
                { "ExpMonth", oUK_DebitCardData.ExpMonth},
                { "ExpYear", oUK_DebitCardData.ExpYear},
            };

            items.Add(oUK_DebitCardData.ItemElementName.ToString(), oUK_DebitCardData.Item);

            return items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all properties of UK_DebitCard (UK_DebitCardTxType, UK_DebitCardData)
        /// </summary>
        /// <param name="oUK_DebitCardItems">Array containing  UK_DebitCard information objects (ClickandBuyTxType, ClickandBuyData) to create string from</param>
        /// <returns>string value containing all UK_DebitCard (UK_DebitCardTxType, UK_DebitCardData)
        public static string UK_DebitCardItemsToString(this Object[] oUK_DebitCardItems)
        {
            if (oUK_DebitCardItems == null || oUK_DebitCardItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oUK_DebitCardItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "uk_debitcardcardtxtype":
                        {
                            result.Append(((UK_DebitCardTxType)item).UK_DebitCardTxTypeToString());
                            break;
                        }

                    case "uk_debitcarddata":
                        {
                            result.Append(((UK_DebitCardData)item).UK_DebitCardDataToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of UK_DebitCardTxType object
        /// </summary>
        /// <param name="oUK_DebitCardTxType">UK_DebitCardTxType object to create string from</param>
        /// <returns>string value containing all UK_DebitCardTxType values</returns>
        public static string UK_DebitCardTxTypeToString(this UK_DebitCardTxType oUK_DebitCardTxType)
        {
            if (oUK_DebitCardTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oUK_DebitCardTxType.StoreId},
                { "Type", oUK_DebitCardTxType.Type.ToString()}
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }
    }
}
