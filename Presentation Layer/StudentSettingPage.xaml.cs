using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Business_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class StudentSettingPage : Page
    {
        private readonly StudentRepository _studentRepo = new();
        private Student _student;
        private int _studentId;

        public StudentSettingPage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            _student = _studentRepo.GetStudentById(studentId);
        }

        // ==== ĐỔI MẬT KHẨU ====
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

            var student = _studentRepo.GetStudentById(_studentId);
            if (student.Password != oldPassword)
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            student.Password = newPassword;
            _studentRepo.UpdateStudent(student);

            MessageBox.Show("Mật khẩu đã được thay đổi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            OldPasswordBox.Password = "";
            NewPasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";
        }

        // ==== GIAO DIỆN SÁNG/TỐI ====
        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["WindowBackgroundColor"] = new SolidColorBrush(Color.FromRgb(244, 246, 248));
            Application.Current.Resources["TextForegroundColor"] = new SolidColorBrush(Colors.Black);

            var settings = AppSettings.Load();
            settings.Theme = "Light";
            settings.Save();
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["WindowBackgroundColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E")); // nền xám đậm
            Application.Current.Resources["TextForegroundColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));  // chữ sáng

            var settings = AppSettings.Load();
            settings.Theme = "Dark";
            settings.Save();
        }


        // ==== ĐẶT LẠI THÔNG TIN CÁ NHÂN ====
        private void ResetProfile_Click(object sender, RoutedEventArgs e)
        {
            _student = _studentRepo.GetStudentById(_studentId);
            MessageBox.Show("Đã khôi phục thông tin gốc từ cơ sở dữ liệu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==== RESET TOÀN BỘ (hiện tại chỉ cảnh báo) ====
        private void ResetAllSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng này chưa được hỗ trợ đầy đủ.", "Thông báo");
        }
    }
}
