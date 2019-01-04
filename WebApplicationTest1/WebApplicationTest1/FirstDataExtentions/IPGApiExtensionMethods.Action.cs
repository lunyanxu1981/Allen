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
        /// Method returns string value of IPGApiOrderRequest object
        /// </summary>
        /// <param name="oIPGApiActionRequest">IPGApiActionRequest object to create string from</param>
        /// <returns>string value containing all IPGApiOrderRequest values</returns>
        public static string IPGApiActionRequestToString(this IPGApiActionRequest oIPGApiActionRequest)
        {
            if (oIPGApiActionRequest.Item == null)
            {
                return "";
            }

            return oIPGApiActionRequest.Item.ActionToString();
        }

        /// <summary>
        /// Method returns string value of Action object
        /// </summary>
        /// <param name="oAction">Action object ot create string from</param>
        /// <returns>string value containing all Action values</returns>
        public static string ActionToString(this IPGWebReference.Action oAction)
        {
            if (oAction == null)
            {
                return "";
            }

            var result = new StringBuilder();

            result.Append(Environment.NewLine + "Action Information : " + Environment.NewLine);

            //ClientLocale
            result.Append(oAction.ClientLocale.ClientLocaleToString());

            switch (oAction.Item.GetType().Name.ToLower())
            {

                case "initiateclearing":
                    {
                        result.Append(((InitiateClearing)oAction.Item).InitiateClearingToString());
                        break;
                    }

                case "inquiryorder":
                    {
                        result.Append(((InquiryOrder)oAction.Item).InquiryOrderToString());
                        break;
                    }

                case "storehosteddata":
                    {
                        result.Append(((StoreHostedData)oAction.Item).StoreHostedDataToString());
                        break;
                    }

                case "recurringpayment":
                    {
                        result.Append(((RecurringPayment)oAction.Item).RecurringPaymentToString());
                        break;
                    }

                case "validate":
                    {
                        result.Append(((Validate)oAction.Item).ValidateToString());
                        break;
                    }

                case "getexternaltransactionstatus":
                    {
                        result.Append(((GetExternalTransactionStatus)oAction.Item).GetExternalTransactionStatusToString());
                        break;
                    }

                case "getexternalconsumerinformation":
                    {
                        result.Append(((GetExternalConsumerInformation)oAction.Item).GetExternalConsumerInformationToString());
                        break;
                    }

                case "sendemailnotification":
                    {
                        result.Append(((SendEMailNotification)oAction.Item).SendEMailNotificationToString());
                        break;
                    }

                case "getlasttransactions":
                    {
                        result.Append(((GetLastTransactions)oAction.Item).GetLastTransactionsToString());
                        break;
                    }

                case "manageproducts":
                    {
                        result.Append(((ManageProducts)oAction.Item).ManageProductsToString());
                        break;
                    }

                case "manageproductstock":
                    {
                        result.Append(((ManageProductStock)oAction.Item).ManageProductStockToString());
                        break;
                    }
                case "getlastorders":
                    {
                        result.Append(((GetLastOrders)oAction.Item).GetLastOrdersToString());
                        break;
                    }
                case "requestcardratefordcc":
                    {
                        result.Append(((RequestCardRateForDCC)oAction.Item).RequestCardRateForDCCToString());
                        break;
                    }
                case "requestmerchantratefordynamicpricing":
                    {
                        result.Append(((RequestMerchantRateForDynamicPricing)oAction.Item).RequestMerchantRateForDynamicPricingToString());
                        break;
                    }
            }

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of GetExternalConsumerInformation object
        /// </summary>
        /// <param name="oGetExternalConsumerInformation">GetExternalConsumerInformation object to create string from</param>
        /// <returns>string value containing all GetExternalConsumerInformation values</returns>
        public static string GetExternalConsumerInformationToString(this GetExternalConsumerInformation oGetExternalConsumerInformation)
        {
            if (oGetExternalConsumerInformation == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oGetExternalConsumerInformation.StoreId},
                { "OrderId", oGetExternalConsumerInformation.OrderId},
                { "DataProvider", oGetExternalConsumerInformation.DataProvider.ToString()},
                { "FirstName", oGetExternalConsumerInformation.FirstName},
                { "Surname", oGetExternalConsumerInformation.Surname},
                { "Birthday", oGetExternalConsumerInformation.Birthday},
                { "Street", oGetExternalConsumerInformation.Street},
                { "HouseNumber", oGetExternalConsumerInformation.HouseNumber},
                { "PostCode", oGetExternalConsumerInformation.PostCode},
                { "City", oGetExternalConsumerInformation.City},
                { "Country", oGetExternalConsumerInformation.Country},
                { "DisplayProcessorMessages", oGetExternalConsumerInformation.DisplayProcessorMessages.ToString()}
            };

            return Environment.NewLine + "GetExternalConsumerInformation Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of GetExternalTransactionStatus object
        /// </summary>
        /// <param name="oGetExternalTransactionStatus">GetExternalTransactionStatus object to create string from</param>
        /// <returns>string value containing all GetExternalTransactionStatus values</returns>
        public static string GetExternalTransactionStatusToString(this GetExternalTransactionStatus oGetExternalTransactionStatus)
        {
            if (oGetExternalTransactionStatus == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>();
            items.Add("StoreId", oGetExternalTransactionStatus.StoreId);
            if (oGetExternalTransactionStatus.Items != null && oGetExternalTransactionStatus.ItemsElementName != null)
            {
                for (int i = 0; i < oGetExternalTransactionStatus.Items.Length; i++)
                {
                    items.Add(oGetExternalTransactionStatus.ItemsElementName[i].ToString(), oGetExternalTransactionStatus.Items[i].ToString());
                }
            }

            return Environment.NewLine + "GetExternalTransactionStatus Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of InitiateClearingObject object
        /// </summary>
        /// <param name="oInitiateClearing">InitiateClearing object to create string from</param>
        /// <returns>string value containing all InitiateClearingObject values</returns>
        public static string InitiateClearingToString(this InitiateClearing oInitiateClearing)
        {
            if (oInitiateClearing == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oInitiateClearing.StoreId}
            };

            return Environment.NewLine + "InitiateClearing Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of InquiryOrder object
        /// </summary>
        /// <param name="oInquiryOrder">InquiryOrder object to create string from</param>
        /// <returns>string value containing all InquiryOrder values</returns>
        public static string InquiryOrderToString(this InquiryOrder oInquiryOrder)
        {
            if (oInquiryOrder == null)
            {
                return "";
            }

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oInquiryOrder.StoreId},
                { "OrderId", oInquiryOrder.OrderId}
            };

            return Environment.NewLine + "InquiryOrder Information : " + Environment.NewLine + items.NotNullDataToString();
        }

        /// <summary>
        /// Method returns string value of RecurringPayment object
        /// </summary>
        /// <param name="oRecurringPayment">RecurringPayment object to create string from</param>
        /// <returns>string value containing all RecurringPayment values</returns>
        public static string RecurringPaymentToString(this RecurringPayment oRecurringPayment)
        {
            if (oRecurringPayment == null)
            {
                return "";
            }

            var result = new StringBuilder();

            var items = new Dictionary<string, string>()
            {
                { "Function", oRecurringPayment.Function.ToString()},
                { "StoreId", oRecurringPayment.StoreId},
                { "OrderId", oRecurringPayment.OrderId},
                { "Comments", oRecurringPayment.Comments},
                { "InvoiceNumber", oRecurringPayment.InvoiceNumber},
                { "DynamicMerchantName", oRecurringPayment.DynamicMerchantName},
                { "PONumber", oRecurringPayment.PONumber},
                { "ReferencedOrderId", oRecurringPayment.ReferencedOrderId},
                { "Ip", oRecurringPayment.Ip},
                { "MandanteReference", oRecurringPayment.MandateReference},
                { "TransactionOrigin", oRecurringPayment.TransactionOrigin.ToString()}
            };

            result.Append(Environment.NewLine + "RecurringPayment Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());

            //CreditCard
            if (oRecurringPayment.CreditCardData != null)
            {
                result.Append(CreditCardDataToString(oRecurringPayment.CreditCardData));
            }

            //DE_DirectDebit
            if (oRecurringPayment.DE_DirectDebitData != null)
            {
                result.Append(DE_DirectDebitDataToString(oRecurringPayment.DE_DirectDebitData));
            }

            //Payment
            result.Append(PaymentToString(oRecurringPayment.Payment));
            //Billing
            result.Append(BillingToString(oRecurringPayment.Billing));
            //Shipping
            result.Append(ShippingToString(oRecurringPayment.Shipping));
            //RecurringPaymentInformation
            result.Append(RecurringPaymentInformationToString(oRecurringPayment.RecurringPaymentInformation));

            result.Append(BasketToString(oRecurringPayment.Basket));
            result.Append(CreditCard3DSecureToString(oRecurringPayment.CreditCard3DSecure));

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of Validate object
        /// </summary>
        /// <param name="oValidate">Validate object to create string from</param>
        /// <returns>string value containing all Validate values</returns>
        public static string ValidateToString(this Validate oValidate)
        {
            if (oValidate == null)
            {
                return "";
            }

            var result = new StringBuilder();
            result.Append(Environment.NewLine + "Validate Information : " + Environment.NewLine);

            var items = new Dictionary<string, string>()
            {
                { "StoreId", oValidate.StoreId},
            };

            if (oValidate.Item != null)
            {
                result.Append(ItemsToString(new object[] { oValidate.Item }));
            }

            result.Append(items.NotNullDataToString());
            //Payment
            result.Append(PaymentToString(oValidate.Payment));
            //Billing
            result.Append(BillingToString(oValidate.Billing));
            //TransactionDetails
            result.Append(TransactionDetailsToString(oValidate.TransactionDetails));

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of StoreHostedData object
        /// </summary>
        /// <param name="oStoreHostedData">StoreHostedData object to create string from</param>
        /// <returns>string value containing all StoreHostedData values</returns>
        public static string StoreHostedDataToString(this StoreHostedData oStoreHostedData)
        {
            if (oStoreHostedData == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oStoreHostedData.StoreId},
            };

            result.Append(Environment.NewLine + "StoreHostedData Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());
            //DataStorageItem
            result.Append(DataStorageItemsToString(oStoreHostedData.DataStorageItem));

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of SendEMailNotification object
        /// </summary>
        /// <param name="oSendEMailNotification">StoreHostedData object to create string from</param>
        /// <returns>string value containing all SendEMailNotification values</returns>
        public static string SendEMailNotificationToString(this SendEMailNotification oSendEMailNotification)
        {
            if (oSendEMailNotification == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oSendEMailNotification.StoreId},
                { "Email", oSendEMailNotification.Email},
                { "OrderId", oSendEMailNotification.OrderId},
                { "TDate", oSendEMailNotification.TDate}
            };

            result.Append(Environment.NewLine + "SendEMailNotification Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of GetLastTransactions object
        /// </summary>
        /// <param name="oGetLastTransactions">GetLastTransactions object to create string from</param>
        /// <returns>string value containing all GetLastTransactions values</returns>
        public static string GetLastTransactionsToString(this GetLastTransactions oGetLastTransactions)
        {
            if (oGetLastTransactions == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oGetLastTransactions.StoreId},
                { "OrderId", oGetLastTransactions.OrderId},
                { "TDate", oGetLastTransactions.TDate},
                { "Count", oGetLastTransactions.count}
            };

            result.Append(Environment.NewLine + "GetLastTransactions Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of ManageProducts object
        /// </summary>
        /// <param name="oManageProducts">ManageProducts object to create string from</param>
        /// <returns>string value containing all ManageProducts values</returns>
        public static string ManageProductsToString(this ManageProducts oManageProducts)
        {
            if (oManageProducts == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oManageProducts.StoreId},
                { "Function", oManageProducts.Function.ToString()}
            };

            result.Append(Environment.NewLine + "ManageProducts Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());
            result.Append(ProductToString(oManageProducts.Product));

            return result.ToString();
        }

        /// <summary>
        /// Method returns string value of ManageProductStock object
        /// </summary>
        /// <param name="oManageProductStock">ManageProductStock object to create string from</param>
        /// <returns>string value containing all ManageProductStock values</returns>
        public static string ManageProductStockToString(this ManageProductStock oManageProductStock)
        {
            if (oManageProductStock == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oManageProductStock.StoreId},
                { "Function", oManageProductStock.Function.ToString()}
            };

            result.Append(Environment.NewLine + "ManageProductStock Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());
            result.Append(ProductStockToString(oManageProductStock.ProductStock));

            return result.ToString();
        }

        public static string GetLastOrdersToString(this GetLastOrders oGetLastOrders)
        {
            if (oGetLastOrders == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oGetLastOrders.StoreId},
                { "OrderId", oGetLastOrders.OrderID},
                { "DateFrom", oGetLastOrders.DateFrom.ToShortDateString()},
                { "DateFromSpecified", oGetLastOrders.DateFromSpecified.ToString()},
                { "DateTo", oGetLastOrders.DateTo.ToShortDateString()},
                { "DateToSpecified", oGetLastOrders.DateToSpecified.ToString()}
            };

            result.Append(Environment.NewLine + "Get Last Orders Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());

            return result.ToString();
        }

        public static string RequestCardRateForDCCToString(this RequestCardRateForDCC oRequestCardRateForDCC)
        {
            if (oRequestCardRateForDCC == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oRequestCardRateForDCC.StoreId},
                { "BaseAmount", oRequestCardRateForDCC.BaseAmount.ToString()},
                { "BaseAmountSpecified", oRequestCardRateForDCC.BaseAmountSpecified.ToString()},
                { "BIN", oRequestCardRateForDCC.BIN.ToString()}
            };

            result.Append(Environment.NewLine + "Get Request Card Rate For DCC Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());

            return result.ToString();
        }

        public static string RequestMerchantRateForDynamicPricingToString(this RequestMerchantRateForDynamicPricing oRequestMerchantRateForDynamicPricing)
        {
            if (oRequestMerchantRateForDynamicPricing == null)
            {
                return "";
            }

            var result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "StoreId", oRequestMerchantRateForDynamicPricing.StoreId},
                { "BaseAmount", oRequestMerchantRateForDynamicPricing.BaseAmount.ToString()},
                { "BaseAmountSpecified", oRequestMerchantRateForDynamicPricing.BaseAmountSpecified.ToString()},
                { "ForeignCurrency", oRequestMerchantRateForDynamicPricing.ForeignCurrency},
            };

            result.Append(Environment.NewLine + "Request Merchant Rate For Dynamic Pricing Information : " + Environment.NewLine);
            result.Append(items.NotNullDataToString());

            return result.ToString();
        }
    }
}
