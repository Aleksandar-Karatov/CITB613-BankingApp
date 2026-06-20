# BankingApp

University project for the course **CITB613: Business Information Systems Practicum** (New Bulgarian University).

The system is a **bank product comparison platform**, consisting of:
1. Shared Business Logic (C# Class Library)
2. Desktop application (WPF) for database management and adding bank products.
3. Web application (ASP.NET Core MVC) — client portal with a catalog, filters, calculator, and an amortization schedule.

---

## How to run the project

### Requirements:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)
- Windows (required for the Desktop WPF application)

The database (SQLite) will be automatically created and seeded with initial data upon the first launch of either application. It is stored locally at:
`%LOCALAPPDATA%\BankingApp\banking.db`

### Starting the Desktop Application (Management)

The Desktop application is used for adding, editing, and deleting bank products.

Open a terminal in the main project folder and execute:
```bash
dotnet run --project BankingApp.Desktop
```

### Starting the Web Application (Client Portal)

The web application contains the public catalog, advanced search, and the calculator with an amortization schedule.

Open a terminal in the main project folder and execute:
```bash
dotnet run --project BankingApp.Web
```
Then open your browser to the address shown in the console (usually `http://localhost:5000` or `https://localhost:5001`).

---

## Solution Structure

* `BankingApp.Business` - Models, DbContext, and the static `DepositCalculator`.
* `BankingApp.Desktop` - WPF project. UI for entering deposits (CRUD).
* `BankingApp.Web` - ASP.NET Core MVC project. Contains `CatalogController` with views for `Index`, `Search`, `Details`, and `Calculate`.

---

*Developed according to the specifications of the group project assignment.*

 ### Developed by Aleksandar Zahariev Karatov, F115376 for the course CITB613: Business Information Systems Practicum (New Bulgarian University).
