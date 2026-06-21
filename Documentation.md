# COURSEWORK
**Course:** CITB613: Business Information Systems Practice  
**University:** New Bulgarian University  
**Student:** Aleksandar Zahariev Karatov
**Faculty Number:** F115376 

---

## 1. Introduction and Project Objective
The "BankingApp" project is a software platform for comparing and analyzing banking products (deposits, loans, and credit cards). 
The main objective of the system is to provide end users with a web-based interface for searching, filtering, and calculating deposit yields, while simultaneously providing a desktop application for administrators and bank employees to manage the database of offered products.

## 2. Technologies Used
The system is developed using the following technologies:
* **.NET 8.0:** Used for the core of both applications.
* **C# 12:** The primary programming language.
* **ASP.NET Core MVC:** Used to build the client web portal (Web App).
* **WPF (Windows Presentation Foundation):** Used to build the administrative desktop application.
* **Entity Framework Core (EF Core):** The ORM system used for database access management.
* **SQLite:** A lightweight, embedded relational database used for project portability.
* **Bootstrap 5 & CSS3:** Used for creating the responsive web user interface.

## 3. System Architecture
The solution is divided into three main projects (N-Tier architecture):

1. **BankingApp.Business (Class Library):**
   Contains the shared business logic, data models (`BankProduct`), database configuration (`BankingDbContext`), and helper classes (such as `DepositCalculator` for generating amortization plans). This library is referenced by the other two projects.

2. **BankingApp.Desktop (WPF Application):**
   Serves as the internal administrative system. It allows performing full CRUD (Create, Read, Update, Delete) operations on banking products.

3. **BankingApp.Web (ASP.NET Core Web App):**
   The public portal for end clients. It connects to the shared database and displays the products in a web interface.

## 4. Modules and Functionalities

### 4.1. Administrative Module (Desktop App)
The administrative application (WPF) provides the following functionalities:

* **Main Dashboard:** 
  * Displays a list (DataGrid) of all registered banking products.
  * Shows main properties: Bank Name, Product Type, Currency, Interest Rate, Term, Minimum and Maximum Amount.
* **Search and Filtering:**
  * Real-time search (in-memory filtering). When typing in the search box, the table is automatically filtered by "Product Name" or "Bank Name".
  * A dropdown list is available for filtering by currency (All, EUR, USD).
* **Data Management (CRUD operations):**
  * **Add:** Opens a form where the employee enters general and specific information for the deposit (Term, Interest, Interest Type, Payout Frequency, Min/Max amount, Currency).
  * **Edit:** Double-clicking a record opens the edit form, loading the current product data. Changes are saved directly to the SQLite database via Entity Framework.
  * **Delete:** Includes a confirmation dialog to prevent accidental deletion of a product.
* **Input Validation:** All fields pass through validation (e.g., checking for negative interest rates, mandatory text fields, and proper number formats).

### 4.2. Client Module (Web App)
The web portal provides the following interface for end users:

* **Landing Page:**
  * Contains a brief overview of the platform's features: Catalog, Calculator, and Payment Plan.
* **Catalog:**
  * Dynamically fetches all deposits from the database and visualizes them as interactive cards.
  * Each card shows the bank's name, interest conditions, and a button to calculate the yield.
* **Advanced Search:**
  * Users can filter deposits by currency, term, minimum amount, and payout frequency.
  * **Technical implementation:** Due to SQLite limitations when comparing decimal numbers, part of the calculations and filtering is performed in the server's memory via LINQ to Objects to ensure accurate mathematical results.
* **Detailed View:**
  * Displays the full specification of the selected banking product: description, interest type, and limits.
* **Yield Calculator:**
  * The user enters the desired investment amount.
  * The system generates a detailed payment plan presented as a table, distributed by months or years.
  * **Calculations:** For each period, the calculator computes the accumulated gross interest, the mandatory state tax on interest (10%), and the net amount. Finally, it displays the total net profit and the final account balance.

## 5. User Interface and Design
The application features a custom and unified design system across both modules.
* **Responsive Layout:** The web application uses Bootstrap to ensure the layout adapts to different screen sizes (mobile phones, tablets, and desktops).
* **Theme Support:** The system includes full support for Dark and Light modes. The user can switch themes dynamically via a navigation button, and the choice is saved locally in the browser.
* **Components:** The interface relies on standard modern web components such as navigation bars, interactive cards for product display, and styled input forms for search and data entry.
* **Consistency:** The WPF desktop application is styled with the same color palette and control designs to ensure a consistent user experience across the entire system.

## 6. Detailed File-by-File Explanation

This section provides an exhaustive, file-by-file breakdown of the core logic, models, views, and configuration files across all three projects in the solution.

### 6.1. BankingApp.Business (Core Logic, Data & Models)
This class library acts as the foundation of the architecture. It contains no UI, only data definitions and mathematical algorithms.

* **`Models/BankProduct.cs`** 
  The primary data model (Entity) representing a banking product. It contains properties mapped to database columns (e.g., `Name`, `Currency`, `InterestRate`, `TermMonths`). It uses Data Annotations like `[Required]` and `[MaxLength]` for database schema generation and form validation.
* **`Models/DepositCalculationResult.cs`**
  A Data Transfer Object (DTO) used by the calculator. It holds the final results of a deposit calculation: the gross interest, tax amount, net interest, effective annual rate (ЕГЛ), and a `List<DepositScheduleLine>` containing the month-by-month amortization schedule.
* **`Data/BankingDbContext.cs`**
  The Entity Framework Core database context. It inherits from `DbContext` and defines the `DbSet<BankProduct> BankProducts` collection. It configures the SQLite connection, ensuring the database file (`banking.db`) is created in the user's `%LOCALAPPDATA%` folder so it can be shared between the Web and Desktop apps.
* **`Data/DbInitializer.cs`**
  A utility class containing a `Seed()` method. When the application starts for the first time, this file checks if the database is empty. If it is, it automatically populates the `BankProducts` table with predefined sample deposits (e.g., DSK, Postbank, UBB deposits) so the system is immediately usable.
* **`Services/DepositCalculator.cs`**
  A static utility class containing the core financial mathematics (`CalculateDepositYield`). It takes a `BankProduct` and a principal amount, runs a `for` loop over the deposit's term, and applies logic for simple vs. compounded interest, as well as monthly vs. maturity payout frequencies. It returns a fully populated `DepositCalculationResult`.

### 6.2. BankingApp.Desktop (WPF Administration Interface)
This project is the internal tool for bank employees, built with Windows Presentation Foundation (WPF) and XAML.

* **`App.xaml` & `App.xaml.cs`**
  The entry point of the desktop application. `App.xaml` defines global application resources, primarily linking to the custom `Theme.xaml`. `App.xaml.cs` handles application startup events, specifically invoking the `DbInitializer.Seed()` method to ensure the database is ready before the UI loads.
* **`Styles/Theme.xaml`**
  A global Resource Dictionary containing all CSS-like styling for the WPF application. It defines the color palette (SolidColorBrushes), custom `ControlTemplates`, and styles for components like `AppButton`, `AppTextBox`, and `DataGridCellStyle`. This file completely removes the default grey Windows styling, replacing it with a modern, dark-themed UI.
* **`MainWindow.xaml` & `MainWindow.xaml.cs`**
  The main shell of the application. It features a static left sidebar (using Segoe MDL2 Assets for icons) and a dynamic central `ContentControl`. The Code-Behind (`.xaml.cs`) handles navigation, swapping the central content between the `ProductListView` and the `ProductFormView` without opening new windows (acting as a desktop Single Page Application).
* **`Views/ProductListView.xaml` & `.xaml.cs`**
  The dashboard view. The XAML defines a customized `DataGrid` displaying all products, along with search textboxes and dropdowns. The Code-Behind connects to the `BankingDbContext`, fetches products via `.ToList()`, binds them to the grid, and implements real-time, in-memory LINQ filtering when the user types in the search bar. It also contains the logic for the "Edit" and "Delete" buttons inside the grid rows.
* **`Views/ProductFormView.xaml` & `.xaml.cs`**
  The data entry form. The XAML defines input fields (`TextBox`) and dropdowns (`ComboBox`) for product properties. The Code-Behind handles the `BtnSave_Click` event: it validates user input (e.g., ensuring text parses correctly to decimals), updates or creates a `BankProduct` entity, and calls `context.SaveChanges()` to persist the data to the SQLite database.

### 6.3. BankingApp.Web (ASP.NET Core Public Portal)
This project is the public-facing website for clients, built with the Model-View-Controller (MVC) pattern.

* **`Program.cs`**
  The startup configuration file for the ASP.NET Core web server. It configures dependency injection, enables MVC routing (`MapControllerRoute`), and serves static files from `wwwroot`. It also calls `DbInitializer.Seed()` on startup.
* **`Controllers/HomeController.cs`**
  Handles routing for the static landing page (`Index.cshtml`).
* **`Controllers/CatalogController.cs`**
  The main controller handling the business logic for the public portal. It contains actions like `Index()` (loads all products), `Details(int id)` (loads a specific product), `Search()` (filters products based on GET parameters using LINQ), and `Calculate(int id, decimal amount)` (invokes the `DepositCalculator` service and passes the schedule to the view).
* **`Views/Shared/_Layout.cshtml`**
  The master HTML wrapper. It contains the `<head>`, CSS links, the Bootstrap navigation bar, the Dark/Light mode toggle button, and the `@RenderBody()` directive where specific page views are injected.
* **`Views/Home/Index.cshtml`**
  The landing page view containing the hero section and marketing copy.
* **`Views/Catalog/Index.cshtml` & `Search.cshtml`**
  These Razor views use a `@foreach` loop to iterate over the `List<BankProduct>` provided by the Controller, rendering an interactive Bootstrap Card component for each deposit in the catalog.
* **`Views/Catalog/Details.cshtml`**
  A detailed view page displaying the full specifications (Term, Limits, Tax Rate, Interest Type) of a single selected product.
* **`Views/Catalog/Calculate.cshtml`**
  Contains an input form for the investment amount. Once calculated, it dynamically renders an HTML `<table>` iterating over the `DepositCalculationResult.Schedule`, displaying the month-by-month balance, earned interest, and deducted taxes.
* **`wwwroot/css/site.css`**
  The custom stylesheet. It defines CSS variables for the "Deep Sea Glass" theme, handles the `.bg-glass` aesthetics (backdrop-filter blurs, borders), and contains media queries and `[data-theme="dark"]` overrides to handle dynamic light/dark mode switching seamlessly.
* **`wwwroot/js/site.js`**
  Contains the client-side JavaScript logic. Primarily, it handles the theme toggler button, checking the user's OS preference, switching the HTML `data-theme` attribute, and saving the preference in the browser's `localStorage`.

## 7. Conclusion
The "BankingApp" project demonstrates the integration of various technologies from the .NET ecosystem. The shared library architecture allows for easy future upgrades (such as adding user profiles or migrating to a different database like SQL Server or PostgreSQL). The application is stable and fully functional.
