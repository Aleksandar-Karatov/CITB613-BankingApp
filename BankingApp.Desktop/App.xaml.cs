using System.Windows;
using BankingApp.Business.Data;

namespace BankingApp.Desktop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the database and seed data on first run
            using (var context = new BankingDbContext())
            {
                DbInitializer.Initialize(context);
            }
        }
    }
}
