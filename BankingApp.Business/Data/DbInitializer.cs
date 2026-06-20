using System;
using System.Linq;
using BankingApp.Business.Models;

namespace BankingApp.Business.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BankingDbContext context)
        {
            // Create database if it doesn't exist
            context.Database.EnsureCreated();

            // Look for any products
            if (context.BankProducts.Any())
            {
                return;   // DB has been seeded
            }

            var products = new BankProduct[]
            {
                new BankProduct
                {
                    Name = "Депозит Моят Актив (Проста лихва)",
                    Type = "Deposit",
                    BankName = "Първа Инвестиционна Банка (Fibank)",
                    Description = "Стандартен тримесечен или дванадесетмесечен депозит с атрактивна проста лихва, изплащана на падеж.",
                    Currency = "EUR",
                    TermMonths = 12,
                    InterestRate = 2.10m,
                    MinAmount = 1000m,
                    MaxAmount = 100000m,
                    InterestType = "Simple",
                    PayoutFrequency = "AtMaturity",
                    TaxRate = 8.0m
                },
                new BankProduct
                {
                    Name = "Fibank Активен Долар",
                    Type = "Deposit",
                    BankName = "Първа Инвестиционна Банка (Fibank)",
                    Description = "Депозит в щатски долари с фиксирана висока лихва за по-дълъг период.",
                    Currency = "USD",
                    TermMonths = 24,
                    InterestRate = 1.80m,
                    MinAmount = 500m,
                    MaxAmount = 50000m,
                    InterestType = "Simple",
                    PayoutFrequency = "AtMaturity",
                    TaxRate = 8.0m
                },
                new BankProduct
                {
                    Name = "Спестовен влог 'Детска мечта'",
                    Type = "Deposit",
                    BankName = "Банка ДСК",
                    Description = "Детски спестовен влог с ежемесечно капитализиране на лихвата и преференциален процент.",
                    Currency = "EUR",
                    TermMonths = 36,
                    InterestRate = 3.20m,
                    MinAmount = 100m,
                    MaxAmount = 25000m,
                    InterestType = "Compounded",
                    PayoutFrequency = "Monthly",
                    TaxRate = 8.0m
                },
                new BankProduct
                {
                    Name = "ДСК Свободен Дует",
                    Type = "Deposit",
                    BankName = "Банка ДСК",
                    Description = "Двугодишен комбиниран депозит в Евро с капитализация на лихвата на всеки 6 месеца.",
                    Currency = "EUR",
                    TermMonths = 24,
                    InterestRate = 1.50m,
                    MinAmount = 2000m,
                    MaxAmount = 150000m,
                    InterestType = "Compounded",
                    PayoutFrequency = "Monthly",
                    TaxRate = 8.0m
                },
                new BankProduct
                {
                    Name = "Постбанк Супер Депозит",
                    Type = "Deposit",
                    BankName = "Постбанк (Юробанк България)",
                    Description = "Депозит в евро с ежемесечно изплащане на лихвата по разплащателна сметка.",
                    Currency = "EUR",
                    TermMonths = 12,
                    InterestRate = 1.95m,
                    MinAmount = 5000m,
                    MaxAmount = 200000m,
                    InterestType = "Simple",
                    PayoutFrequency = "Monthly",
                    TaxRate = 8.0m
                },
                new BankProduct
                {
                    Name = "ОББ Смарт Спестител",
                    Type = "Deposit",
                    BankName = "Обединена Българска Банка (ОББ)",
                    Description = "Гъвкав 6-месечен депозит в евро с възможност за допълване и капитализиране.",
                    Currency = "EUR",
                    TermMonths = 6,
                    InterestRate = 1.20m,
                    MinAmount = 1000m,
                    MaxAmount = 75000m,
                    InterestType = "Compounded",
                    PayoutFrequency = "Monthly",
                    TaxRate = 8.0m
                },
                new BankProduct
                {
                    Name = "ЦКБ Спестовен влог Силва",
                    Type = "Deposit",
                    BankName = "Централна Кооперативна Банка (ЦКБ)",
                    Description = "Дългосрочен спестовен влог с фиксирана доходност и нисък праг на откриване.",
                    Currency = "EUR",
                    TermMonths = 12,
                    InterestRate = 2.00m,
                    MinAmount = 200m,
                    MaxAmount = 50000m,
                    InterestType = "Simple",
                    PayoutFrequency = "AtMaturity",
                    TaxRate = 8.0m
                },
                new BankProduct
                {
                    Name = "ПроКредит Банк - ЕкоДепозит",
                    Type = "Deposit",
                    BankName = "ПроКредит Банк",
                    Description = "Екологичен депозит, средствата от който се използват само за финансиране на зелени проекти. Капитализация на падеж.",
                    Currency = "EUR",
                    TermMonths = 12,
                    InterestRate = 2.25m,
                    MinAmount = 10000m,
                    MaxAmount = 500000m,
                    InterestType = "Compounded",
                    PayoutFrequency = "AtMaturity",
                    TaxRate = 8.0m
                },
                // Placeholders for expansion (non-deposits, only Name and Type implemented)
                new BankProduct
                {
                    Name = "Потребителски кредит с фиксирана лихва",
                    Type = "Loan",
                    BankName = "Банка ДСК",
                    Description = "Placeholder"
                },
                new BankProduct
                {
                    Name = "Кредитна карта Visa Classic",
                    Type = "Credit Card",
                    BankName = "Първа Инвестиционна Банка (Fibank)",
                    Description = "Placeholder"
                }
            };

            context.BankProducts.AddRange(products);
            context.SaveChanges();
        }
    }
}
