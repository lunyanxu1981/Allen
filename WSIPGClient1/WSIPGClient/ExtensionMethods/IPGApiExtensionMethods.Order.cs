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
        /// Method returns string value of IPGApiOrderRequest object
        /// </summary>
        /// <param name="oIPGApiOrderRequest">IPGApiOrderRequest object to create string from</param>
        /// <returns>string value containing all IPGApiOrderRequest values</returns>
        public static string IPGApiOrderRequestToString(this IPGApiOrderRequest oIPGApiOrderRequest)
        {
            if (oIPGApiOrderRequest.Item == null)
            {
                return "";
            }
            var result = new StringBuilder();

            switch (oIPGApiOrderRequest.Item.GetType().Name.ToLower())
            {
                case "transaction":
                    {
                        result.Append(((Transaction)oIPGApiOrderRequest.Item).ToString());
                        break;
                    }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of Transaction object
        /// </summary>
        /// <param name="oTransaction">Transaction object to create string from</param>
        /// <returns>string value containing all Transaction values</returns>
        public static string TransactionToString(this Transaction oTransaction)
        {
            var result = new StringBuilder();

            //Client locale
            result.Append(ClientLocaleToString(oTransaction.ClientLocale));
            //Transaction items
            result.Append(ItemsToString(oTransaction.Items));
            //Payment
            result.Append(PaymentToString(oTransaction.Payment));
            //Billing
            result.Append(BillingToString(oTransaction.Billing));
            //Shipping
            result.Append(ShippingToString(oTransaction.Shipping));
            //Transaction details
            result.Append(TransactionDetailsToString(oTransaction.TransactionDetails));
            //Basket
            result.Append(BasketToString(oTransaction.Basket));

            return result.ToString();
        }
    }
}
