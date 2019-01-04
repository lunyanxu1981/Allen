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
        /// Method returns string value of all values of DE_DirectDebitData object
        /// </summary>
        /// <param name="oDE_DirectDebitData">DE_DirectDebitData object to create string from</param>
        /// <returns>string value containing all DE_DirectDebitData object values</returns>
        public static string DE_DirectDebitDataToString(this DE_DirectDebitData oDE_DirectDebitData)
        {
            if (oDE_DirectDebitData == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>();
            if (oDE_DirectDebitData.Items != null && oDE_DirectDebitData.ItemsElementName != null)
            {
                for (int i = 0; i < oDE_DirectDebitData.Items.Length; i++)
                {
                    items.Add(oDE_DirectDebitData.ItemsElementName[i].ToString(), oDE_DirectDebitData.Items[i]);
                }
            }

            items.Add("MandateReference", oDE_DirectDebitData.MandateReference);

            return items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all properties of DE_DirectDebit (DE_DirectDebitTxType, DE_DirectDebitData)
        /// </summary>
        /// <param name="oDE_DirectDebitItems">Array containing DE_DirectDebit information objects (ClickandBuyTxType, ClickandBuyData) to create string from</param>
        /// <returns>string value containing all DE_DirectDebit (DE_DirectDebitTxType, DE_DirectDebitData) values</returns>
        public static string DE_DirectDebitItemsToString(this Object[] oDE_DirectDebitItems)
        {
            if (oDE_DirectDebitItems == null || oDE_DirectDebitItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oDE_DirectDebitItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "de_directdebittxtype":
                        {
                            result.Append(((DE_DirectDebitTxType)item).DE_DirectDebitTxTypeToString());
                            break;
                        }

                    case "de_directdebitdata":
                        {
                            result.Append(((DE_DirectDebitData)item).DE_DirectDebitDataToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of DE_DirectDebitTxType object
        /// </summary>
        /// <param name="oDE_DirectDebitTxType">DE_DirectDebitTxType object to create string from</param>
        /// <returns>string value containing all DE_DirectDebitTxType values</returns>
        public static string DE_DirectDebitTxTypeToString(this DE_DirectDebitTxType oDE_DirectDebitTxType)
        {
            if (oDE_DirectDebitTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oDE_DirectDebitTxType.StoreId},
                { "Type", oDE_DirectDebitTxType.Type.ToString()},
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }
    }
}
