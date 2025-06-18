using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Views;
using WpfApp1.Database;
using System.IO;

namespace WpfApp1
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            if (e.Args.Length > 0 && e.Args[0].ToLower() == "/setupdb")
            {
                SetupDatabase();
                Environment.Exit(0);
                return;
            }

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            
            this.MainWindow = loginWindow;
        }

        private void SetupDatabase()
        {
            try
            {
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "CreateDatabase.sql");
                
                bool result = DatabaseConnection.Instance.ExecuteSqlScript(scriptPath);
                
                Environment.ExitCode = result ? 0 : 1;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db_setup_error.log"), ex.ToString());
                Environment.ExitCode = 1;
            }
        }
    }
}
