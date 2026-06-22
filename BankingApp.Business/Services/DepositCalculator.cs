using System;
using System.Collections.Generic;
using BankingApp.Business.Models;

namespace BankingApp.Business.Services
{
    public static class DepositCalculator
    {
        public static DepositCalculationResult Calculate(BankProduct product, decimal principal)
        {
            // 1. Validate constraints
            if (principal < product.MinAmount || principal > product.MaxAmount)
            {
                throw new ArgumentException($"Сумата за изчисление трябва да бъде между {product.MinAmount:N2} и {product.MaxAmount:N2} {product.Currency}.");
            }

            var result = new DepositCalculationResult
            {
                Principal = principal,
                NominalRate = product.InterestRate,
                TermMonths = product.TermMonths,
                InterestType = product.InterestType,
                PayoutFrequency = product.PayoutFrequency
                // Tax rate is product.TaxRate
            };

            decimal runningBalance = principal;
            decimal totalGrossInterest = 0;
            decimal totalTax = 0;
            decimal cumulativePayout = 0;

            // Interest calculation depends on Type and Payout frequency
            bool isCompounded = string.Equals(product.InterestType, "Compounded", StringComparison.OrdinalIgnoreCase);
            bool isMonthlyPayout = string.Equals(product.PayoutFrequency, "Monthly", StringComparison.OrdinalIgnoreCase);

            decimal monthlyNominalRate = (product.InterestRate / 100m) / 12m;
            decimal taxFactor = product.TaxRate / 100m;

            for (int month = 1; month <= product.TermMonths; month++)
            {
                decimal startBalance = runningBalance;
                decimal interestEarned = 0;

                if (isCompounded)
                {
                    // Compounded / Capitalized: interest is earned on the current running balance
                    interestEarned = runningBalance * monthlyNominalRate;
                }
                else
                {
                    // Simple interest: interest is earned on the initial principal
                    interestEarned = principal * monthlyNominalRate;
                }

                decimal taxDeducted = interestEarned * taxFactor;
                decimal netInterest = interestEarned - taxDeducted;

                if (isMonthlyPayout)
                {
                    // Paid out monthly — interest is withdrawn, balance stays at principal
                    cumulativePayout += netInterest;
                    runningBalance = principal;
                }
                else
                {
                    // Capitalized or paid at maturity — net interest added to balance
                    runningBalance += netInterest;
                }

                totalGrossInterest += interestEarned;
                totalTax += taxDeducted;

                result.Schedule.Add(new DepositScheduleLine
                {
                    Month = month,
                    StartBalance = Math.Round(startBalance, 2),
                    InterestEarned = Math.Round(interestEarned, 2),
                    TaxDeducted = Math.Round(taxDeducted, 2),
                    NetInterest = Math.Round(netInterest, 2),
                    EndBalance = isMonthlyPayout 
                        ? Math.Round(principal + cumulativePayout, 2) 
                        : Math.Round(runningBalance, 2),
                    CumulativePayout = Math.Round(cumulativePayout, 2)
                });
            }

            result.GrossInterest = Math.Round(totalGrossInterest, 2);
            result.TaxAmount = Math.Round(totalTax, 2);
            result.NetInterest = Math.Round(totalGrossInterest - totalTax, 2);
            result.TotalNetPayout = Math.Round(principal + result.NetInterest, 2);

            // Effective Annual Interest Rate (ЕГЛ / APR)
            // General annualized net yield formula: EGL = ((Total Net Payout / Principal) ^ (12 / Term)) - 1
            if (principal > 0 && product.TermMonths > 0)
            {
                double ratio = (double)(result.TotalNetPayout / principal);
                double exponent = 12.0 / product.TermMonths;
                double eglValue = Math.Pow(ratio, exponent) - 1.0;
                result.EffectiveAnnualRate = Math.Round((decimal)eglValue * 100m, 2);
            }
            else
            {
                result.EffectiveAnnualRate = 0;
            }

            return result;
        }
    }
}
