using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BankingApp.Business.Data;
using BankingApp.Business.Models;
using BankingApp.Business.Services;

namespace BankingApp.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly BankingDbContext _context;

        public CatalogController(BankingDbContext context)
        {
            _context = context;
        }

        //  GET /Catalog — Deposit Catalog
        public IActionResult Index()
        {
            var deposits = _context.BankProducts
                .Where(p => p.Type == "Deposit")
                .OrderBy(p => p.BankName)
                .ThenBy(p => p.Name)
                .ToList();

            return View(deposits);
        }

        //  GET /Catalog/Search — Advanced Search
        public IActionResult Search(
            string? currency,
            decimal? minAmount,
            decimal? maxAmount,
            int? minTerm,
            int? maxTerm,
            string? interestType,
            string? payoutFrequency)
        {
            var query = _context.BankProducts
                .Where(p => p.Type == "Deposit")
                .ToList() // Fix: SQLite decimal string comparison bug
                .AsQueryable();

            if (!string.IsNullOrEmpty(currency))
                query = query.Where(p => p.Currency == currency);

            if (minAmount.HasValue)
                query = query.Where(p => p.MaxAmount >= minAmount.Value);

            if (maxAmount.HasValue)
                query = query.Where(p => p.MinAmount <= maxAmount.Value);

            if (minTerm.HasValue)
                query = query.Where(p => p.TermMonths >= minTerm.Value);

            if (maxTerm.HasValue)
                query = query.Where(p => p.TermMonths <= maxTerm.Value);

            if (!string.IsNullOrEmpty(interestType))
                query = query.Where(p => p.InterestType == interestType);

            if (!string.IsNullOrEmpty(payoutFrequency))
                query = query.Where(p => p.PayoutFrequency == payoutFrequency);

            var results = query.OrderByDescending(p => p.InterestRate).ToList();

            // Preserve filter values for the form
            ViewBag.Currency = currency;
            ViewBag.MinAmount = minAmount;
            ViewBag.MaxAmount = maxAmount;
            ViewBag.MinTerm = minTerm;
            ViewBag.MaxTerm = maxTerm;
            ViewBag.InterestType = interestType;
            ViewBag.PayoutFrequency = payoutFrequency;
            ViewBag.HasSearched = true;

            return View(results);
        }

        //  GET /Catalog/Details/5 — Product Details
        public IActionResult Details(int id)
        {
            var product = _context.BankProducts.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        //  GET /Catalog/Calculate/5 — Calculator Page
        public IActionResult Calculate(int id)
        {
            var product = _context.BankProducts.Find(id);
            if (product == null)
                return NotFound();

            ViewBag.Product = product;
            ViewBag.Result = null;
            ViewBag.ErrorMessage = null;

            return View();
        }

        //  POST /Catalog/Calculate/5 — Run Calculation
        [HttpPost]
        public IActionResult Calculate(int id, decimal amount)
        {
            var product = _context.BankProducts.Find(id);
            if (product == null)
                return NotFound();

            ViewBag.Product = product;
            ViewBag.Amount = amount;

            try
            {
                var result = DepositCalculator.Calculate(product, amount);
                ViewBag.Result = result;
                ViewBag.ErrorMessage = null;
            }
            catch (ArgumentException ex)
            {
                ViewBag.Result = null;
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }
    }
}
