using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BankingApp.Business.Data;
using BankingApp.Business.Models;

namespace BankingApp.Desktop.Views
{
    public partial class ProductListView : UserControl
    {
        private readonly string _typeFilter;
        private List<BankProduct> _products = new();

        /// <summary>Raised when the user wants to edit a product. Carries the product Id.</summary>
        public event EventHandler<int>? EditRequested;

        /// <summary>Raised when the user clicks Add.</summary>
        public event EventHandler? AddRequested;

        /// <summary>Raised after a product is deleted so the parent can refresh counts.</summary>
        public event EventHandler? ProductDeleted;

        public ProductListView(string typeFilter = "All")
        {
            InitializeComponent();
            _typeFilter = typeFilter;
            SetTitle();
            LoadProducts();
        }

        //  Data loading

        private void LoadProducts()
        {
            using var context = new BankingDbContext();
            IQueryable<BankProduct> query = context.BankProducts;

            if (_typeFilter != "All")
            {
                query = query.Where(p => p.Type == _typeFilter);
            }

            _products = query.OrderBy(p => p.BankName).ThenBy(p => p.Name).ToList();
            ApplyFilter();
        }

        private void SearchFilter_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_products == null) return;

            var keyword = TxtSearchKeyword?.Text?.ToLower() ?? "";
            var currencyItem = CmbSearchCurrency?.SelectedItem as ComboBoxItem;
            var currency = currencyItem?.Content?.ToString();

            var filtered = _products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filtered = filtered.Where(p => 
                    (p.Name != null && p.Name.ToLower().Contains(keyword)) || 
                    (p.BankName != null && p.BankName.ToLower().Contains(keyword)));
            }

            if (currency != "Всички" && !string.IsNullOrEmpty(currency))
            {
                filtered = filtered.Where(p => p.Currency == currency);
            }

            var resultList = filtered.ToList();
            ProductGrid.ItemsSource = resultList;
            TxtSubtitle.Text = $"{resultList.Count} намерени записа";
        }

        private void SetTitle()
        {
            TxtTitle.Text = _typeFilter switch
            {
                "Deposit" => "Депозити",
                "Loan" => "Кредити",
                "Credit Card" => "Кредитни карти",
                _ => "Всички продукти"
            };
        }

        //  Button handlers

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductGrid.SelectedItem is BankProduct product)
            {
                EditRequested?.Invoke(this, product.Id);
            }
            else
            {
                MessageBox.Show("Моля, изберете продукт от списъка.",
                    "Няма избран продукт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductGrid.SelectedItem is not BankProduct product)
            {
                MessageBox.Show("Моля, изберете продукт от списъка.",
                    "Няма избран продукт", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Сигурни ли сте, че искате да изтриете \"{product.Name}\"?",
                "Потвърждение за изтриване",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using var context = new BankingDbContext();
                var entity = context.BankProducts.Find(product.Id);
                if (entity != null)
                {
                    context.BankProducts.Remove(entity);
                    context.SaveChanges();
                }

                LoadProducts();
                ProductDeleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductGrid.SelectedItem is BankProduct product)
            {
                EditRequested?.Invoke(this, product.Id);
            }
        }
    }
}
