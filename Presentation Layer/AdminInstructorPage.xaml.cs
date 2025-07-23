using Business_Layer;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for AdminInstructorPage.xaml
    /// </summary>
    public partial class AdminInstructorPage : Page
    {
        private readonly InstructorRepository _instructorRepo = new();
        private List<Instructor> _instructors = new();
        private const string EmailPattern = @"^[\w\-.]+@([\w\-]+\.)+[\w\-]{2,4}$";
        private const string PhonePattern = @"^0\d{9}$";

        public AdminInstructorPage()
        {
            InitializeComponent();
            LoadInstructors();
        }

        // Nạp toàn bộ giảng viên lên DataGrid
        private void LoadInstructors()
        {
            _instructors = _instructorRepo.GetAllInstructors();
            InstructorsDataGrid.ItemsSource = _instructors;
            ClearForm();
        }

        // Clear form và reset nút
        private void ClearForm()
        {
            NameTextBox.Clear();
            ExperienceTextBox.Clear();
            EmailTextBox.Clear();
            PhoneTextBox.Clear();
            AddButton.IsEnabled = true;
            UpdateButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            InstructorsDataGrid.UnselectAll();
        }

        // Khi chọn dòng trên DataGrid
        private void InstructorsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InstructorsDataGrid.SelectedItem is Instructor ins)
            {
                NameTextBox.Text = ins.InstructorName;
                ExperienceTextBox.Text = ins.Experience.ToString();
                EmailTextBox.Text = ins.Email;
                PhoneTextBox.Text = ins.PhoneNumber;

                AddButton.IsEnabled = false;
                UpdateButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                ClearForm();
            }
        }

        // Thêm mới giảng viên
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text.Trim();
            var expTxt = ExperienceTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();
            var phone = PhoneTextBox.Text.Trim();

            // 1) Tên bắt buộc
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Tên không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // 2) Kinh nghiệm phải là số nguyên >= 0
            if (!int.TryParse(expTxt, out int exp) || exp < 0)
            {
                MessageBox.Show("Kinh Nghiệm Không hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // 3) Email đúng định dạng
            if (!Regex.IsMatch(email, EmailPattern))
            {
                MessageBox.Show("Email không đúng định dạng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Regex.IsMatch(phone, PhonePattern))
            {
                MessageBox.Show("Số điện thoại không đúng định dạng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // nếu pass hết thì mới tạo
            var ins = new Instructor
            {
                InstructorName = name,
                Experience = exp,
                Email = email,
                PhoneNumber = phone
            };
            _instructorRepo.AddInstructor(ins);
            LoadInstructors();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (InstructorsDataGrid.SelectedItem is not Instructor ins) return;

            var name = NameTextBox.Text.Trim();
            var expTxt = ExperienceTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();
            var phone = PhoneTextBox.Text.Trim();

            // (giống Add) validate lại
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Tên không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(expTxt, out int exp) || exp < 0)
            {
                MessageBox.Show("Kinh Nghiệm Không hợp lệ", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Regex.IsMatch(email, EmailPattern))
            {
                MessageBox.Show("Email không đúng định dạng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Regex.IsMatch(phone, PhonePattern))
            {
                MessageBox.Show("Số điện thoại không đúng định dạng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // cập nhật và save
            ins.InstructorName = name;
            ins.Experience = exp;
            ins.Email = email;
            ins.PhoneNumber = phone;
            _instructorRepo.UpdateInstructor(ins);
            LoadInstructors();
        }

        // Xóa giảng viên
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (InstructorsDataGrid.SelectedItem is not Instructor ins) return;

            var result = MessageBox.Show(
                $"Bạn chắc chắn muốn xóa giảng viên “{ins.InstructorName}”?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _instructorRepo.DeleteInstructor(ins.InstructorId);
                LoadInstructors();
            }
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Clear();
            ExperienceTextBox.Clear();
            EmailTextBox.Clear();
            PhoneTextBox.Clear();
            AddButton.IsEnabled = true;
            UpdateButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            InstructorsDataGrid.UnselectAll();
        }
       

    }
}
