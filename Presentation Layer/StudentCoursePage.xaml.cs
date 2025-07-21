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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Business_Layer;
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for StudentCoursePage.xaml
    /// </summary>
    public partial class StudentCoursePage : Page
    {
        private readonly ILifeSkillCourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository; // Assume this exists
        private readonly ApplicationDbContext _context; // Inject context
        private int _currentStudentId = 1; // Example student ID, replace with logged-in user ID

        public StudentCoursePage()
        {
            InitializeComponent();
            _context = ApplicationDbContext.Instance; // Use singleton instance
            _courseRepository = new LifeSkillCourseRepository();
            _enrollmentRepository = new EnrollmentRepository(); // Assume this exists
            LoadCourses();
        }

        private void LoadCourses()
        {
            var allCourses = _courseRepository.GetAllLifeSkillCourses();
            var openCourses = (from course in allCourses
                               join enrollment in _context.Enrollments on course.CourseId equals enrollment.CourseId into enrollmentsGroup
                               where course.Status == "Mở đăng ký"
                               select new
                               {
                                   CourseId = course.CourseId,
                                   CourseName = course.CourseName,
                                   Instructor = course.Instructor,
                                   StartDate = course.StartDate,
                                   EndDate = course.EndDate,
                                   Price = course.Price,
                                   MaxStudents = course.MaxStudents,
                                   AvailableSpots = course.MaxStudents - enrollmentsGroup.Count(),
                                   CanRegister = course.MaxStudents > enrollmentsGroup.Count() && HasPaidForCourse(course.CourseId)
                               }).ToList();

            CourseList.ItemsSource = openCourses;
        }

        private bool HasPaidForCourse(int courseId)
        {
            // Placeholder logic - implement actual payment check using PaymentRepository
            // This should query Payments table to check if student has paid for the course
            return true; // Replace with actual logic
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Safely cast DataContext to dynamic to access CourseId
                dynamic dataContext = button.DataContext;
                if (dataContext != null)
                {
                    int courseId = dataContext.CourseId;
                    var enrollment = new Enrollment { StudentId = _currentStudentId, CourseId = courseId, CompletionStatus = false };
                    // Placeholder: Implement enrollment logic using _enrollmentRepository
                    _enrollmentRepository.AddEnrollment(enrollment); // Assume this method exists
                    MessageBox.Show($"Registered for course ID {courseId} successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCourses(); // Refresh list
                }
            }
        }
    }
}

