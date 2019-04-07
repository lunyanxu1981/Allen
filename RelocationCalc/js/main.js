var app = new Vue({
    el: '#appReloc',
    data : {  
        CurrentYear : new Date().getFullYear(),
        LivingAreaRaw : 0.00,
        AtticAreaRaw : 0.00,
        EvaluationPriceRaw : 49527.00,
        AveragePriceRaw: 49527.00,
        CalculatePrimaryCompensation : true,
        CalculateAwardCompensation : true,
        CalculateRelocationCompensation : true,
        CalculateBankInterest : true,
        AtticHeightFactor : 0.5,
        ConstructionAreaFactor : 1.54,
        SubsidyAreaRaw : 15,
        OwnershipFactor : 0.8,
        SubsidyFactorRaw : 0.3,
        InterestRateRaw : 0.046,
        InterestDayNumRaw : 106,
        SignupAwardOption : 45,
        SignupPerSqAwardRaw : 5000,
        SignupBasicAreaRaw: 25,
        EquipmentRelocationFeeRaw : 2000,
        RelocationPerSqFeeRaw : 24,
        RelocationMinFeeRaw : 1000,
        DecoPerSqCompensationRaw : 500,
        NoIllegalFacilityAwardRaw : 100000,
        CashPerSqCompensationRaw : 20000,
        CashMinCompensationRaw : 600000,
        TempLodgeFeeRaw : 30000,
        RelocationAwardRaw : 400000,
        RelocationBasicAreaRaw : 25,
        RelocationPerSqAwardRaw : 1000,
        ResidenceRevokeAwardRaw : 10000
    },
    methods : {
        IsValidAmount : function(newValue){
            return newValue != '' && !isNaN(newValue.replace(',',''));
        },
        ParseAmount : function(newValue){
            return parseFloat(newValue.replace(',',''));
        },
        HigherPrice : function(isStrFormat){            
            var avgPrice = isNaN(this.AveragePriceRaw) ? 0.00 : parseFloat(this.AveragePriceRaw);
            var evalPrice = isNaN(this.EvaluationPriceRaw) ? 0.00 : parseFloat(this.EvaluationPriceRaw);
            return avgPrice > evalPrice ? (isStrFormat ? avgPrice.toLocaleString() : avgPrice) : (isStrFormat ? evalPrice.toLocaleString() : evalPrice);                        
        },
        HouseCompensation : function(isStrFormat){
            return isStrFormat ? (this.HigherPrice(false) * this.ConstructionArea * this.OwnershipFactor).toLocaleString() : parseFloat(this.HigherPrice(false) * this.ConstructionArea * this.OwnershipFactor);
        },
        SubsidyCompensation : function(isStrFormat){
            return isStrFormat ? (this.AveragePriceRaw * this.ConstructionArea * this.SubsidyFactorRaw).toLocaleString() : parseFloat(this.AveragePriceRaw * this.ConstructionArea * this.SubsidyFactorRaw);
        },
        AdditionalCompensation : function(isStrFormat){
            if(this.ConstructionArea == 0) return 0;
            return isStrFormat ? (this.AveragePriceRaw * this.SubsidyAreaRaw).toLocaleString() : parseFloat(this.AveragePriceRaw * this.SubsidyAreaRaw);
        },
        PrimaryCompensation : function(isStrFormat){
            return isStrFormat ? (this.HouseCompensation(false) + this.SubsidyCompensation(false) + this.AdditionalCompensation(false)).toLocaleString()
                    :parseFloat((this.HouseCompensation(false) + this.SubsidyCompensation(false) + this.AdditionalCompensation(false)));
        },
        SignupAward : function(isStrFormat){
            if(this.ConstructionArea == 0) return 0;
            var comp = parseFloat(this.SignupAwardOption) * 10000 + ((this.ConstructionArea > this.SignupBasicAreaRaw) ? (this.ConstructionArea - this.SignupBasicAreaRaw) * this.SignupPerSqAwardRaw : 0);
            return isStrFormat ? comp.toLocaleString() : comp;
        },
        RelocationFee : function(isStrFormat) {
            if(this.ConstructionArea == 0) return 0;
            var fee = this.ConstructionArea * this.RelocationPerSqFeeRaw;
            return isStrFormat ? (fee < this.RelocationMinFeeRaw ? this.RelocationMinFeeRaw : fee).toLocaleString()
                :(fee < this.RelocationMinFeeRaw ? this.RelocationMinFeeRaw : fee);
        },
        DecoCompensation : function(isStrFormat){
            if(this.ConstructionArea == 0) return 0;
            return isStrFormat ? (this.ConstructionArea * this.DecoPerSqCompensationRaw).toLocaleString()
                : (this.ConstructionArea * this.DecoPerSqCompensationRaw);
        },
        CashCompensation : function(isStrFormat){
            if(this.ConstructionArea == 0) return 0;
            var comp = this.ConstructionArea * this.CashPerSqCompensationRaw;
            return isStrFormat ? (comp < this.CashMinCompensationRaw ? this.CashMinCompensationRaw : comp).toLocaleString()
                : (comp < this.CashMinCompensationRaw ? this.CashPerSqCompensationRaw : comp);
        },
        AwardCompensation : function(isStrFormat){
            var comp = this.ConstructionArea == 0 ? 0 : this.SignupAward(false) + this.EquipmentRelocationFeeRaw + this.RelocationFee(false) + this.DecoCompensation(false)
                + this.NoIllegalFacilityAwardRaw + this.CashCompensation(false) + this.TempLodgeFeeRaw;
            return isStrFormat ? comp.toLocaleString() : comp;
        },
        RelocationAwardTotal : function(isStrFormat) {
            if(this.ConstructionArea == 0) return 0;
            var comp = parseFloat(this.RelocationAwardRaw) + 
                ((this.ConstructionArea > this.RelocationBasicAreaRaw)?(this.ConstructionArea - this.RelocationBasicAreaRaw) * this.RelocationPerSqAwardRaw : 0);
            return isStrFormat ? comp.toLocaleString() : comp;
        },
        RelocationAwardCompensation : function(isStrFormat){
            var comp = this.ConstructionArea == 0 ? 0 : this.RelocationAwardTotal(false) + this.ResidenceRevokeAwardRaw;
            return isStrFormat ? comp.toLocaleString() : comp;
        },
        BankInterestCompensation : function(isStrFormat){
            var interest = this.ConstructionArea == 0 ? 0 : this.BankInterest(false);
            return isStrFormat ? interest.toLocaleString() : interest;
        },
        BankInterest: function(isStrFormat){
            if(this.ConstructionArea == 0) return 0;
            var interest = (this.PrimaryCompensation(false) + this.AwardCompensation(false) + this.RelocationAwardTotal(false) - this.TempLodgeFeeRaw) 
            * this.InterestRateRaw / 360.0 * this.InterestDayNumRaw;
            return isStrFormat ? interest.toLocaleString() : interest;
        },
        TotalCompensation : function(isStrFormat){
            if(this.ConstructionArea == 0) return 0;
            var comp = (this.CalculatePrimaryCompensation ? this.PrimaryCompensation(false) : 0)
                + (this.CalculateAwardCompensation ? this.AwardCompensation(false) : 0)
                + (this.CalculateRelocationCompensation ? this.RelocationAwardCompensation(false) : 0)
                + (this.CalculateBankInterest ? this.BankInterestCompensation(false) : 0);
            return isStrFormat ? comp.toLocaleString() : comp;
        }

    },
    computed : {        
        LivingArea : {            
            get : function() {
                if(this.LivingAreaRaw == 0.00){
                    return '';
                } else {
                    return this.LivingAreaRaw;
                }
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.LivingAreaRaw = this.ParseAmount(newValue);
                }else{
                    this.LivingAreaRaw = 0;
                }
            }
        },
        AtticArea : {
            get : function() {
                if(this.AtticAreaRaw == 0.00){
                    return '';
                } else {
                    return this.AtticAreaRaw;
                }
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.AtticAreaRaw = this.ParseAmount(newValue);
                }else{
                    this.AtticAreaRaw = 0;
                }
            }
        },
        ConstructionArea : {
            get : function() {
                return (parseFloat(this.LivingAreaRaw) + parseFloat(this.AtticAreaRaw) * parseFloat(this.AtticHeightFactor)) * parseFloat(this.ConstructionAreaFactor);
            }
        },
        EvaluationPrice : {
            get : function() {
                if(isNaN(this.EvaluationPriceRaw)){
                    return "0.00";
                } else {
                    return this.EvaluationPriceRaw.toLocaleString();
                }
            }, 
            set : function(newValue) {
                if(this.IsValidAmount(newValue)){
                    this.EvaluationPriceRaw = this.ParseAmount(newValue);
                }
            }
        },
        AveragePrice : {
            get : function() {
                if(isNaN(this.AveragePriceRaw)){
                    return "0.00";
                } else {
                    return this.AveragePriceRaw.toLocaleString();
                }
            }, 
            set : function(newValue) {
                if(this.IsValidAmount(newValue)){
                    this.AveragePriceRaw = this.ParseAmount(newValue);
                } 
            }
        },
        SubsidyArea : {
            get : function(){
                return (parseFloat(this.SubsidyAreaRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.SubsidyAreaRaw = this.ParseAmount(newValue);
                }
            }
        },
        SubsidyFactor : {
            get : function(){
                return (parseFloat(this.SubsidyFactorRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.SubsidyFactorRaw = this.ParseAmount(newValue);
                }
            }
        }, 
        InterestRate : {
            get : function() {
                return (parseFloat(this.InterestRateRaw) * 100).toLocaleString();
            },
            set : function(newValue) {
                if(this.IsValidAmount(newValue)){
                    this.InterestRateRaw = this.ParseAmount(newValue) / 100.00;
                }
            }
        },
        SignupPerSqAward : {
            get : function(){
                return (parseFloat(this.SignupPerSqAwardRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.SignupPerSqAwardRaw = this.ParseAmount(newValue);
                }
            }
        },
        SignupBasicArea : {
            get : function(){
                return (parseFloat(this.SignupBasicAreaRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.SignupBasicAreaRaw = this.ParseAmount(newValue);
                }
            }
        },
        EquipmentRelocationFee : {
            get : function(){
                return (parseFloat(this.EquipmentRelocationFeeRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.EquipmentRelocationFeeRaw = this.ParseAmount(newValue);
                }
            }
        },
        RelocationPerSqFee : {
            get : function(){
                return (parseFloat(this.RelocationPerSqFeeRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.RelocationPerSqFeeRaw = this.ParseAmount(newValue);
                }
            }
        },
        RelocationMinFee : {
            get : function(){
                return (parseFloat(this.RelocationMinFeeRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.RelocationMinFeeRaw = this.ParseAmount(newValue);
                }
            }
        },
        DecoPerSqCompensation : {
            get : function(){
                return (parseFloat(this.DecoPerSqCompensationRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.DecoPerSqCompensationRaw = this.ParseAmount(newValue);
                }
            }
        },
        NoIllegalFacilityAward : {
            get : function(){
                return (parseFloat(this.NoIllegalFacilityAwardRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.NoIllegalFacilityAwardRaw = this.ParseAmount(newValue);
                }
            }
        },
        CashPerSqCompensation: {
            get : function(){
                return (parseFloat(this.CashPerSqCompensationRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.CashPerSqCompensationRaw = this.ParseAmount(newValue);
                }
            }
        },
        CashMinCompensation : {
            get : function(){
                return (parseFloat(this.CashMinCompensationRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.CashMinCompensationRaw = this.ParseAmount(newValue);
                }
            }
        },
        TempLodgeFee : {
            get : function(){
                return (parseFloat(this.TempLodgeFeeRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.TempLodgeFeeRaw = this.ParseAmount(newValue);
                }
            }
        },
        RelocationAward : {
            get : function(){
                return (parseFloat(this.RelocationAwardRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.RelocationAwardRaw = this.ParseAmount(newValue);
                }
            }
        },
        RelocationBasicArea : {
            get : function(){
                return (parseFloat(this.RelocationBasicAreaRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.RelocationBasicAreaRaw = this.ParseAmount(newValue);
                }
            }
        },
        RelocationPerSqAward : {
            get : function(){
                return (parseFloat(this.RelocationPerSqAwardRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.RelocationPerSqAwardRaw = this.ParseAmount(newValue);
                }
            }
        },
        ResidenceRevokeAward : {
            get : function(){
                return (parseFloat(this.ResidenceRevokeAwardRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.ResidenceRevokeAwardRaw = this.ParseAmount(newValue);
                }
            }
        },
        InterestDayNum : {
            get : function(){
                return (parseFloat(this.InterestDayNumRaw)).toLocaleString();
            },
            set : function(newValue){
                if(this.IsValidAmount(newValue)){
                    this.InterestDayNumRaw = this.ParseAmount(newValue);
                }
            }
        }
        
    }
})