using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Presentation_Layer.Helpers;

namespace Presentation_Layer
{
    public partial class StudentSettingPage : Page
    {
        private bool _darkModeEnabled = false;

        public StudentSettingPage()
        {
            InitializeComponent();
            string currentTheme = Properties.Settings.Default.Theme;
            if (currentTheme == "Dark")
            {
                DarkModeToggle.IsChecked = true;
            }
            else
            {
                DarkModeToggle.IsChecked = false;
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var oldPassword = OldPasswordBox.Password;
            var newPassword = NewPasswordBox.Password;
            var confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirm)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Thực hiện đổi mật khẩu (gọi repo/update)
            MessageBox.Show("Mật khẩu đã được thay đổi.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            _darkModeEnabled = true;
            ThemeManager.SetTheme("Dark");

            Properties.Settings.Default.Theme = "Dark";
            Properties.Settings.Default.Save();
        }

        private void DarkModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _darkModeEnabled = false;
            ThemeManager.SetTheme("Light");

            Properties.Settings.Default.Theme = "Light";
            Properties.Settings.Default.Save();
        }

        private void ResetProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng đang phát triển...", "Thông báo");
        }

        private void ResetAllSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã khôi phục cài đặt mặc định.", "Thông báo");
        }
    }
}
