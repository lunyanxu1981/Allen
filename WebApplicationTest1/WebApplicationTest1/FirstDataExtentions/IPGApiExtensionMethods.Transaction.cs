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
        /// Method returns string value of all values of Billing object
        /// </summary>
        /// <param name="oBilling">Billing object to create string from</param>
        /// <returns>string value containing all Billing object values</returns>
        public static string BillingToString(this Billing oBilling)
        {
            if (oBilling == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
              {
                  { "CustomerID", oBilling.CustomerID},
                  { "Name", oBilling.Name},
                  { "Company", oBilling.Company},
                  { "Adress1", oBilling.Address1},
                  { "Adress2", oBilling.Address2},
                  { "City", oBilling.City},
                  { "State", oBilling.State},
                  { "Zip", oBilling.Zip},
                  { "Country", oBilling.Country},
                  { "Phone", oBilling.Phone},
                  { "Fax", oBilling.Fax},
                  { "Email", oBilling.Email},
                  { "Addrnum", oBilling.Addrnum}
              };

            return Environment.NewLine + "Billing Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of Shipping object
        /// </summary>
        /// <param name="oShipping"></param>
        /// <returns>string value containing all Shipping values</returns>
        public static string ShippingToString(this Shipping oShipping)
        {
            if (oShipping == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "Type", oShipping.Type},
                { "Name", oShipping.Name},
                { "Adress1", oShipping.Address1},
                { "Adress2", oShipping.Address2},
                { "City", oShipping.City},
                { "State", oShipping.State},
                { "Zip", oShipping.Zip},
                { "Country", oShipping.Country},
            };

            return Environment.NewLine + "Shipping Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of Payment object
        /// </summary>
        /// <param name="oPayment"></param>
        /// <returns>string value containing all Payment values</returns>
        public static string PaymentToString(this Payment oPayment)
        {
            if (oPayment == null)
            {
                return "";
            }

            StringBuilder hostedDataIds = new StringBuilder();

            if (oPayment.HostedDataID != null)
            {
                for (int i = 0; i < oPayment.HostedDataID.Length; i++)
                {
                    hostedDataIds.Append(oPayment.HostedDataID[i]);
                }
            }

            var items = new Dictionary<string, string>()
            {
                { "HostedDataID", hostedDataIds.ToString()},
                { "HostedDataStoreID", oPayment.HostedDataStoreID},
                { "ChargeTotal", oPayment.ChargeTotal.ToString()},
                { "Currency", oPayment.Currency},
                { "DeliveryAmount", oPayment.DeliveryAmount.ToString()},
                { "DeliveryAmountSpecified", oPayment.DeliveryAmountSpecified.ToString()},
                { "SubTotal", oPayment.SubTotal.ToString()},
                { "ValueAddedTax", oPayment.ValueAddedTax.ToString()},
                { "ValueAddedTaxSpecified", oPayment.ValueAddedTaxSpecified.ToString()},
                { "DeclineHostedDataDuplicates", oPayment.DeclineHostedDataDuplicates.ToString()},
                { "NumberOfInstallments", oPayment.numberOfInstallments}
            };

            return "Payment Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of ClickandBuyTxType object
        /// </summary>
        /// <param name="oClientLocale"></param>
        /// <returns>string value containing all ClickandBuyTxType values</returns>
        public static string ClientLocaleToString(this ClientLocale oClientLocale)
        {
            if (oClientLocale == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "Country", oClientLocale.Country},
                { "Language", oClientLocale.Language}
            };

            return Environment.NewLine + "ClientLocale Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of TransactionDetails object
        /// </summary>
        /// <param name="oTransactionDetails"></param>
        /// <returns>string value containing all TransactionDetails values</returns>
        public static string TransactionDetailsToString(this TransactionDetails oTransactionDetails)
        {
            if (oTransactionDetails == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "Comments", oTransactionDetails.Comments},
                { "InvoiceNumber", oTransactionDetails.InvoiceNumber},
                { "DynamicMerchantName", oTransactionDetails.DynamicMerchantName},
                { "PONumber", oTransactionDetails.PONumber},
                { "OrderId", oTransactionDetails.OrderId},
                { "Ip", oTransactionDetails.Ip},
                { "ReferenceNumber", oTransactionDetails.ReferenceNumber},
                { "TDate", oTransactionDetails.TDate},
                { "TransactionOrigin", oTransactionDetails.TransactionOrigin.ToString()},
                { "TransactionOriginSpecified", oTransactionDetails.TransactionOriginSpecified.ToString()},
                { "TerminalID", oTransactionDetails.Terminal != null ? oTransactionDetails.Terminal.TerminalID : null},
                { "OfflineApprovalType", oTransactionDetails.OfflineApprovalType.ToString()},
                { "OfflineApprovalTypeSpecified", oTransactionDetails.OfflineApprovalTypeSpecified.ToString()},
                { "InquiryRateReference", InquiryRateReferenceToString(oTransactionDetails.InquiryRateReference)},
            };

            return Environment.NewLine + "TransactionDetails Information : " + Environment.NewLine + items.NotNullDataToString();
        }
        /// <summary>
        /// Method returns string value of InquiryRateReference object
        /// </summary>
        /// <returns>string value containing all InquiryRateReference values</returns>
        public static string InquiryRateReferenceToString(this InquiryRateReference oInquiryRateReference)
        {
            if (oInquiryRateReference == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                //{ "DccApplied", oInquiryRateReference.DccApplied.ToString()},
                //{ "DccAppliedSpecified", oInquiryRateReference.DccAppliedSpecified.ToString()},
                { "InquiryRateId", oInquiryRateReference.InquiryRateId.ToString()}
            };

            return Environment.NewLine + "InquiryRateReference Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of Basket object
        /// </summary>
        /// <param name="oBasket"></param>
        /// <returns>string value containing all Basket values</returns>
        public static string BasketToString(this Basket oBasket)
        {
            if (oBasket == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "ProductStock", oBasket.ProductStock.ToString()},
                { "ProductStockSpecified", oBasket.ProductStockSpecified.ToString()}
            };

            return Environment.NewLine + "Basket Information : " + Environment.NewLine + items.NotNullDataToString() + Environment.NewLine + BasketItemToString(oBasket.Item);
        }

        /// <summary>
        /// Method returns string value of all BasketItem[] object items
        /// </summary>
        /// <param name="oBasketItem"></param>
        /// <returns>string value containing all BasketItem[] items values</returns>
        public static string BasketItemToString(this BasketItem[] oBasketItem)
        {
            if (oBasketItem == null || oBasketItem.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oBasketItem)
            {
                result.Append("BasketItem Information : " + Environment.NewLine);

                var items = new Dictionary<string, string>()
                {
                    { "Currency", item.Currency},
                    { "DeliveryAmount", item.DeliveryAmount.ToString()},
                    { "DeliveryAmountSpecified", item.DeliveryAmountSpecified.ToString()},
                    { "Description", item.Description},
                    { "ChargeTotal", item.ChargeTotal.ToString()},
                    { "ID", item.ID},
                    { "Quantity", item.Quantity.ToString()},
                    { "SubTotal", item.SubTotal.ToString()},
                    { "ValueAddedTax", item.ValueAddedTax.ToString()},
                    { "ValueAddedTaxSpecified", item.ValueAddedTaxSpecified.ToString()}
                };

                result.Append(items.NotNullDataToString() + Environment.NewLine + BasketItemOptionToString(item.Option));
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of all BasketItemOption[] object items
        /// </summary>
        /// <param name="oBasketItemOption"></param>
        /// <returns>string value containing all BasketItemOption[] items values</returns>
        private static string BasketItemOptionToString(this BasketItemOption[] oBasketItemOption)
        {
            if (oBasketItemOption == null || oBasketItemOption.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oBasketItemOption)
            {
                result.Append("BasketItemOption Information : " + Environment.NewLine);

                var items = new Dictionary<string, string>()
                {
                    { "Name", item.Name},
                    { "Choice", item.Choice}
                };

                result.Append(items.NotNullDataToString() + Environment.NewLine);
            }

            return result.ToString();
        }
    }
}
