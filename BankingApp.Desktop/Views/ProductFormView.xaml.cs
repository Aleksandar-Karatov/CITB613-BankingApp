using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BankingApp.Business.Data;
using BankingApp.Business.Models;

namespace BankingApp.Desktop.Views
{
    public partial class ProductFormView : UserControl
    {
        private readonly int? _editProductId;

        /// <summary>Raised after the product is successfully saved.</summary>
        public event EventHandler? ProductSaved;

        /// <summary>Raised when the user cancels.</summary>
        public event EventHandler? Cancelled;

        /// <summary>Create form in Add mode.</summary>
        public ProductFormView()
        {
            InitializeComponent();
            _editProductId = null;
            TxtFormTitle.Text = "Добавяне на нов продукт";
            UpdateDepositFieldsVisibility();
        }

        /// <summary>Create form in Edit mode for the given product.</summary>
        public ProductFormView(int productId)
        {
            InitializeComponent();
            _editProductId = productId;
            TxtFormTitle.Text = "Редактиране на продукт";
            LoadProduct(productId);
            UpdateDepositFieldsVisibility();
        }

        //  Load existing product into form fields

        private void LoadProduct(int id)
        {
            using var context = new BankingDbContext();
            var product = context.BankProducts.Find(id);
            if (product == null)
            {
                MessageBox.Show("Продуктът не е намерен.", "Грешка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Set Type combo
            foreach (ComboBoxItem item in CmbType.Items)
            {
                if (item.Content.ToString() == product.Type)
                {
                    CmbType.SelectedItem = item;
                    break;
                }
            }

            TxtName.Text = product.Name;
            TxtBankName.Text = product.BankName;
            TxtDescription.Text = product.Description;

            // Deposit fields
            foreach (ComboBoxItem item in CmbCurrency.Items)
            {
                if (item.Content.ToString() == product.Currency)
                {
                    CmbCurrency.SelectedItem = item;
                    break;
                }
            }

            TxtTermMonths.Text = product.TermMonths.ToString();
            TxtInterestRate.Text = product.InterestRate.ToString(CultureInfo.InvariantCulture);
            TxtMinAmount.Text = product.MinAmount.ToString(CultureInfo.InvariantCulture);
            TxtMaxAmount.Text = product.MaxAmount.ToString(CultureInfo.InvariantCulture);

            foreach (ComboBoxItem item in CmbInterestType.Items)
            {
                if (item.Content.ToString() == product.InterestType)
                {
                    CmbInterestType.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in CmbPayoutFrequency.Items)
            {
                if (item.Content.ToString() == product.PayoutFrequency)
                {
                    CmbPayoutFrequency.SelectedItem = item;
                    break;
                }
            }

            TxtTaxRate.Text = product.TaxRate.ToString(CultureInfo.InvariantCulture);
        }

        //  Visibility toggle for deposit-specific fields

        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDepositFieldsVisibility();
        }

        private void UpdateDepositFieldsVisibility()
        {
            if (DepositFields == null) return;

            string selectedType = GetSelectedComboText(CmbType);
            DepositFields.Visibility = selectedType == "Deposit"
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        //  Save

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            string name = TxtName.Text.Trim();
            string bankName = TxtBankName.Text.Trim();
            string type = GetSelectedComboText(CmbType);

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Моля, въведете име на продукта.",
                    "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(bankName))
            {
                MessageBox.Show("Моля, въведете име на банката.",
                    "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtBankName.Focus();
                return;
            }

            using var context = new BankingDbContext();

            BankProduct product;
            if (_editProductId.HasValue)
            {
                product = context.BankProducts.Find(_editProductId.Value)!;
                if (product == null)
                {
                    MessageBox.Show("Продуктът не е намерен в базата данни.",
                        "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                product = new BankProduct();
                context.BankProducts.Add(product);
            }

            product.Name = name;
            product.BankName = bankName;
            product.Type = type;
            product.Description = TxtDescription.Text.Trim();

            if (type == "Deposit")
            {
                // Parse and validate deposit fields
                product.Currency = GetSelectedComboText(CmbCurrency);
                product.InterestType = GetSelectedComboText(CmbInterestType);
                product.PayoutFrequency = GetSelectedComboText(CmbPayoutFrequency);

                if (!int.TryParse(TxtTermMonths.Text.Trim(), out int termMonths) || termMonths <= 0)
                {
                    ShowValidationError("Моля, въведете валиден срок (положително цяло число).", TxtTermMonths);
                    return;
                }
                product.TermMonths = termMonths;

                if (!decimal.TryParse(TxtInterestRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) || rate < 0)
                {
                    ShowValidationError("Моля, въведете валиден лихвен процент.", TxtInterestRate);
                    return;
                }
                product.InterestRate = rate;

                if (!decimal.TryParse(TxtMinAmount.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minAmt) || minAmt < 0)
                {
                    ShowValidationError("Моля, въведете валидна минимална сума.", TxtMinAmount);
                    return;
                }
                product.MinAmount = minAmt;

                if (!decimal.TryParse(TxtMaxAmount.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal maxAmt) || maxAmt <= 0)
                {
                    ShowValidationError("Моля, въведете валидна максимална сума.", TxtMaxAmount);
                    return;
                }
                product.MaxAmount = maxAmt;

                if (minAmt > maxAmt)
                {
                    ShowValidationError("Минималната сума не може да бъде по-голяма от максималната.", TxtMinAmount);
                    return;
                }

                if (!decimal.TryParse(TxtTaxRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal taxRate) || taxRate < 0)
                {
                    ShowValidationError("Моля, въведете валиден данъчен процент.", TxtTaxRate);
                    return;
                }
                product.TaxRate = taxRate;
            }

            context.SaveChanges();
            ProductSaved?.Invoke(this, EventArgs.Empty);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        //  Helpers

        private static string GetSelectedComboText(ComboBox combo)
        {
            if (combo.SelectedItem is ComboBoxItem item)
                return item.Content?.ToString() ?? string.Empty;
            return string.Empty;
        }

        private static void ShowValidationError(string message, TextBox focusTarget)
        {
            MessageBox.Show(message, "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
            focusTarget.Focus();
        }
    }
}
