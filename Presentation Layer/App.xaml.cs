using PdfSharp.Fonts;
using System.Configuration;
using System.Data;
using System.Windows;
using PdfSharp.Fonts;



namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application

    {
        public App()
        {
            
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            
            GlobalFontSettings.FontResolver = new CustomFontResolver(); 
        }

    }
}
