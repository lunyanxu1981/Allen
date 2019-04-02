var app = new Vue({
    el: '#appTax',
    data:{        
        Salary:'',
        ShowNoSalaryAlert: false,
        PreviousYear: new Date().getFullYear() - 1,
        CurrentYear: new Date().getFullYear(),
        PreviousYearAvgSalary: 7132.00,
        CurrentYearAvgSalary: 7832.00,
        SuppHouseFundRate : 0,
        PreDeductible : 5000.00,
        ChildrenEducationGlobal : 0,
        ParentsSupportGlobal : 0,
        HouseLoanOrRentGlobal : 0,
        ContinuousEducationGlobal : 0,
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
                Id : 'divTaxJan', SalaryPlaceHolder : "一月",  Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxFeb', SalaryPlaceHolder : "二月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxMar', SalaryPlaceHolder : "三月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxApr', SalaryPlaceHolder : "四月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxMay', SalaryPlaceHolder : "五月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxJun', SalaryPlaceHolder : "六月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxJul', SalaryPlaceHolder : "七月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxAug', SalaryPlaceHolder : "八月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxSep', SalaryPlaceHolder : "九月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxOct', SalaryPlaceHolder : "十月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxNov', SalaryPlaceHolder : "十一月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            },
            { 
                Id : 'divTaxDec', SalaryPlaceHolder : "十二月", Salary : 0,  ChildrenEducation : 0, ParentsSupport : 0, HouseLoanOrRent : 0, ContinuousEducation : 0
            }
        ]
          
    },
    methods:{
        VerifySalary(){
            if(this.Salary =='' || isNaN(this.Salary)){
                this.ShowNoSalaryAlert=true;                
            }else{
                this.ShowNoSalaryAlert=false;                
            }
        },
        /*
        ApplyToWholeYear(){
            this.VerifySalary();
            if(this.ShowNoSalaryAlert==false){
                for(let i = 0; i< this.TaxItems.length; i++){
                    this.TaxItems[i].Salary = parseFloat(this.Salary);
                    //Additional deductibles
                    this.TaxItems[i].ChildrenEducation = parseFloat(this.ChildrenEducationGlobal);
                    this.TaxItems[i].ParentsSupport = parseFloat(this.ParentsSupportGlobal);
                    this.TaxItems[i].HouseLoanOrRent = parseFloat(this.HouseLoanOrRentGlobal);
                    this.TaxItems[i].ContinuousEducation = parseFloat(this.ContinuousEducationGlobal);
                }
            }
        },
        */
        FormatCurrency(money){
            return "￥" + parseFloat(money).toFixed(2);
        },
        HideNoSalaryAlert(){
            this.ShowNoSalaryAlert = false;
        }
    },
    computed : {
        IdSelector(){
            return (Id) =>{
                return "#" + Id;
            }
        },
        /* Benefit calculation base on previous year average salary*/
        PreviousMax(){
            return (index) => {
                //console.log('PreviousMax('+index+') ');
                return this.TaxItems[index].Salary > this.PreviousYearAvgSalary * 3 
                        ? this.PreviousYearAvgSalary * 3 
                        : this.TaxItems[index].Salary;
            }
        },
        /* Benefit calculation base on current year average salary*/
        CurrentMax(){
            return (index) => {
                //console.log('CurrentMax('+index+') ');
                return this.TaxItems[index].Salary > this.CurrentYearAvgSalary * 3 
                    ? this.CurrentYearAvgSalary * 3 
                    : this.TaxItems[index].Salary;
            }
        },
        /* Social insurance calculation base*/
        SocialBase(){
            return (index) => {
                //console.log('SocialBase('+index+') ');
                if( index < 3 ) {
                    return this.PreviousMax(index);
                } else {
                    return this.CurrentMax(index);
                }
            }
        },
        /* Housing fund calculation base*/
        HousingFundBase(){
            return (index) => {
                //console.log('HousingFundBase('+index+') ');
                if( index < 6 ) {
                    return this.PreviousMax(index);
                } else{
                    return this.CurrentMax(index);
                }
            }
        },
        /* Social insurance amount */
        SocialInsurance() {
            return (index) => {
                //console.log('SocialInsurance('+index+') ');
                /* Retirement 8% Medical 2% Unemployment 0.5% */
                return this.SocialBase(index) * 0.105;
            }
        },
        /* Housing fund amount (primary 7% + supp 1%~5%)*/
        HousingFund(){
            return (index) => {
                //console.log('HousingFund('+index+') ');
                return this.HousingFundBase(index) * 0.07 + this.HousingFundBase(index) * this.SuppHouseFundRate;
            }
        },
        /* Return the ceiling integer amount and the reconciliation amount is reflected in HousingFundRecon */
        HousingFundCeil() {
            return (index) => {
                //console.log('HousingFundCeil('+index+') ');
                return Math.ceil(this.HousingFund(index));
            }
        },
        /* Return the housing fund reconciliation amount in string */
        HousingFundRecon(){
            return (index) => {
                //console.log('HousingFundRecon('+index+') ');
                return (this.HousingFundCeil(index) - this.HousingFund(index)).toFixed(2);
            }
        },        
        /*Accumulative deductible */
        AccumulativeDeductible() {
            return (index) => {
                let accumulativeDeductible = this.SocialInsurance(index) +  
                                             this.HousingFundCeil(index) +
                                             parseFloat(this.PreDeductible) + 
                                             parseFloat(this.TaxItems[index].ChildrenEducation) + 
                                             parseFloat(this.TaxItems[index].ParentsSupport) + 
                                             parseFloat(this.TaxItems[index].HouseLoanOrRent) + 
                                             parseFloat(this.TaxItems[index].ContinuousEducation);
                if(index > 0) {
                    accumulativeDeductible += this.AccumulativeDeductible(index - 1);
                } 
                return accumulativeDeductible;
            }
        },
        /* Accoumulative salary */
        AccumulativeSalary(){
            return (index) => {
                let accumulativeSalary = parseFloat(this.TaxItems[index].Salary);
                if(index > 0) {
                    accumulativeSalary += parseFloat(this.AccumulativeSalary(index -1));
                }
                return accumulativeSalary;
            }
        },
        /* Accoumulative taxable income */
        AccumulativeTaxableSalary(){
            return (index) => {
                return this.AccumulativeSalary(index) - this.AccumulativeDeductible(index);
            }
        },
        CurrentMonthTax(){
            return (index) => {
                let currentMonthTax = 0;
                if(this.TaxItems[index].Salary == 0){
                    return 0.00
                }
                let accumulativeTaxableSalary = this.AccumulativeTaxableSalary(index);
                for(let i = 0; i < this.TaxRates.length; i++){
                    if(accumulativeTaxableSalary >= this.TaxRates[i].Min && accumulativeTaxableSalary <= this.TaxRates[i].Max){
                        currentMonthTax = accumulativeTaxableSalary * this.TaxRates[i].TaxRate - this.TaxRates[i].Deductible;
                        break;
                    }
                }
                let accumulativePreviousTax = 0.00;
                for(let j = index - 1; j >= 0; j--){
                    accumulativePreviousTax += this.CurrentMonthTax(j);
                }
                if(currentMonthTax - accumulativePreviousTax > 0 ){
                    return currentMonthTax - accumulativePreviousTax;
                }else{
                    return 0.00;
                }
            }
        },
        AccumulativePreviousTax() {
            return (index) => {
                if(index > 0){
                    return this.AccumulativePreviousTax(index-1) + this.CurrentMonthTax(index);
                }else{
                    return 0.00;
                }

            }
        },
        ActualPayment(){
            return (index) => {
                return this.TaxItems[index].Salary - 
                        this.SocialInsurance(index) - 
                        this.HousingFundCeil(index) - 
                        this.CurrentMonthTax(index);
            }
        }
    },
    watch:{        
        Salary : {
            handler : function( after, before ){
                this.VerifySalary();
                if(this.ShowNoSalaryAlert==false){
                    for(let i = 0; i< this.TaxItems.length; i++){
                        this.TaxItems[i].Salary = parseFloat(this.Salary);
                    }
                }
            }
        },
        ChildrenEducationGlobal : { 
            handler : function( after, before ) {
                this.VerifySalary();
                if(this.ShowNoSalaryAlert==false){
                    for(let i = 0; i< this.TaxItems.length; i++){
                        this.TaxItems[i].ChildrenEducation = this.ChildrenEducationGlobal;                        
                    }
                }
                
            }
        },
        ParentsSupportGlobal : {
            handler : function( after, before ) {
                this.VerifySalary();
                if(this.ShowNoSalaryAlert==false){
                    for(let i = 0; i< this.TaxItems.length; i++){                        
                        this.TaxItems[i].ParentsSupport = this.ParentsSupportGlobal;
                    }
                }
            }
        },
        HouseLoanOrRentGlobal : {
            handler : function( after, before ) {
                this.VerifySalary();
                if(this.ShowNoSalaryAlert==false){
                    for(let i = 0; i< this.TaxItems.length; i++){                        
                        this.TaxItems[i].HouseLoanOrRent = this.HouseLoanOrRentGlobal;
                    }
                }
            }
        },
        ContinuousEducationGlobal : {
            handler : function( after, before) {
                this.VerifySalary();
                if(this.ShowNoSalaryAlert==false){
                    for(let i = 0; i< this.TaxItems.length; i++){                        
                        this.TaxItems[i].ContinuousEducation = this.ContinuousEducationGlobal;
                    }
                }
                
            }
        }
    }
})