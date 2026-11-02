
using Microsoft.Maui.Storage;

namespace HesapMakinesi
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string tema = Preferences.Get("UygulamaTemasi", "System");

            if (tema == "Light")
            {
                UserAppTheme = AppTheme.Light;
            }
            else if (tema == "Dark")
            {
                UserAppTheme = AppTheme.Dark;
            }
            else
            {
                UserAppTheme = AppTheme.Unspecified;
            }

            MainPage = new AppShell();
        }
    }
}

