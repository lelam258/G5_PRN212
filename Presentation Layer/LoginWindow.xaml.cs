using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Repositories.Interfaces;
using Repositories.Repositories;
using BCrypt.Net;
using Business_Layer;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IStudentRepository _studentRepository;
        private string _status;
        public LoginWindow()
        {
            InitializeComponent();
            _studentRepository = new StudentRepository();
        }

        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _status = selectedItem.Content.ToString();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string codeName = TxtCodeName.Text;
            string password = TxtPassword.Password;
            string role = _status;
            if (string.IsNullOrEmpty(codeName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                return;
            }
            else if (role == "Người học")
            {
                var student = _studentRepository.GetStudentByCode(codeName);

                if (student == null)
                {
                    MessageBox.Show("⚠️ Mã người dùng không tồn tại.");
                    return;
                }

                // So sánh đúng: password là người dùng nhập, student.Password là từ DB
                bool isMatch = BCrypt.Net.BCrypt.Verify(password.Trim(), student.Password);


                if (!isMatch)
                {
                    MessageBox.Show("❌ Mật khẩu không đúng.");
                    return;
                }

                // Nếu đúng:
                student.LastLogin = DateTime.Now;
                _studentRepository.UpdateStudent(student);
                MessageBox.Show($"✅ Chào bạn {student.StudentCode}!");
                StudentWindow studentDashboard = new StudentWindow(codeName);
                studentDashboard.Show();
                this.Close();
            }


            else if (role == "Quản trị viên")
            {
                // Admin login logic can be added here
                if (codeName != "admin" || password != "admin123")
                {
                    MessageBox.Show("Sai mã người dùng hoặc mật khẩu");
                    return;
                }
                else
                {
                    MessageBox.Show($"Chào bạn quản lý!");
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    this.Close();
                }
            }
        }
    }
}
