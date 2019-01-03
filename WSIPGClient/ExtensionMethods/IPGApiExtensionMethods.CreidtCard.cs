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
        /// Method returns string value of all values of CreditCardData object
        /// </summary>
        /// <param name="oCreditCardData">CreditCardData object to create string from</param>
        /// <returns>string value containing all CreditCardData object values</returns>
        public static string CreditCardDataToString(this CreditCardData oCreditCardData)
        {
            if (oCreditCardData == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>();
            if (oCreditCardData.Items != null && oCreditCardData.ItemsElementName != null)
            {
                for (int i = 0; i < oCreditCardData.Items.Length; i++)
                {
                    items.Add(oCreditCardData.ItemsElementName[i].ToString(), oCreditCardData.Items[i].ToString());
                }
            }
            items.Add("BrandSpecified", oCreditCardData.BrandSpecified.ToString());

            return  items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all values of CreditCard3DSecure object
        /// </summary>
        /// <param name="oCreditCard3DSecure">CreditCard3DSecure object to create string from</param>
        /// <returns>string value containing all CreditCard3DSecure object values</returns>
        public static string CreditCard3DSecureToString(this CreditCard3DSecure oCreditCard3DSecure)
        {
            if (oCreditCard3DSecure == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "PayerAuthenticationResponse", oCreditCard3DSecure.PayerAuthenticationResponse.ToString()},
                { "PayerAuthenticationResponseSpecified", oCreditCard3DSecure.PayerAuthenticationResponseSpecified.ToString()},
                { "AuthenticationValue", oCreditCard3DSecure.AuthenticationValue},
                { "VerificationResponse", oCreditCard3DSecure.VerificationResponse.ToString()},
                { "XID", oCreditCard3DSecure.XID}
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all properties of CreditCard (CreditCardTxType, CreditCardData, CreditCard3DSecure)
        /// </summary>
        /// <param name="oCreditCardItems">Array containing CreditCard information objects (CreditCardTxType, CreditCardData, CreditCard3DSecure) to create string from</param>
        /// <returns>string value containing all CreditCard (CreditCardTxType, CreditCardData, CreditCard3DSecure) values</returns>
        public static string CreditCardItemsToString(this Object[] oCreditCardItems)
        {
            if (oCreditCardItems == null || oCreditCardItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oCreditCardItems)
            {
                switch (item.GetType().Name.ToLower())
                {
                    case "creditcardtxtype":
                        {
                            result.Append(((CreditCardTxType)item).CreditCardTxTypeToString());
                            break;
                        }
                    case "creditcarddata":
                        {
                            result.Append(((CreditCardData)item).CreditCardDataToString());
                            break;
                        }
                    case "creditcard3dsecure":
                        {
                            result.Append(((CreditCard3DSecure)item).CreditCard3DSecureToString());
                            break;
                        }

                    case "mcc6012details":
                        {
                            result.Append(((MCC6012Details)item).MCC6012DetailsToString());
                            break;
                        }

                    case "transactioncardfunction":
                        {
                            result.Append(((CardFunctionType)item).ToString());
                            break;
                        }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of CreditCardTxType object
        /// </summary>
        /// <param name="oCreditCardTxType">CreditCardTxType object to create string from</param>
        /// <returns>string value containing all CreditCardTxType values</returns>
        public static string CreditCardTxTypeToString(this CreditCardTxType oCreditCardTxType)
        {
            if (oCreditCardTxType == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oCreditCardTxType.StoreId},
                { "Type", oCreditCardTxType.Type.ToString()}
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of MCC6012Details object
        /// </summary>
        /// <param name="oMCC6012Details">CreditCardTxType object to create string from</param>
        /// <returns>string value containing all MCC6012Details values</returns>
        public static string MCC6012DetailsToString(this MCC6012Details oMCC6012Details)
        {
            if (oMCC6012Details == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "AccountFirst6", oMCC6012Details.AccountFirst6},
                { "AccountLast4", oMCC6012Details.AccountLast4},
                { "BirthDate", oMCC6012Details.BirthDate},
                { "PostCode", oMCC6012Details.PostCode},
                { "Surname", oMCC6012Details.Surname},
            };

            return items.NotNullDataToString() + Environment.NewLine;
        }
    }
}
