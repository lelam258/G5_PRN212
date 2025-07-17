using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer;

namespace Presentation_Layer
{
    public partial class StudentCoursePage : Page
    {
        private readonly LifeSkillCourseDAO _courseDAO;
        private readonly Student _currentStudent;

        public StudentCoursePage(Student student)
        {
            InitializeComponent();
            _courseDAO = new LifeSkillCourseDAO();
            _currentStudent = student;
            LoadCourses();
        }

        private void LoadCourses()
        {
            var courses = _courseDAO.GetAllLifeSkillCourses()
                .Where(c => c.Status == "Mở đăng ký" && (c.MaxStudents == null || c.Enrollments?.Count < c.MaxStudents))
                .ToList();

            foreach (var course in courses)
            {
                course.CanRegister = HasPaidForCourse(course.CourseId) && (course.Enrollments?.All(e => e.StudentId != _currentStudent.StudentId) ?? true);
            }

            CourseListView.ItemsSource = courses;
        }

        private bool HasPaidForCourse(int courseId)
        {
            using (var context = ApplicationDbContext.Instance)
            {
                return context.Payments.Any(p => p.StudentId == _currentStudent.StudentId && p.CourseId == courseId && p.Status == "Đã thanh toán");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is LifeSkillCourse course)
            {
                var enrollment = new Enrollment
                {
                    StudentId = _currentStudent.StudentId,
                    CourseId = course.CourseId,
                    CompletionStatus = false,
                    CompletionDate = null
                };

                using (var context = ApplicationDbContext.Instance)
                {
                    context.Enrollments.Add(enrollment);
                    context.SaveChanges();
                }

                MessageBox.Show($"Đăng ký khóa học {course.CourseName} thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCourses(); // Refresh the list
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox)?.Text?.ToLower() ?? string.Empty;
            var courses = _courseDAO.GetAllLifeSkillCourses()
                .Where(c => c.Status == "Mở đăng ký" && (c.MaxStudents == null || c.Enrollments?.Count < c.MaxStudents)
                    && (c.CourseName.ToLower().Contains(searchText) || (c.Instructor?.InstructorName.ToLower().Contains(searchText) ?? false)))
                .ToList();

            foreach (var course in courses)
            {
                course.CanRegister = HasPaidForCourse(course.CourseId) && (course.Enrollments?.All(e => e.StudentId != _currentStudent.StudentId) ?? true);
            }

            CourseListView.ItemsSource = courses;
        }
    }
}