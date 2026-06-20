using System.Windows;
using BankingApp.Business.Data;
using BankingApp.Desktop.Views;

namespace BankingApp.Desktop
{
    public partial class MainWindow : Window
    {
        private string _currentFilter = "All";

        public MainWindow()
        {
            InitializeComponent();
            UpdateProductCount();
            ShowProductList("All");
        }

        //  Navigation handlers

        private void BtnAllProducts_Click(object sender, RoutedEventArgs e) => ShowProductList("All");
        private void BtnDeposits_Click(object sender, RoutedEventArgs e) => ShowProductList("Deposit");
        private void BtnLoans_Click(object sender, RoutedEventArgs e) => ShowProductList("Loan");
        private void BtnCards_Click(object sender, RoutedEventArgs e) => ShowProductList("Credit Card");

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var formView = new ProductFormView();
            formView.ProductSaved += OnProductSaved;
            formView.Cancelled += OnFormCancelled;
            MainContent.Content = formView;
        }

        //  View management

        public void ShowProductList(string typeFilter)
        {
            _currentFilter = typeFilter;
            var listView = new ProductListView(typeFilter);
            listView.EditRequested += OnEditRequested;
            listView.AddRequested += (_, _) => BtnAddProduct_Click(this, new RoutedEventArgs());
            listView.ProductDeleted += (_, _) =>
            {
                UpdateProductCount();
            };
            MainContent.Content = listView;
        }

        public void ShowEditForm(int productId)
        {
            var formView = new ProductFormView(productId);
            formView.ProductSaved += OnProductSaved;
            formView.Cancelled += OnFormCancelled;
            MainContent.Content = formView;
        }

        //  Event handlers

        private void OnEditRequested(object? sender, int productId)
        {
            ShowEditForm(productId);
        }

        private void OnProductSaved(object? sender, EventArgs e)
        {
            UpdateProductCount();
            ShowProductList(_currentFilter);
        }

        private void OnFormCancelled(object? sender, EventArgs e)
        {
            ShowProductList(_currentFilter);
        }

        private void UpdateProductCount()
        {
            using var context = new BankingDbContext();
            int count = context.BankProducts.Count();
            TxtProductCount.Text = $"{count} продукта в базата";
        }
    }
}
