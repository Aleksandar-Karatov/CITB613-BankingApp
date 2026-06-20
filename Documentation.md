# COURSEWORK
**Course:** CITB613: Business Information Systems Practice  
**University:** New Bulgarian University  
**Student:** [Your Name and Surname]  
**Faculty Number:** [Your F.N.]  

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

## 6. Project File Structure, Code Logic, and UI Composition
This section provides a deep dive into how the code is structured, how the UI is composed, and where the core logic resides across the different modules.

### 6.1. BankingApp.Business (Core Logic & Data)
This library is the backbone of the application. It contains the models and the mathematical logic, ensuring that both the Web and Desktop apps use the exact same rules.

* **Models/BankProduct.cs:** This C# class represents the blueprint of a banking product. It contains properties with data types corresponding to the database columns (e.g., `decimal InterestRate`, `int TermMonths`).
* **Data/BankingDbContext.cs:** This class inherits from `DbContext` (Entity Framework Core). It acts as the bridge between the C# code and the SQLite database. It contains `DbSet<BankProduct> BankProducts`, which allows developers to query the database using standard C# LINQ instead of writing raw SQL queries.
* **Services/DepositCalculator.cs:** This is where the core financial mathematics are hardcoded. 
  * **Functionality:** It receives an investment amount, term, interest rate, and payout frequency.
  * **Logic:** It uses a `for` loop to calculate the accrued interest for each period. It applies the standard formula `(Amount * InterestRate / 100)`, calculates the mandatory 10% state tax (`tax = grossInterest * 0.10m`), and stores the result in a list of `DepositCalculationResult` objects. This guarantees that the amortization schedule is calculated consistently.

### 6.2. BankingApp.Desktop (WPF UI Composition & Logic)
The Desktop app uses the Windows Presentation Foundation (WPF) framework. The UI is composed using a separation of markup and logic: the visual layout is written in **XAML** (`.xaml`), while the logic is written in "Code-Behind" C# files (`.xaml.cs`).

* **UI Composition (MainWindow.xaml):** 
  * The main window is composed of a static sidebar (DockPanel) and a dynamic central area (`<ContentControl x:Name="MainContent" />`). 
  * Instead of opening multiple pop-up windows, the application acts as a Single Page Application (SPA). When the user clicks a button in the sidebar, `MainWindow.xaml.cs` swaps the content of `MainContent` with either the `ProductListView` or the `ProductFormView`.
* **Views/ProductListView.xaml & .xaml.cs:** 
  * **UI:** Contains a `DataGrid` to display products in a table, and a Search Bar composed of TextBoxes and ComboBoxes.
  * **C# Logic:** In the code-behind, the `LoadProducts()` method connects directly to the `BankingDbContext` to fetch all products and binds them to the `DataGrid`. 
  * **Search Logic:** The `SearchFilter_Changed` event handler captures keystrokes. It uses LINQ (`.Where(p => p.Name.Contains(keyword))`) to instantly filter the list of products in the computer's RAM (in-memory) without querying the database again, ensuring zero lag.
* **Views/ProductFormView.xaml & .xaml.cs:** 
  * **UI:** A data entry form built with `TextBox` and `ComboBox` controls.
  * **C# Logic:** The `BtnSave_Click` method contains all the validation and database logic. It reads the text from the UI fields, safely parses them into numbers using `decimal.TryParse()`, and validates the business rules (e.g., ensuring the Minimum Amount is not greater than the Maximum Amount). If validation passes, it creates a new `BankProduct` object, adds it to the `BankingDbContext`, and calls `context.SaveChanges()` to write it to the SQLite database.

### 6.3. BankingApp.Web (ASP.NET Core UI Composition & Logic)
The Web app follows the **MVC (Model-View-Controller)** architectural pattern. The logic is handled by Controllers, and the UI is rendered by Razor Views.

* **Controllers/CatalogController.cs:** 
  * **Purpose:** This file acts as the "traffic cop" for the web portal. 
  * **Logic:** When a user visits the Search page, the `Search()` method is triggered. It accepts HTTP GET parameters (like `minAmount`, `currency`). It opens a connection to `BankingDbContext`, queries the database, and uses LINQ to filter out deposits that do not match the user's criteria. Finally, it passes the filtered `List<BankProduct>` to the View.
* **UI Composition (Razor Views - .cshtml):**
  * **Shared/_Layout.cshtml:** This is the master wrapper template. It contains the HTML `<head>`, the navigation bar, and the footer. Inside it is a `@RenderBody()` command, which injects the specific page content.
  * **Catalog/Index.cshtml & Search.cshtml:** These files mix HTML with C# code (Razor syntax). They use a `@foreach (var item in Model)` loop to dynamically generate a Bootstrap "Card" component for every single banking product passed from the Controller.
  * **Catalog/Calculate.cshtml:** This view contains the form where users input their investment amount. When submitted, the Controller passes the data to the `DepositCalculator.cs` service, and the View dynamically generates an HTML `<table>` rendering the exact month-by-month amortization plan.
* **wwwroot/css/site.css & js/site.js:** 
  * Contains the static assets. `site.css` defines the custom Deep Sea Glass theme variables (colors, padding, frosted glass effects). `site.js` contains the JavaScript logic that listens for the Dark/Light mode button click, swaps a `data-theme` attribute on the HTML tag, and saves the user's preference in the browser's `localStorage`.

## 7. Conclusion
The "BankingApp" project demonstrates the integration of various technologies from the .NET ecosystem. The shared library architecture allows for easy future upgrades (such as adding user profiles or migrating to a different database like SQL Server or PostgreSQL). The application is stable and fully functional.
