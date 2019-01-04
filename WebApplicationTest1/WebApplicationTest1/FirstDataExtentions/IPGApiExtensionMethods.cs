using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using WebApplicationTest1.IPGWebReference;

namespace WebApplicationTest1.FirstDataExtentions
{
    public static partial class IPGApiExtensionMethods
    {
        /// <summary>
        /// Method returns string value of all Error object items
        /// </summary>
        /// <param name="oError">Array containing Error objects</param>
        /// <returns>string value containing all Error items values</returns>
        public static string ErrorToString(this Error[] oError)
        {
            if (oError == null)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var error in oError)
            {
                var items = new Dictionary<string, string>()
               {
                    { "Code", error.Code},
                    { "ErrorMessage", error.ErrorMessage}
                };

                result.Append(Environment.NewLine + items.NotNullDataToString());
            }
            return Environment.NewLine + "Error : " + Environment.NewLine + result.ToString() + Environment.NewLine;
        }

        /// <summary>
        /// Method returns string value of all TransactionValues[] object items
        /// </summary>
        /// <param name="oTransactionValues"></param>
        /// <returns>string value containing all TransactionValues[] items values</returns>
        public static string TransactionValuesToString(this TransactionValues[] oTransactionValues)
        {
            if (oTransactionValues == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();
            int transactionValuesNumber = 0;

            foreach (var item in oTransactionValues)
            {
                result.Append(String.Format(TransactionValueToString(item), ++transactionValuesNumber));
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of TransactionValues object
        /// </summary>
        /// <param name="oTransactionValues"></param>
        /// <returns>string value containing all TransactionValues values</returns>
        public static string TransactionValueToString(this TransactionValues oTransactionValues)
        {
            if (oTransactionValues == null)
            {
                return "";
            }

            var result = new StringBuilder();
            result.Append(Environment.NewLine + "TransactionValues : Transaction {0}" + Environment.NewLine);

            //IPGApi order response
            result.Append(IPGApiOrderResponseToString(oTransactionValues.IPGApiOrderResponse));
            //Client locale
            result.Append(ClientLocaleToString(oTransactionValues.ClientLocale));
            //Transaction values items
            result.Append(ItemsToString(oTransactionValues.Items));
            //Payment
            result.Append(PaymentToString(oTransactionValues.Payment));
            //Billing
            result.Append(BillingToString(oTransactionValues.Billing));
            //Shipping
            result.Append(ShippingToString(oTransactionValues.Shipping));
            //Transaction details
            result.Append(TransactionDetailsToString(oTransactionValues.TransactionDetails));

            var items = new Dictionary<string, string>()
            {
                { "ReceiptNumber", oTransactionValues.ReceiptNumber},
                { "ResponseCode", oTransactionValues.ResponseCode},
                { "TraceNumber", oTransactionValues.TraceNumber},
                { "TransactionState", oTransactionValues.TransactionState},
                { "UserID", oTransactionValues.UserID},
                { "SubmissionComponent", oTransactionValues.SubmissionComponent}
            };

            result.Append(items.NotNullDataToString());

            //DE Direct Debit
            result.Append(DE_DirectDebitTxTypeToString(oTransactionValues.GiroPayTxType));
            result.Append(DE_DirectDebitDataToString(oTransactionValues.GiroPayData));
            //Basket
            result.Append(BasketToString(oTransactionValues.Basket));

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of all DataStorageItem[] object items
        /// </summary>
        /// <param name="oDataStorageItems"></param>
        /// <returns>string value containing all DataStorageItem[] items values</returns>
        public static string DataStorageItemsToString(this DataStorageItem[] oDataStorageItems)
        {
            if (oDataStorageItems == null || oDataStorageItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();

            foreach (var item in oDataStorageItems)
            {
                result.Append("DataStorageItem Information : " + Environment.NewLine);

                if (item.Item != null)
                {
                    result.Append(DataStorageItemToString(item) + Environment.NewLine);
                }

                var items = new Dictionary<string, string>()
                {
                    { "HostedDataID", item.HostedDataID},
                    { "DeclineHostedDataDuplicates", item.DeclineHostedDataDuplicates.ToString()}
                };

                result.Append(items.NotNullDataToString() + Environment.NewLine);
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of DataStorageItem object
        /// </summary>
        /// <param name="oDataStorageItem"></param>
        /// <returns>string value containing all DataStorageItem values</returns>
        public static string DataStorageItemToString(this DataStorageItem oDataStorageItem)
        {
            if (oDataStorageItem == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var name = oDataStorageItem.Item.GetType().Name.ToLower();

            //CreditCard
            if (name.Contains("credit"))
            {
                result.Append(CreditCardDataToString((CreditCardData)oDataStorageItem.Item));
            }

            //DE_DirectDebit
            else if (name.Contains("de_"))
            {
                result.Append(DE_DirectDebitDataToString((DE_DirectDebitData)oDataStorageItem.Item));
            }

            //OrderId
            else if (name.Contains("string"))
            {
                result.Append("OrderId = " + (String)oDataStorageItem.Item.ToString());
            }

            //Function
            else if (name.Contains("function"))
            {
                result.Append("Function = " + (String)oDataStorageItem.Item.ToString());
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of Items (e.g Transaction Item: CreditCardTxType, CreditCardData, CreditCard3DSecure) object
        /// </summary>
        /// <param name="oItems"></param>
        /// <returns>string value containing all Items (e.g Transaction Item: CreditCardTxType, CreditCardData, CreditCard3DSecure) values</returns>
        public static string ItemsToString(this Object[] oItems)
        {
            if (oItems == null || oItems.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();
            var name = oItems[0].GetType().Name.ToLower();

            //CreditCard
            if (name.Contains("credit"))
            {
                result.Append(CreditCardItemsToString(oItems.ToArray()));
            }

            //CustomerCard
            else if (name.Contains("customer"))
            {
                result.Append(CustomerCardItemsToString(oItems.ToArray()));
            }

            //DE_DirectDebit
            else if (name.Contains("de_"))
            {
                result.Append(DE_DirectDebitItemsToString(oItems.ToArray()));
            }

            //Paypal
            else if (name.Contains("pay"))
            {
                result.Append(PayPalItemsToString(oItems.ToArray()));
            }

            //TopUp
            else if (name.Contains("top"))
            {
                result.Append(TopUpItemsToString(oItems.ToArray()));
            }

            //Sofort
            else if (name.Contains("sofort"))
            {
                result.Append(SofortItemsToString(oItems.ToArray()));
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of RecurringPaymentValues object
        /// </summary>
        /// <param name="oRecurringPaymentValues"></param>
        /// <returns>string value containing all RecurringPaymentValues values</returns>
        public static string RecurringPaymentValuesToString(this RecurringPaymentValues oRecurringPaymentValues)
        {
            if (oRecurringPaymentValues == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "State", oRecurringPaymentValues.State.ToString()},
                { "CreationDate", oRecurringPaymentValues.CreationDate},
                { "FailureCount", oRecurringPaymentValues.FailureCount.ToString()},
                { "FailureCountSpecified", oRecurringPaymentValues.FailureCountSpecified.ToString()},
                { "NextAttemptDate", oRecurringPaymentValues.NextAttemptDate},
                { "RunCount", oRecurringPaymentValues.RunCount.ToString()},
                { "RunCountSpecified", oRecurringPaymentValues.RunCountSpecified.ToString()},
                { "CreditCardData", oRecurringPaymentValues.CreditCardData.CreditCardDataToString()},
                { "DE_DirectDebitData", oRecurringPaymentValues.DE_DirectDebitData.DE_DirectDebitDataToString()},
                { "HostedDataID", oRecurringPaymentValues.HostedDataID},
                { "HostedDataStoreID", oRecurringPaymentValues.HostedDataStoreID},
                { "ChargeTotal", oRecurringPaymentValues.ChargeTotal.ToString()},
                { "Currency", oRecurringPaymentValues.Currency},
                { "TransactionOrigin",oRecurringPaymentValues.TransactionOrigin.ToString()},
                { "TransactionOriginSpecified",oRecurringPaymentValues.TransactionOriginSpecified.ToString()},
                { "InvoiceNumber", oRecurringPaymentValues.InvoiceNumber},
                { "PONumber", oRecurringPaymentValues.PONumber},
                { "Comments", oRecurringPaymentValues.Comments},
                { "DeliveryAmount", oRecurringPaymentValues.DeliveryAmount.ToString()},
                { "DeliveryAmountSpecified", oRecurringPaymentValues.DeliveryAmountSpecified.ToString()},
                { "SubTotal", oRecurringPaymentValues.SubTotal.ToString()},
                { "ValueAddedTax", oRecurringPaymentValues.ValueAddedTax.ToString()},
                { "ValueAddedTaxSpecified", oRecurringPaymentValues.ValueAddedTaxSpecified.ToString()}
            };

            return Environment.NewLine + "RecurringPaymentValues Information : " + oRecurringPaymentValues.RecurringPaymentInformationToString() + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of RecurringPaymentInformation object
        /// </summary>
        /// <param name="oRecurringPaymentInformation"></param>
        /// <returns>string value containing all RecurringPaymentInformation values</returns>
        public static string RecurringPaymentInformationToString(this RecurringPaymentInformation oRecurringPaymentInformation)
        {
            if (oRecurringPaymentInformation == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "InstallmentCount", oRecurringPaymentInformation.InstallmentCount},
                { "InstallmentFrequency", oRecurringPaymentInformation.InstallmentFrequency},
                { "InstallmentPeriod", oRecurringPaymentInformation.InstallmentPeriod.ToString()},
                { "InstallmentPeriodSpecified", oRecurringPaymentInformation.InstallmentPeriodSpecified.ToString()},
                { "MaximumFailures", oRecurringPaymentInformation.MaximumFailures},
                { "RecurringStartDate", oRecurringPaymentInformation.RecurringStartDate}
            };

            return Environment.NewLine + "RecurringPaymentInformation Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method is used to get string of keys and values contained in Dictionary. Only not null values are added to the result string
        /// </summary>
        /// <param name="items">Dictionary containing value name as key and then its value of some Object </param>
        /// <returns>string containing all not null values and their associated keys</returns>
        public static string NotNullDataToString(this Dictionary<string, string> items)
        {
            var result = new StringBuilder();
            foreach (var name in items.Keys)
            {
                if (items[name] != null)
                {
                    result.Append(name + " = " + items[name] + Environment.NewLine);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of Product object
        /// </summary>
        /// <param name="oProducts">Product object to create string from</param>
        /// <returns>string value containing all Product values</returns>
        public static string ProductToString(this Product[] oProducts)
        {
            if (oProducts == null || oProducts.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();
            foreach (var product in oProducts)
            {
                var items = new Dictionary<string, string>()
            {
                { "Currency", product.Currency},
                { "DeliveryAmount", product.DeliveryAmount.ToString()},
                { "DeliveryAmountSpecified", product.DeliveryAmountSpecified.ToString()},
                { "Description", product.Description},
                { "ChargeTotal", product.ChargeTotal.ToString()},
                { "OfferEnds", product.OfferEnds.ToString()},
                { "OfferEndsSpecified", product.OfferEndsSpecified.ToString()},
                { "OfferStarts", product.OfferStarts.ToString()},
                { "OfferStartsSpecified", product.OfferStartsSpecified.ToString()},
                { "ProductID", product.ProductID.ToString()},
                { "SubTotal", product.SubTotal.ToString()},
                { "ValueAddedTax", product.ValueAddedTax.ToString()},
                { "ValueAddedTaxSpecified", product.ValueAddedTaxSpecified.ToString()}
            };

                result.Append(Environment.NewLine + "Product Information : " + Environment.NewLine);
                result.Append(items.NotNullDataToString() + Environment.NewLine);
                result.Append(ProductChoiceToString(product.Choice));
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of ProductStock object
        /// </summary>
        /// <param name="oProductStock">ProductStock object to create string from</param>
        /// <returns>string value containing all ProductStock values</returns>
        public static string ProductStockToString(this ProductStock[] oProductStock)
        {
            if (oProductStock == null || oProductStock.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();
            foreach (var product in oProductStock)
            {
                var items = new Dictionary<string, string>()
            {
                { "ProductID", product.ProductID},
                { "Quantity", product.Quantity}
            };

                result.Append(Environment.NewLine + "ProductStock Information : " + Environment.NewLine);
                result.Append(items.NotNullDataToString() + Environment.NewLine);
                result.Append(ProductChoiceToString(product.Choice));
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of ProductChoice object
        /// </summary>
        /// <param name="oProductChoice">ProductChoice object to create string from</param>
        /// <returns>string value containing all ProductChoice values</returns>
        public static string ProductChoiceToString(this ProductChoice[] oProductChoice)
        {
            if (oProductChoice == null || oProductChoice.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();
            foreach (var productChoice in oProductChoice)
            {
                var items = new Dictionary<string, string>()
            {
                { "Name", productChoice.Name},
                { "OptionName", productChoice.OptionName},
            };

                result.Append(Environment.NewLine + "ProductChoice Information : " + Environment.NewLine);
                result.Append(items.NotNullDataToString() + Environment.NewLine);
            }

            return result.ToString();
        }

        public static String OrderValuesToString(this OrderValueType[] oOrderValueType)
        {
            if (oOrderValueType == null || oOrderValueType.Count() == 0)
            {
                return "";
            }

            var result = new StringBuilder();
            int transactionValuesNumber = 0;
            foreach (var valueType in oOrderValueType)
            {
                var items = new Dictionary<string, string>()
            {
                { "OrderId", valueType.OrderId},
                { "OrderDate", valueType.OrderDate.ToShortDateString()},
                { "MandateReference", valueType.MandateReference},
            };

                result.Append(Environment.NewLine + "Order Values Information : " + Environment.NewLine);
                result.Append(items.NotNullDataToString() + Environment.NewLine);
                result.Append(valueType.Billing.BillingToString());
                result.Append(valueType.Shipping.ShippingToString());
                result.Append(Environment.NewLine + "Order " + ++transactionValuesNumber + valueType.TransactionValues.TransactionValuesToString() + Environment.NewLine);
                result.Append(valueType.Basket.BasketToString());
            }

            return result.ToString();
        }

        private static String InquiryRateTypeToString(this InquiryRateType oInquiryRateType, String inquiryRateTypeInformationType)
        {
            if (oInquiryRateType == null)
            {
                return "";
            }

            var result = new StringBuilder();

            var items = new Dictionary<string, string>()
            {
                { "DccApplied", oInquiryRateType.DccApplied.ToString()},
                { "DccAppliedSpecified", oInquiryRateType.DccAppliedSpecified.ToString()},
                { "DccOffered", oInquiryRateType.DccOffered.ToString()},
                { "DccOfferedSpecified", oInquiryRateType.DccOfferedSpecified.ToString()},
                { "ExchangeRate", oInquiryRateType.ExchangeRate.ToString()},
                { "ExchangeRateSpecified", oInquiryRateType.ExchangeRateSpecified.ToString()},
                { "ExchangeRateSourceName", oInquiryRateType.ExchangeRateSourceName},
                { "ExchangeRateSourceTimestamp", oInquiryRateType.ExchangeRateSourceTimestamp.ToString()},
                { "ExchangeRateSourceTimestampSpecified", oInquiryRateType.ExchangeRateSourceTimestampSpecified.ToString()},
                { "ExpirationTimestamp", oInquiryRateType.ExpirationTimestamp.ToString()},
                { "ExpirationTimestampSpecified", oInquiryRateType.ExpirationTimestampSpecified.ToString()},
                { "ForeignAmount", oInquiryRateType.ForeignAmount.ToString()},
                { "ForeignAmountSpecified", oInquiryRateType.ForeignAmountSpecified.ToString()},
                { "ForeignCurrencyCode", oInquiryRateType.ForeignCurrencyCode},
                { "InquiryRateId", oInquiryRateType.InquiryRateId.ToString()},
                { "MarginRatePercentage", oInquiryRateType.MarginRatePercentage.ToString()},
                { "MarginRatePercentageSpecified", oInquiryRateType.MarginRatePercentageSpecified.ToString()},
            };

            result.Append(Environment.NewLine + inquiryRateTypeInformationType + Environment.NewLine + "Inquiry Rate Type Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString() + Environment.NewLine);

            return result.ToString();
        }

        public static String MerchantRateForDynamicPricingToString(this InquiryRateType oInquiryRateType, String inquiryRateTypeInformationType)
        {
            return InquiryRateTypeToString(oInquiryRateType, inquiryRateTypeInformationType);
        }

        public static String CardRafeForDCCToString(this InquiryRateType oInquiryRateType, String inquiryRateTypeInformationType)
        {
            return InquiryRateTypeToString(oInquiryRateType, inquiryRateTypeInformationType);
        }
    }
}
