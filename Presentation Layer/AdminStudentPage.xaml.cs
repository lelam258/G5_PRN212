using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
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
using BCrypt.Net;
using Business_Layer;
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for AdminStudentPage.xaml
    /// </summary>
    public partial class AdminStudentPage : Page
    {
        private readonly IStudentRepository _studentRepository;
        public AdminStudentPage()
        {
            InitializeComponent();
            _studentRepository = new StudentRepository();
            LoadStudent();
        }
        public void LoadStudent()
        {
            var students = _studentRepository.GetAllStudents()
                .Select(s => new StudentView
                {
                    StudentID = s.StudentId,
                    StudentCode = s.StudentCode,
                    StudentName = s.StudentName,
                    BirthDate = (DateTime)s.DateOfBirth,
                    Email = s.Email,
                    Phone = s.PhoneNumber,
                    Status = s.Status
                }).ToList();
            DataGrid.ItemsSource = students;
        }
        public class StudentView
        {
            public int StudentID { get; set; }
            public string StudentCode { get; set; }
            public string StudentName { get; set; }
            public DateTime BirthDate { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GenderComboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                GenderTextBox.Text = selectedItem.Content.ToString();
                // Handle the selection change if needed
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var students = _studentRepository.GetAllStudents();
            string searchName = SearchNameTextBox.Text;
            if (!string.IsNullOrEmpty(searchName))
            {
                students = students.Where(s =>
                s.StudentName.Contains(searchName, StringComparison.OrdinalIgnoreCase) ||
                s.StudentCode.Contains(searchName, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Contains(searchName, StringComparison.OrdinalIgnoreCase) ||
                s.PhoneNumber.Contains(searchName, StringComparison.OrdinalIgnoreCase) 
                ).ToList();
            }
            if (GenderComboBox.SelectedItem is ComboBoxItem selectedStatus)
            {
                string status = selectedStatus.Content.ToString();
                if (!string.IsNullOrEmpty(status) && !status.Equals("Tất cả"))
                {
                    students = students.Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            var studentViews = students.Select(s => new StudentView
            {
                StudentID = s.StudentId,
                StudentCode = s.StudentCode,
                StudentName = s.StudentName,
                BirthDate = (DateTime)s.DateOfBirth,
                Email = s.Email,
                Phone = s.PhoneNumber,
                Status = s.Status
            }).ToList();
            DataGrid.ItemsSource = studentViews;
            DataGrid.SelectedItem = null; // Clear selection after search
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if ((CodeTextBox.Text == string.Empty) || (NameTextBox.Text == string.Empty) || 
                (BirthDatePicker.SelectedDate == DateTime.MinValue) || GenderComboBox1.SelectedItem.ToString() == string.Empty)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin");
                return;
            }
            try
            {
                var student = new Student
                {
                    StudentCode = CodeTextBox.Text,
                    StudentName = NameTextBox.Text,
                    DateOfBirth = BirthDatePicker.SelectedDate,
                    Email = EmailTextBox.Text,
                    PhoneNumber = PhoneTextBox.Text,
                    Status = GenderTextBox.Text,
                    Password = BCrypt.Net.BCrypt.HashPassword(CodeTextBox.Text)
                };
                _studentRepository.AddStudent(student);
                LoadStudent();
                MessageBox.Show("Thêm sinh viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sinh viên: {ex.Message}");
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is not StudentView studentView)
            {
                MessageBox.Show("Cần chọn sinh viên!");
                return;
            }
            var existedStudent = _studentRepository.GetStudentById(studentView.StudentID);
            if (existedStudent != null)
            {
                existedStudent.StudentCode = CodeTextBox.Text;
                existedStudent.StudentName = NameTextBox.Text;
                existedStudent.DateOfBirth = BirthDatePicker.SelectedDate;
                existedStudent.Email = EmailTextBox.Text;
                existedStudent.PhoneNumber = PhoneTextBox.Text;
                existedStudent.Status = GenderTextBox.Text;
                _studentRepository.UpdateStudent(existedStudent);
                LoadStudent();
                MessageBox.Show("Cập nhật thông tin sinh viên thành công!");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is not StudentView studentView)
            {
                MessageBox.Show("Cần chọn sinh viên!");
                return;
            }
            var student = _studentRepository.GetStudentById(studentView.StudentID);
            if (student != null)
            {
                _studentRepository.DeleteStudent(student.StudentId);
                LoadStudent();
                MessageBox.Show("Xóa sinh viên thành công!");
            }
            else
            {
                MessageBox.Show("Sinh viên không tồn tại!");
            }
        }

        private void DataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem is StudentView selectedStudent)
            {
                CodeTextBox.Text = selectedStudent.StudentCode;
                NameTextBox.Text = selectedStudent.StudentName;
                BirthDatePicker.SelectedDate = selectedStudent.BirthDate;
                EmailTextBox.Text = selectedStudent.Email;
                PhoneTextBox.Text = selectedStudent.Phone;
                GenderComboBox1.SelectedItem = GenderComboBox1.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == selectedStudent.Status);
                UpdateButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                CodeTextBox.Clear();
                NameTextBox.Clear();
                BirthDatePicker.SelectedDate = null;
                EmailTextBox.Clear();
                PhoneTextBox.Clear();
                GenderComboBox1.SelectedIndex = -1; // Clear selection
            }
        }
    }
}
