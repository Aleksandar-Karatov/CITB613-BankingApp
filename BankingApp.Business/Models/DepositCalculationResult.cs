using System.Collections.Generic;

namespace BankingApp.Business.Models
{
    public class DepositCalculationResult
    {
        public decimal Principal { get; set; }
        public decimal NominalRate { get; set; } // as percentage, e.g. 2.5
        public int TermMonths { get; set; }
        public string InterestType { get; set; } = "Simple";
        public string PayoutFrequency { get; set; } = "AtMaturity";
        public decimal GrossInterest { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetInterest { get; set; }
        public decimal TotalNetPayout { get; set; }
        public decimal EffectiveAnnualRate { get; set; } // ЕГЛ, as percentage, e.g. 2.52
        
        public List<DepositScheduleLine> Schedule { get; set; } = new();
    }

    public class DepositScheduleLine
    {
        public int Month { get; set; }
        public decimal StartBalance { get; set; }
        public decimal InterestEarned { get; set; }
        public decimal TaxDeducted { get; set; }
        public decimal NetInterest { get; set; }
        public decimal EndBalance { get; set; }
        public decimal CumulativePayout { get; set; }
    }
}
