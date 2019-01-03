using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.RequestSamples.Order
{
    class RefundOrderRequest
    {
        public IPGApiOrderRequest oRefundOrderRequest { get; set; }

        public RefundOrderRequest()
        {
            Transaction oTransaction = new Transaction();
            CreditCardTxType oCreditCardTxType = new CreditCardTxType();  
            
                     
           oCreditCardTxType.StoreId = "330995000";
         
            oCreditCardTxType.Type = CreditCardTxTypeType.@return;
         

            oTransaction.Items = new Object[] {oCreditCardTxType};
                      

            Payment oPayment = new Payment();
            oPayment.SubTotal = 1;
            oPayment.ValueAddedTax = 0;
            oPayment.ValueAddedTaxSpecified = true;
            oPayment.DeliveryAmount = 0;
            oPayment.DeliveryAmountSpecified = true;
            oPayment.ChargeTotal = 1;
            oPayment.Currency = "356";            
            oTransaction.Payment = oPayment;



            TransactionDetails oTransactionDetails = new TransactionDetails();
            oTransactionDetails.OrderId = "123456789123456789";
            
            oTransaction.TransactionDetails = oTransactionDetails;



            oRefundOrderRequest = new IPGApiOrderRequest();
            oRefundOrderRequest.Item = oTransaction;

            
        }
    }
}
