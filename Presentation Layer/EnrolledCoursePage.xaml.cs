using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer; // For ApplicationDbContext
using Microsoft.EntityFrameworkCore; // For Include method
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for EnrolledCoursePage.xaml
    /// </summary>
    public partial class EnrolledCoursePage : Page
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILifeSkillCourseRepository _courseRepository;
        private readonly ApplicationDbContext _context; // Inject context
        private int _currentStudentId = 1; // Example student ID, replace with logged-in user ID

        public EnrolledCoursePage()
        {
            InitializeComponent();
            _context = ApplicationDbContext.Instance; // Use singleton instance
            _enrollmentRepository = new EnrollmentRepository();
            _courseRepository = new LifeSkillCourseRepository();
            LoadEnrolledCourses();
        }

        private void LoadEnrolledCourses()
        {
            var enrollments = _enrollmentRepository.GetAllEnrollments()
                .Where(e => e.StudentId == _currentStudentId)
                .Join(_context.LifeSkillCourses.Include(c => c.Instructor), // Eagerly load Instructor
                      e => e.CourseId,
                      c => c.CourseId,
                      (e, c) => new
                      {
                          EnrollmentId = e.EnrollmentId, // Include EnrollmentId for cancellation
                          CourseId = c.CourseId,
                          CourseName = c.CourseName,
                          Instructor = c.Instructor,
                          StartDate = c.StartDate,
                          EndDate = c.EndDate,
                          CompletionStatus = e.CompletionStatus,
                          ProgressPercentage = CalculateProgress(e) // Placeholder for progress calculation
                      }).ToList();

            EnrolledCourseList.ItemsSource = enrollments;
        }

        private double CalculateProgress(Enrollment enrollment)
        {
            // Placeholder logic: Assume 100% if completed, 0% if not
            // For a real implementation, join with AssessmentResults to calculate based on scores
            return enrollment.CompletionStatus ? 100.0 : 0.0;
        }

        private void CancelCourse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected enrollment from the button's DataContext
                var button = sender as Button;
                var enrollment = button?.DataContext as dynamic;
                if (enrollment == null)
                {
                    MessageBox.Show("Please select a course to cancel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Confirm cancellation with the user
                var result = MessageBox.Show($"Are you sure you want to cancel the course '{enrollment.CourseName}'?",
                                             "Confirm Cancellation",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                // Delete the enrollment using the repository
                _enrollmentRepository.DeleteEnrollment(enrollment.EnrollmentId);

                // Refresh the course list
                LoadEnrolledCourses();

                MessageBox.Show("Course canceled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while canceling the course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected enrollment from the button's DataContext
                var button = sender as Button;
                var enrollment = button?.DataContext as dynamic;
                if (enrollment == null)
                {
                    MessageBox.Show("Please select a course to view details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Navigate to the CourseDetailsPage, passing the CourseId
                var detailsPage = new CourseDetailsPage(enrollment.CourseId, _currentStudentId);
                NavigationService?.Navigate(detailsPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading course details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}