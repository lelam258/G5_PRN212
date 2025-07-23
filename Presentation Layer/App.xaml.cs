using PdfSharp.Fonts;
using System.Configuration;
using System.Data;
using System.Windows;


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
            // Load theme người dùng chọn trước đó
            string savedTheme = Presentation_Layer.Properties.Settings.Default.Theme;
            if (string.IsNullOrEmpty(savedTheme))
                savedTheme = "Light"; // fallback mặc định

            Helpers.ThemeManager.SetTheme(savedTheme);
        }
        public static void ApplyTheme(string themeFile)
        {
            try
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri($"/Presentation Layer;component/Themes/{themeFile}", UriKind.Relative)
                };

                // Xóa các ResourceDictionary cũ liên quan đến theme (nếu có)
                var existingDictionaries = Current.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.OriginalString.Contains("Themes/"))
                    .ToList();

                foreach (var oldDict in existingDictionaries)
                    Current.Resources.MergedDictionaries.Remove(oldDict);

                // Thêm theme mới
                Current.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải theme: {themeFile}\n{ex.Message}", "Lỗi theme");
            }
        }


    }
}
