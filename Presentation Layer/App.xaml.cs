using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = AppSettings.Load();

            if (settings.Theme == "Dark")
            {
                Application.Current.Resources["WindowBackgroundColor"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                Application.Current.Resources["TextForegroundColor"] = new SolidColorBrush(Colors.White);
            }
            else
            {
                Application.Current.Resources["WindowBackgroundColor"] = new SolidColorBrush(Color.FromRgb(244, 246, 248));
                Application.Current.Resources["TextForegroundColor"] = new SolidColorBrush(Colors.Black);
            }
        }


    }


}
