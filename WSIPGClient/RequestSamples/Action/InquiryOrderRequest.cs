using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.RequestSamples.Action
{
    class InquiryOrderRequest
    {
        public IPGApiActionRequest InquiryOrderActionRequest{ get; set; }

        public InquiryOrderRequest()
        {
            InquiryOrder oInquiryOrder = new InquiryOrder();
            oInquiryOrder.StoreId = "4700000018";
            oInquiryOrder.OrderId = "20191288";

            ClientLocale oClientLocale = new ClientLocale();
            oClientLocale.Country = "UK";
            oClientLocale.Language = "en";
            
            WSIPGClient.WebReference.Action oAction;
            oAction = new WSIPGClient.WebReference.Action();
            oAction.Item = oInquiryOrder;
            oAction.ClientLocale = oClientLocale;

            InquiryOrderActionRequest = new IPGApiActionRequest();
            InquiryOrderActionRequest.Item = oAction;
        }
    }
}
