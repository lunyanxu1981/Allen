using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSIPGClient.WebReference;

namespace WSIPGClient.RequestSamples.Action
{
    class InitiateClearingRequest
    {
        public IPGApiActionRequest InitiateClearingActionRequest { get; set; }

        public InitiateClearingRequest()
        {
            InitiateClearing oInitiateClearing = new InitiateClearing();
            oInitiateClearing.StoreId = "330995000";

            ClientLocale oClientLocale = new ClientLocale();
            oClientLocale.Country = "IN";
            oClientLocale.Language = "en";

            WSIPGClient.WebReference.Action oAction = new WSIPGClient.WebReference.Action();
            oAction.Item = oInitiateClearing;
            oAction.ClientLocale = oClientLocale;

            InitiateClearingActionRequest = new IPGApiActionRequest();
            InitiateClearingActionRequest.Item = oAction;
        }
    }
}
