using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.RequestSamples.Action
{
    class CreatePaymentURLRequest
    {
        public IPGApiActionRequest CreatePaymentURLActionRequest { get; set; }

        public CreatePaymentURLRequest()
        {
            CreatePaymentURL oCreatePaymentURL = new CreatePaymentURL();
           
            
            Transaction oTransaction = new Transaction();

            PaymentUrlTxType oPaymentUrlTxType = new PaymentUrlTxType();
            oPaymentUrlTxType.StoreId = "330995000";
            oPaymentUrlTxType.Type = PaymentUrlTxTypeType.sale;

            oTransaction.Items = new Object[] { oPaymentUrlTxType };

            Payment oPayment = new Payment();
            oPayment.SubTotal = 13;
            oPayment.ChargeTotal = 13;
            oPayment.Currency = "356";

            oTransaction.Payment = oPayment;

            TransactionDetails oTransactionDetails = new TransactionDetails();
            oTransaction.TransactionDetails = oTransactionDetails;

            ClientLocale oClientLocale = new ClientLocale();
            oClientLocale.Country = "IN";
            oClientLocale.Language = "en";

            oTransaction.ClientLocale = oClientLocale;
            oCreatePaymentURL.Transaction = oTransaction;

            oCreatePaymentURL.AuthenticateTransactionSpecified = false;
            oCreatePaymentURL.ExpirationSpecified = false;

            WSIPGClient.WebReference.Action oAction = new WSIPGClient.WebReference.Action();
            oAction.Item = oCreatePaymentURL;

            CreatePaymentURLActionRequest = new IPGApiActionRequest();
            CreatePaymentURLActionRequest.Item = oAction;
        }

     
    }
}
