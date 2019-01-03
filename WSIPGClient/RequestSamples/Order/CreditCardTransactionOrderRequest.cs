using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.RequestSamples.Order
{
    class CreditCardTransactionOrderRequest
    {
        public IPGApiOrderRequest oCreditCardTransactionOrderRequest { get; set; }

        public CreditCardTransactionOrderRequest()
        {
            Transaction oTransaction = new Transaction();

            CreditCardTxType oCreditCardTxType = new CreditCardTxType();
            oCreditCardTxType.StoreId = "330995000";
            oCreditCardTxType.Type = CreditCardTxTypeType.sale;

            CreditCardData oCreditCardData = new CreditCardData();
            oCreditCardData.Brand = CreditCardDataBrand.VISA;
            oCreditCardData.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.CardNumber, ItemsChoiceType.ExpMonth, ItemsChoiceType.ExpYear, ItemsChoiceType.CardCodeValue };
            oCreditCardData.Items = new Object[] { "403587*****4977", "12", "18", "977" };

            oTransaction.Items = new Object[] { oCreditCardTxType, oCreditCardData };

            Payment oPayment = new Payment();
            oPayment.SubTotal = 10;
            oPayment.ValueAddedTax = 2;
            oPayment.ValueAddedTaxSpecified = true;
            oPayment.DeliveryAmount = 1;
            oPayment.DeliveryAmountSpecified = true;
            oPayment.ChargeTotal = 13;
            oPayment.Currency = "978";

            oTransaction.Payment = oPayment;

            Billing oBilling = new Billing();
            oBilling.Address1 = "Address";
            oBilling.City = "City";
            oBilling.State = "State";
            oBilling.Zip = "Zip";
            oBilling.Country = "Country";

            oTransaction.Billing = oBilling;

            ClientLocale oClientLocale = new ClientLocale();
            oClientLocale.Country = "UK";
            oClientLocale.Language = "en";

            oTransaction.ClientLocale = oClientLocale;

            oCreditCardTransactionOrderRequest = new IPGApiOrderRequest();
            oCreditCardTransactionOrderRequest.Item = oTransaction;           
        }
    }
}
