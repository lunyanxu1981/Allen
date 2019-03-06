using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.RequestSamples.Order
{
    class SendEMailNotification
    {
        public IPGApiActionRequest oIPGApiActionRequest { get; set; }

        public SendEMailNotification()
        {
            

            SendEMailNotification oSendEMailNotification = new SendEMailNotification();

         //   oSendEMailNotification.StoreId = "330995001";

            InquiryOrder oInquiryOrder = new InquiryOrder();
            oInquiryOrder.StoreId = "330995001";
            oInquiryOrder.OrderId = "12345";

            ClientLocale oClientLocale = new ClientLocale();
            oClientLocale.Country = "UK";
            oClientLocale.Language = "en";

            WSIPGClient.WebReference.Action oAction;
            oAction = new WSIPGClient.WebReference.Action();
            oAction.Item = oInquiryOrder;
            oAction.ClientLocale = oClientLocale;

            oIPGApiActionRequest = new IPGApiActionRequest();
            oIPGApiActionRequest.Item = oAction;


        }
    }
}
