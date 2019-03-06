using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using WSIPGClient.WebReference;

namespace WSIPGClient.ExtensionMethods
{
    public static partial class IPGApiExtensionMethods
    {
        /// <summary>
        /// Method returns string value of IPGApiActionResponseObject object
        /// </summary>
        /// <param name="oIPGApiActionResponse">IPGApiActionResponse object to create string from</param>
        /// <returns>string value containing all IPGApiActionResponseObject values</returns>
        public static string IPGApiActionResponseToString(this IPGApiActionResponse oIPGApiActionResponse)
        {
            if (oIPGApiActionResponse == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();

            var items = new Dictionary<string, string>()
            {
                { "OrderId",  oIPGApiActionResponse.OrderId},
                { "successfully",  oIPGApiActionResponse.successfully.ToString()},
                { "ProcessorRequestMessage",  oIPGApiActionResponse.ProcessorRequestMessage},
                { "ProcessorResponseCode", oIPGApiActionResponse.ProcessorResponseCode},
                { "ProcessorResponseMessage", oIPGApiActionResponse.ProcessorResponseMessage},
                { "MandateReference", oIPGApiActionResponse.MandateReference},
                { "ResultInfo:MoreResultsAvailable", oIPGApiActionResponse.ResultInfo.ResultInfoToString()},
                { "paymentUrl", oIPGApiActionResponse.paymentUrl}
            };

            result.Append(items.NotNullDataToString());
            //Error message from server
            result.Append(oIPGApiActionResponse.Error.ErrorToString());
            //Data storage items
            result.Append(oIPGApiActionResponse.DataStorageItem.DataStorageItemsToString());
            //Billing
            result.Append(oIPGApiActionResponse.Billing.BillingToString());
            //Shipping
            result.Append(oIPGApiActionResponse.Shipping.ShippingToString());
            //Recurring payment information
            result.Append(oIPGApiActionResponse.RecurringPaymentInformation.RecurringPaymentValuesToString());
            //Transaction values
            result.Append(Environment.NewLine + oIPGApiActionResponse.TransactionValues.TransactionValuesToString() + Environment.NewLine);
            //Basket
            result.Append(oIPGApiActionResponse.Basket.BasketToString());
            //Product
            result.Append(oIPGApiActionResponse.Product.ProductToString());
            //ProductStock
            result.Append(oIPGApiActionResponse.ProductStock.ProductStockToString());
            //OrderValues
            result.Append(oIPGApiActionResponse.OrderValues.OrderValuesToString());
            //MerchantRateForDynamicPricing
            result.Append(oIPGApiActionResponse.MerchantRateForDynamicPricing.MerchantRateForDynamicPricingToString("Merchant Rate For Dynamic Pricing Information"));
            //CardRafeForDCC
            result.Append(oIPGApiActionResponse.CardRateForDCC.CardRafeForDCCToString("Card Rate For DCC Information"));

            return result.ToString();
        }

        private static string ResultInfoToString(this ResultInfoType oResultInfoType)
        {
            if (oResultInfoType == null)
            {
                return "";
            }

            return oResultInfoType.MoreResultsAvailable.ToString();
        }

        /// <summary>
        /// Method returns string value of IPGApiOrderResponse object
        /// </summary>
        /// <param name="oIPGApiOrderResponse">IPGApiOrderResponse object to create string from</param>
        /// <returns>string value containing all IPGApiOrderResponse values</returns>
        public static string IPGApiOrderResponseToString(this IPGApiOrderResponse oIPGApiOrderResponse)
        {
            if (oIPGApiOrderResponse == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();
            result.Append(Environment.NewLine + "IPGApiOrderResponse : " + Environment.NewLine);

            var items = new Dictionary<string, string>()
            {
                { "ApprovalCode",  oIPGApiOrderResponse.ApprovalCode},
                { "AVSResponse",  oIPGApiOrderResponse.AVSResponse},
                { "Brand",  oIPGApiOrderResponse.Brand},
                { "CommercialServiceProvider",  oIPGApiOrderResponse.CommercialServiceProvider},
                { "Country",  oIPGApiOrderResponse.Country},
                { "ErrorMessage",  oIPGApiOrderResponse.ErrorMessage},
                { "MandateReference",  oIPGApiOrderResponse.MandateReference},
                { "OrderId",  oIPGApiOrderResponse.OrderId},
                { "PayerSecurityLevel",  oIPGApiOrderResponse.PayerSecurityLevel},
                { "PaymentType",  oIPGApiOrderResponse.PaymentType},
                { "ProcessorApprovalCode",  oIPGApiOrderResponse.ProcessorApprovalCode},
                { "ProcessorCCVResponse",  oIPGApiOrderResponse.ProcessorCCVResponse},
                { "ProcessorReceiptNumber",  oIPGApiOrderResponse.ProcessorReceiptNumber},
                { "ProcessorReferenceNumber",  oIPGApiOrderResponse.ProcessorReferenceNumber},
                { "ProcessorResponseCode",  oIPGApiOrderResponse.ProcessorResponseCode},
                { "ProcessorResponseMessage",  oIPGApiOrderResponse.ProcessorResponseMessage},
                { "ProcessorTraceNumber",  oIPGApiOrderResponse.ProcessorTraceNumber},
                { "TDate",  oIPGApiOrderResponse.TDate},
                { "TDateFormatted",  oIPGApiOrderResponse.TDateFormatted},
                { "TerminalID",  oIPGApiOrderResponse.TerminalID},
                { "TransactionResult",  oIPGApiOrderResponse.TransactionResult},
                { "TransactionTime",  oIPGApiOrderResponse.TransactionTime},
                { "ReferencedTDate",  oIPGApiOrderResponse.ReferencedTDate},
            };

            result.Append(items.NotNullDataToString() + Environment.NewLine);
            result.Append(EMVCardPresentResponseToString(oIPGApiOrderResponse.EMVCardPresentResponse));
            return result.ToString();
        }

        /// <summary>
        /// Returns information from SoapException it contains some response information (Order or Action).
        /// Information is taken from MerchantException or ProcessingException.
        /// </summary>
        /// <param name="oSoapException">SoapException to get information from</param>
        /// <returns>string containing information extracted from exception</returns>
        public static string SoapExceptionResponseToString(this SoapException oSoapException)
        {
            if (oSoapException == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();
            if (oSoapException.Detail == null)
                return oSoapException.Message + Environment.NewLine;

            var DetailInnerXmlValue = (String)oSoapException.Detail.InnerXml;

            switch (oSoapException.Message.ToLower())
            {
                case "merchantexception":
                    {
                        result.Append(Environment.NewLine + "Wrong values from the Merchant" + Environment.NewLine + DetailInnerXmlValue);
                        break;
                    }
                case "processingexception":
                    {
                        result.Append(Environment.NewLine + "Error reported" + Environment.NewLine);
                        result.Append(Environment.NewLine + GetDetailInformationFromProcessingException(DetailInnerXmlValue));
                        break;
                    }
            }

            return result.ToString();
        }

        /// <summary>
        /// Gest information from SoapException : ProcessingException from property Detail
        /// </summary>
        /// <param name="Detail">string object containing XML defining SoapException detail(innerXml)</param>
        /// <returns>string containing information stored in SoapException : ProcessingException</returns>
        private static String GetDetailInformationFromProcessingException(String Detail)
        {
            if(Detail == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();
            using (XmlReader reader = XmlReader.Create(new StringReader(Detail)))
            {
                try
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                var ElementName = reader.Name.Split(':');

                                if (reader.Depth == 0) { result.Append(ElementName[1] + " : " + Environment.NewLine); break; }

                                result.Append(ElementName[1] + " = ");
                                break;

                            case XmlNodeType.Text:
                                result.Append(reader.Value + Environment.NewLine);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.Append("Exception : " + e.Message + " : " + e.StackTrace);
                }
            }
            return result.ToString();
        }

        private static String EMVCardPresentResponseToString(this EMVCardPresentResponse oEMVCardPresentResponse)
        {
            if (oEMVCardPresentResponse == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "TLVData",  oEMVCardPresentResponse.TLVData.ToString()}
            };

            result.Append(Environment.NewLine + "EMVCardPresentResponse : " + Environment.NewLine);
            result.Append(items.NotNullDataToString() + Environment.NewLine);
            result.Append(EMVCardPresentResponseDataToString(oEMVCardPresentResponse.EMVResponseData));
            return result.ToString();
        }

        private static String EMVCardPresentResponseDataToString(this EMVResponseData oEMVCardPresentResponseData)
        {
            if (oEMVCardPresentResponseData == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();
            var items = new Dictionary<string, string>()
            {
                { "IssuerAuthenticationData91",  oEMVCardPresentResponseData.IssuerAuthenticationData91.ToString()},
                { "IssuerAuthorizationResponseCode8A",  oEMVCardPresentResponseData.IssuerAuthorizationResponseCode8A.ToString()},
                { "IssuerScriptTemplate171",  oEMVCardPresentResponseData.IssuerScriptTemplate171.ToString()},
                { "IssuerScriptTemplate272",  oEMVCardPresentResponseData.IssuerScriptTemplate272.ToString()},
                { "MessageControlFieldDF4F",  oEMVCardPresentResponseData.MessageControlFieldDF4F.ToString()}
            };

            result.Append(Environment.NewLine + "EMVResponseData : " + Environment.NewLine);
            result.Append(items.NotNullDataToString() + Environment.NewLine);
            return result.ToString();
        }
    }
}
