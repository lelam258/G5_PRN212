using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Presentation_Layer
{
    public partial class StudentSettingPage : Page
    {
        private bool _darkModeEnabled = false;

        public StudentSettingPage()
        {
            InitializeComponent();
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
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        }

        private void DarkModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _darkModeEnabled = false;
            Background = new SolidColorBrush(Color.FromRgb(244, 246, 248));
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
