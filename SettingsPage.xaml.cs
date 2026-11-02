
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace HesapMakinesi
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            string tema = Preferences.Get("UygulamaTemasi", "System");

            if (tema == "Light")
            {
                ThemePicker.SelectedIndex = 0;
            }
            else if (tema == "Dark")
            {
                ThemePicker.SelectedIndex = 1;
            }
            else
            {
                ThemePicker.SelectedIndex = 2;
            }
        }

        
        private void OnThemeChanged(object sender, EventArgs e)
        {
            if (ThemePicker.SelectedIndex == 0)
            {
                Application.Current.UserAppTheme = AppTheme.Light;
                Preferences.Set("UygulamaTemasi", "Light");
            }
            else if (ThemePicker.SelectedIndex == 1)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
                Preferences.Set("UygulamaTemasi", "Dark");
            }
            else if (ThemePicker.SelectedIndex == 2)
            {
                Application.Current.UserAppTheme = AppTheme.Unspecified;
                Preferences.Set("UygulamaTemasi", "System");
            }
        }
    }
}
