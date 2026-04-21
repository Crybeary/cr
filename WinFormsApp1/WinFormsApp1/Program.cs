using CreditApp;

namespace CreditApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            DatabaseHelper.InitializeDatabase();
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}