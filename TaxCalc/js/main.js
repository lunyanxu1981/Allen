var app = new Vue({
    el: '#appTax',
    data:{
        Copyright: new Date().getFullYear(),
        TaxRates: [
            { Min : 0.00,      Max : 36000.00,      TaxRate : 0.03,  Deductible : 0.00 },
            { Min : 36000.01,  Max : 144000.00,     TaxRate : 0.1,   Deductible : 2520.00 },
            { Min : 144000.01, Max : 300000.00,     TaxRate : 0.2,   Deductible : 16920.00 },
            { Min : 300000.01, Max : 420000.00,     TaxRate : 0.25,  Deductible : 31920.00 },
            { Min : 420000.01, Max : 660000.00,     TaxRate : 0.3,   Deductible : 52920.00 },
            { Min : 660000.01, Max : 960000.00,     TaxRate : 0.35,  Deductible : 85920.00 },
            { Min : 960000.01, Max : 999999999.00,  TaxRate : 0.45,  Deductible : 181920.00 },
        ],
        TaxItems: [
            { 
                Id : 'divTaxJan', SalaryPlaceHolder : "一月",  
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxFeb', SalaryPlaceHolder : "二月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxMar', SalaryPlaceHolder : "三月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxApr', SalaryPlaceHolder : "四月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxMay', SalaryPlaceHolder : "五月",
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxJun', SalaryPlaceHolder : "六月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxJul', SalaryPlaceHolder : "七月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxAug', SalaryPlaceHolder : "八月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxSep', SalaryPlaceHolder : "九月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxOct', SalaryPlaceHolder : "十月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxNov', SalaryPlaceHolder : "十一月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            },
            { 
                Id : 'divTaxDec', SalaryPlaceHolder : "十二月", 
                Salary : 0, SocialBase : 0, SocialInsurance : 0, HousingFundBase : 0, 
                HousingFund : 0, HousingFundRecon : 0, ChildrenEducation : 0, ParentsSupport : 0, 
                HouseLoanOrRent : 0, ContinuousEducation : 0, AccumulativeDeductible : 0, AccumulativeSalary : 0, 
                AccumulativeTaxableSalary : 0, AccumulativePreviousTax : 0, CurrentMonthTax : 0, ActualPayment : 0
            }
        ],
        Salary:'',
        ShowNoSalaryAlert: false,
        PreviousYear: new Date().getFullYear() - 1,
        PreviousYearAvgSalary: 7132.00,
        CurrentYear: new Date().getFullYear(),
        CurrentYearAvgSalary: 7832.00,
        SuppHouseFundRate : 0,
        PreDeductible : 5000,
        ChildrenEducation : 0,
        ParentsSupport : 0,
        HouseLoanOrRent : 0,
        ContinuousEducation : 0        
    },
    methods:{
        CalculateTax(){
            if(this.ShowNoSalaryAlert==false){
                for(var i = 0; i< this.TaxItems.length; i++){
                    //Set benefit base
                    var previousMax = this.TaxItems[i].Salary > this.PreviousYearAvgSalary * 3 
                                        ? this.PreviousYearAvgSalary * 3 
                                        : this.TaxItems[i].Salary;
                    var currentMax = this.TaxItems[i].Salary > this.CurrentYearAvgSalary * 3 
                                        ? this.CurrentYearAvgSalary * 3 
                                        : this.TaxItems[i].Salary;
                    //Social insurance
                    if( i < 3 ) {
                        this.TaxItems[i].SocialBase = previousMax;
                    } else {
                        this.TaxItems[i].SocialBase = currentMax;
                    }
                    this.TaxItems[i].SocialInsurance = this.TaxItems[i].SocialBase * 0.105;

                    //House fund
                    if( i < 6 ) {
                        this.TaxItems[i].HousingFundBase = previousMax;
                    } else{
                        this.TaxItems[i].HousingFundBase = currentMax;
                    }
                    var houseFund = this.TaxItems[i].HousingFundBase * 0.07 + this.TaxItems[i].HousingFundBase * this.SuppHouseFundRate;
                    this.TaxItems[i].HousingFund = Math.ceil(houseFund);
                    this.TaxItems[i].HousingFundRecon = (Math.ceil(houseFund) - houseFund).toFixed(2); 

                    //Accumulative deductible
                    var accumulativeDeductible = this.TaxItems[i].SocialInsurance +  this.TaxItems[i].HousingFund + this.PreDeductible + 
                        parseFloat(this.TaxItems[i].ChildrenEducation) + parseFloat(this.TaxItems[i].ParentsSupport) + 
                        parseFloat(this.TaxItems[i].HouseLoanOrRent)+ parseFloat(this.TaxItems[i].ContinuousEducation);
                    if(i > 0) {
                        accumulativeDeductible += this.TaxItems[i-1].AccumulativeDeductible;
                    }
                    this.TaxItems[i].AccumulativeDeductible = accumulativeDeductible;

                    //Accumulative salary
                    var accumulativeSalary = parseFloat(this.TaxItems[i].Salary);
                    if(i > 0) {
                        accumulativeSalary += parseFloat(this.TaxItems[i-1].AccumulativeSalary);
                    }
                    this.TaxItems[i].AccumulativeSalary = accumulativeSalary;

                    //Accumulative taxalbe salary
                    this.TaxItems[i].AccumulativeTaxableSalary = this.TaxItems[i].AccumulativeSalary - this.TaxItems[i].AccumulativeDeductible;

                    //Accumulative prevlously taxed amount
                    if(i > 0){
                        this.TaxItems[i].AccumulativePreviousTax = this.TaxItems[i-1].AccumulativePreviousTax + this.TaxItems[i].CurrentMonthTax;
                    }

                    //Current month tax
                    var currentMonthTax = 0;
                    for(var j = 0; j < this.TaxRates.length; j++){
                        if(this.TaxItems[i].AccumulativeTaxableSalary >= this.TaxRates[j].Min && this.TaxItems[i].AccumulativeTaxableSalary <= this.TaxRates[j].Max){
                            currentMonthTax = this.TaxItems[i].AccumulativeTaxableSalary * this.TaxRates[j].TaxRate - this.TaxRates[j].Deductible;
                            break;
                        }
                    }
                    if(currentMonthTax - this.TaxItems[i].AccumulativePreviousTax > 0 ){
                        this.TaxItems[i].CurrentMonthTax = currentMonthTax - this.TaxItems[i].AccumulativePreviousTax;
                    }

                    this.TaxItems[i].ActualPayment = this.TaxItems[i].Salary - this.TaxItems[i].SocialInsurance - this.TaxItems[i].HousingFund - this.TaxItems[i].CurrentMonthTax;

                } 
            }else{
                for(var i = 0; i<this.TaxItems.length; i++){
                    this.TaxItems[i].Salary = 0;
                }
            }  
        },
        ApplyToWholeYear(){
            if(this.Salary =='' || isNaN(this.Salary)){
                this.ShowNoSalaryAlert=true;                
            }else{
                this.ShowNoSalaryAlert=false;                
            }
            if(this.ShowNoSalaryAlert==false){
                for(var i = 0; i< this.TaxItems.length; i++){
                    this.TaxItems[i].Salary = parseFloat(this.Salary);
                    //Additional deductibles
                    this.TaxItems[i].ChildrenEducation = this.ChildrenEducation;
                    this.TaxItems[i].ParentsSupport = this.ParentsSupport;
                    this.TaxItems[i].HouseLoanOrRent = this.HouseLoanOrRent;
                    this.TaxItems[i].ContinuousEducation = this.ContinuousEducation;
                }
            }
            this.$options.methods.CalculateTax.bind(this)();
        },
        AppendIdSelector(Id){
            return "#" + Id;
        },
        FormatCurrency(money){
            return "￥" + parseFloat(money).toFixed(2);
        },
        HideNoSalaryAlert(){
            this.ShowNoSalaryAlert = false;
        }
    },
    computed : {
        
    },
    watch:{
        TaxItems : {
            handler : function( after, before ) {
                this.$options.methods.CalculateTax.bind(this)();
            },
            deep : true
        },
        SuppHouseFundRate : {
            handler : function( after, before ) {
                this.$options.methods.CalculateTax.bind(this)();
            }
        },
        ChildrenEducation : { 
            handler : function( after, before ) {
                if(this.ShowNoSalaryAlert==false){
                    for(var i = 0; i< this.TaxItems.length; i++){
                        this.TaxItems[i].Salary = parseFloat(this.Salary);
                        this.TaxItems[i].ChildrenEducation = this.ChildrenEducation;
                    }
                }
                this.$options.methods.CalculateTax.bind(this)();
            }
        },
        ParentsSupport : {
            handler : function( after, before ) {
                if(this.ShowNoSalaryAlert==false){
                    for(var i = 0; i< this.TaxItems.length; i++){
                        this.TaxItems[i].Salary = parseFloat(this.Salary);
                        this.TaxItems[i].ParentsSupport = this.ParentsSupport;
                    }
                }
                this.$options.methods.CalculateTax.bind(this)();
            }
        },
        HouseLoanOrRent : {
            handler : function( after, before ) {
                if(this.ShowNoSalaryAlert==false){
                    for(var i = 0; i< this.TaxItems.length; i++){
                        this.TaxItems[i].Salary = parseFloat(this.Salary);
                        this.TaxItems[i].HouseLoanOrRent = this.HouseLoanOrRent;
                    }
                }
                this.$options.methods.CalculateTax.bind(this)();
            }
        },
        ContinuousEducation : {
            handler : function( after, before) {
                if(this.ShowNoSalaryAlert==false){
                    for(var i = 0; i< this.TaxItems.length; i++){
                        this.TaxItems[i].Salary = parseFloat(this.Salary);
                        this.TaxItems[i].ContinuousEducation = this.ContinuousEducation;
                    }
                }
                this.$options.methods.CalculateTax.bind(this)();
            }
        }
    }

})