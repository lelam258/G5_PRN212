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

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        private readonly IStudentRepository _studentRepository;
        private string _code;
        public StudentWindow(string code)
        {
            InitializeComponent();
            _studentRepository = new StudentRepository();
            _code = code;
        }
        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListBox.SelectedItem is ListBoxItem selectedItem)
            {
                string pagePath = selectedItem.Tag.ToString();
                try
                {
                    if (pagePath == "Logout")
                    {
                        var result = MessageBox.Show("Bạn có chắc muốn đăng xuất không?",
                                                     "Xác nhận đăng xuất",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Show login window
                            LoginWindow loginWindow = new LoginWindow();
                            loginWindow.Show();
                            // Close current admin window
                            this.Close();
                        }

                        // Cancel logout if No is selected
                        selectedItem.IsSelected = false;
                        return;
                    }
                    if (pagePath == "StudentCourseSchedulePage.xaml")
                    {
                        var student = _studentRepository.GetStudentByCode(_code);
                        if (student != null)
                        {
                            ContentFrame.Navigate(new StudentCourseSchedulePage(student.StudentId));
                        }
                    }
                    else if (pagePath == "StudentPaymentPage.xaml")
                    {
                        var student = _studentRepository.GetStudentByCode(_code);
                        if (student != null)
                        {
                            ContentFrame.Navigate(new StudentPaymentPage(student.StudentId));
                        }
                    }
                    else if (pagePath == "StudentNotificationPage.xaml")
                    {
                        var student = _studentRepository.GetStudentByCode(_code);
                        if (student != null)
                        {
                            ContentFrame.Navigate(new StudentNotificationPage(student.StudentId));
                        }
                    }
                        else if (pagePath == "StudentInformationPage.xaml")
    {
        var student = _studentRepository.GetStudentByCode(_code);
        if (student != null)
        {
            ContentFrame.Navigate(new StudentInformationPage(student.StudentId));
        }
    }
    else if (pagePath == "StudentAssessmentPage.xaml")
    {
        var student = _studentRepository.GetStudentByCode(_code);
        if (student != null)
        {
            ContentFrame.Navigate(new StudentAssessmentPage(student.StudentId));
        }
    }
    else if (pagePath == "StudentCourseMaterialPage.xaml")
    {
        var student = _studentRepository.GetStudentByCode(_code);
        if (student != null)
        {
            ContentFrame.Navigate(new StudentCourseMaterialPage(student.StudentId));
        }
    }
    else if (pagePath == "StudentFeedbackPage.xaml")
    {
        var student = _studentRepository.GetStudentByCode(_code);
        if (student != null)
        {
        ContentFrame.Navigate(new StudentFeedbackPage(student.StudentId));
        }
    }
                    else if (pagePath == "StudentSettingPage.xaml")
                    {
                        var student = _studentRepository.GetStudentByCode(_code);
                        if (student != null)
                        {
                            ContentFrame.Navigate(new StudentSettingPage(student.StudentId));
                        }
                    }
                    else
    {
        // 👈 mặc định các page không cần tham số
        ContentFrame.Navigate(new Uri(pagePath, UriKind.Relative));
    }
}
                catch (Exception ex)
                {
                    MessageBox.Show("Trang này hiện không khả dụng", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
