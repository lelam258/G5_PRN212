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
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AvatarPath);
            if (File.Exists(fullPath))
            {
                AvatarImage.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            }
        }

        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg;*.png)|*.jpg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                string destinationDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "Avatars");

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(destinationDirectory);

                string destinationPath = Path.Combine(destinationDirectory, fileName);
                File.Copy(openFileDialog.FileName, destinationPath, true);

                // Lưu đường dẫn vào DB (tương đối)
                AvatarPath = $"AppData/Avatars/{fileName}";

                // Hiển thị ảnh luôn
                AvatarImage.Source = new BitmapImage(new Uri(destinationPath, UriKind.Absolute));
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
