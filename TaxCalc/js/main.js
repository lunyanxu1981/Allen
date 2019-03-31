var app = new Vue({
    el: '#app',
    data:{
        TaxItems: [
            { 
                Id : 'divTaxJan',
                SalaryPlaceHolder : "一月", 
                Salary : '',
                SocialBase : '',
                SocialInsurance : '',
                HousingFundBase : '',
                HousingFund : '',
                HousingFundRecon : '',
            },
            { 
                Id : 'divTaxFeb',
                SalaryPlaceHolder : "二月", 
                Salary : ''
            },
            { 
                Id : 'divTaxMar',
                SalaryPlaceHolder : "三月", 
                Salary : ''
            },
            { 
                Id : 'divTaxApr',
                SalaryPlaceHolder : "四月", 
                Salary : ''
            },
            { 
                Id : 'divTaxMay',
                SalaryPlaceHolder : "五月",
                Salary : ''
            },
            { 
                Id : 'divTaxJun',
                SalaryPlaceHolder : "六月", 
                Salary : ''
            },
            { 
                Id : 'divTaxJul',
                SalaryPlaceHolder : "七月", 
                Salary : ''
            },
            { 
                Id : 'divTaxAug',
                SalaryPlaceHolder : "八月", 
                Salary : ''
            },
            { 
                Id : 'divTaxSep',
                SalaryPlaceHolder : "九月", 
                Salary : ''
            },
            { 
                Id : 'divTaxOct',
                SalaryPlaceHolder : "十月", 
                Salary : ''
            },
            { 
                Id : 'divTaxNov',
                SalaryPlaceHolder : "十一月", 
                Salary : ''
            },
            { 
                Id : 'divTaxDec',
                SalaryPlaceHolder : "十二月", 
                Salary:''
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
                    //Social
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
                } 
            }else{
                for(var i = 0; i<this.TaxItems.length; i++){
                    this.TaxItems[i].Salary = '';
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
                    this.TaxItems[i].Salary = this.Salary;
                }
            }
            this.$options.methods.CalculateTax.bind(this)();
        },
        AppendIdSelector(Id){
            return "#" + Id;
        },
        FormatCurrency(money){
            return "￥" + parseFloat(money).toFixed(2).toString();
        },
        HideNoSalaryAlert(){
            this.ShowNoSalaryAlert = false;
        }
    },
    computed:{
        
    },
    watch:{
        TaxItems:{
            handler:function(after, before){
                this.$options.methods.CalculateTax.bind(this)();
            },
            deep: true
        }
    }

})