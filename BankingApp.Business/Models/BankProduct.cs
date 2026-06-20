using System.ComponentModel.DataAnnotations;

namespace BankingApp.Business.Models
{
    public class BankProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Deposit"; // "Deposit", "Loan", "Credit Card", "Checking Account"

        [Required]
        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Specific fields for Deposits (Null or default if not a Deposit)
        [MaxLength(10)]
        public string Currency { get; set; } = "EUR"; // EUR, USD

        public int TermMonths { get; set; } = 12;

        public decimal InterestRate { get; set; } // e.g., 2.5 meaning 2.5%

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        [MaxLength(50)]
        public string InterestType { get; set; } = "Simple"; // "Simple" (проста лихва), "Compounded" (сложна / капитализирана)

        [MaxLength(50)]
        public string PayoutFrequency { get; set; } = "AtMaturity"; // "AtMaturity" (на падеж), "Monthly" (месечно)

        public decimal TaxRate { get; set; } = 8.0m; // Default Bulgarian tax on deposit interest is 8%
    }
}
