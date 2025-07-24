using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Business_Layer;
using Presentation_Layer.Helpers;
using Repositories.Interfaces;

namespace Presentation_Layer
{
    public partial class StudentSettingPage : Page
    {
        private bool _darkModeEnabled = false;
        private readonly IStudentRepository _studentRepository;
        private int _studentId;

        public StudentSettingPage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;

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
            var context = ApplicationDbContext.Instance;

            var student = context.Students.FirstOrDefault(s => s.StudentId == _studentId);
            if (student == null)
            {
                MessageBox.Show("Không tìm thấy sinh viên.");
                return;
            }

            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường.");
                return;
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, student.Password))
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu mới không khớp.");
                return;
            }

            // Mã hóa mật khẩu mới
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            student.Password = hashedPassword;

            context.SaveChanges();

            MessageBox.Show("✅ Đổi mật khẩu thành công!");
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

    }
}
