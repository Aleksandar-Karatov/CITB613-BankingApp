using Microsoft.EntityFrameworkCore;
using BankingApp.Business.Models;
using System.IO;
using System;

namespace BankingApp.Business.Data
{
    public class BankingDbContext : DbContext
    {
        public DbSet<BankProduct> BankProducts { get; set; } = null!;

        public BankingDbContext()
        {
        }

        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Put database in user's AppData/Local folder to ensure both Web and Desktop apps share the exact same DB
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string dbFolder = Path.Combine(appData, "BankingApp");
                
                if (!Directory.Exists(dbFolder))
                {
                    Directory.CreateDirectory(dbFolder);
                }
                
                string dbPath = Path.Combine(dbFolder, "banking.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure some fields if needed
            modelBuilder.Entity<BankProduct>().HasKey(bp => bp.Id);
        }
    }
}
