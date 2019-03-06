using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.RequestSamples.Action
{    
    class ValidateRequest
    {
        public IPGApiActionRequest ValidateActionRequest { get; set; }

        public ValidateRequest()
        {
            Validate oValidate = new Validate();
            oValidate.StoreId = "330995000";            

            CreditCardData oCreditCardData = new CreditCardData();
            oCreditCardData.Brand = CreditCardDataBrand.VISA;
            oCreditCardData.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.CardNumber, ItemsChoiceType.ExpMonth, ItemsChoiceType.ExpYear, ItemsChoiceType.CardCodeValue };
            oCreditCardData.Items = new Object[] { "4035874000424977", "12", "18", "977" };

            oValidate.Item = oCreditCardData;

            Payment oPayment = new Payment();
            oPayment.SubTotal = 10;
            oPayment.ValueAddedTax = 2;
            oPayment.DeliveryAmount = 1;
            oPayment.ChargeTotal = 13;
            oPayment.Currency = "978";

            oValidate.Payment = oPayment;

            ClientLocale oClientLocale = new ClientLocale();
            oClientLocale.Country = "UK";
            oClientLocale.Language = "en";

            WSIPGClient.WebReference.Action oAction = new WSIPGClient.WebReference.Action();
            oAction.Item = oValidate;            
            oAction.ClientLocale = oClientLocale;

            ValidateActionRequest = new IPGApiActionRequest();
            ValidateActionRequest.Item = oAction;
        }
    }
}
