using Business_Layer;
using Microsoft.Win32;
using Repositories.Repositories;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Presentation_Layer
{
    public partial class StudentInformationPage : Page
    {
        private readonly StudentRepository _studentRepo = new();
        private Student _student;

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AvatarPath { get; set; }

        public StudentInformationPage(int studentId)
        {
            InitializeComponent();
            _student = _studentRepo.GetStudentById(studentId);

            // Bind dữ liệu
            Email = _student.Email;
            PhoneNumber = _student.PhoneNumber;
            DateOfBirth = _student.DateOfBirth;
            AvatarPath = _student.AvatarPath;

            DataContext = this;
        }

        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh đại diện",
                Filter = "Hình ảnh (*.jpg;*.png)|*.jpg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(dialog.FileName);
                string destPath = $"AppData/Avatars/{fileName}";

                Directory.CreateDirectory("AppData/Avatars");
                File.Copy(dialog.FileName, destPath, true);

                AvatarPath = Path.GetFullPath(destPath);
                DataContext = null;
                DataContext = this;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            _student.Email = Email;
            _student.PhoneNumber = PhoneNumber;
            _student.DateOfBirth = DateOfBirth;
            _student.AvatarPath = AvatarPath;

            _studentRepo.UpdateStudent(_student);
            MessageBox.Show("Thông tin đã được cập nhật!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
