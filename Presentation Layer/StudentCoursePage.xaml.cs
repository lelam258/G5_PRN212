using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer; // For ApplicationDbContext
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
                                   AvailableSpots = course.MaxStudents - enrollmentsGroup.Count()
                               }).ToList();

            CourseList.ItemsSource = openCourses;
        }

        private bool HasPaidForCourse(int courseId)
        {
            // Placeholder logic - implement actual payment check using PaymentRepository
            // This should query Payments table to check if student has paid for the course
            return true; // Replace with actual logic
        }

        private bool IsStudentAlreadyEnrolled(int courseId, int studentId)
        {
            // Check if the student is already enrolled in the course
            return _context.Enrollments.Any(e => e.CourseId == courseId && e.StudentId == studentId);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                dynamic dataContext = button.DataContext;
                if (dataContext != null)
                {
                    int courseId = dataContext.CourseId;
                    var course = _courseRepository.GetLifeSkillCourseById(courseId);
                    var enrollmentsCount = _context.Enrollments.Count(e => e.CourseId == courseId);

                    // Check if the student is already enrolled
                    if (IsStudentAlreadyEnrolled(courseId, _currentStudentId))
                    {
                        MessageBox.Show("You are already registered for this course!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Check if there are available spots and payment is completed
                    if (course.MaxStudents > enrollmentsCount && HasPaidForCourse(courseId))
                    {
                        var enrollment = new Enrollment { StudentId = _currentStudentId, CourseId = courseId, CompletionStatus = false };
                        _enrollmentRepository.AddEnrollment(enrollment); // Assume this method exists
                        MessageBox.Show($"Registered for course ID {courseId} successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCourses(); // Refresh list
                    }
                    else
                    {
                        MessageBox.Show("No available spots or payment not completed!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
    }
}