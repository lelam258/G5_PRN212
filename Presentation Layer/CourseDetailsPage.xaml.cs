using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for CourseDetailsPage.xaml
    /// </summary>
    public partial class CourseDetailsPage : Page
    {
        private readonly ILifeSkillCourseRepository _courseRepository;
        private readonly ApplicationDbContext _context;
        private readonly int _courseId;
        private readonly int _studentId;

        public LifeSkillCourse Course { get; set; }
        public System.Collections.Generic.List<object> AssessmentResults { get; set; }

        public CourseDetailsPage(int courseId, int studentId)
        {
            InitializeComponent();
            _courseId = courseId;
            _studentId = studentId;
            _context = ApplicationDbContext.Instance;
            _courseRepository = new LifeSkillCourseRepository();
            LoadCourseDetails();
            DataContext = this;
        }

        private void LoadCourseDetails()
        {
            try
            {
                // Load course with instructor
                Course = _context.LifeSkillCourses
                    .Include(c => c.Instructor)
                    .FirstOrDefault(c => c.CourseId == _courseId);

                if (Course == null)
                {
                    MessageBox.Show("Course not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Load assessment results for the student
                AssessmentResults = _context.AssessmentResults
                    .Where(ar => ar.StudentId == _studentId && ar.Assessment.CourseId == _courseId)
                    .Join(_context.Assessments,
                          ar => ar.AssessmentId,
                          a => a.AssessmentId,
                          (ar, a) => new
                          {
                              AssessmentName = a.AssessmentName,
                              AssessmentType = a.AssessmentType,
                              Score = ar.Score,
                              MaxScore = a.MaxScore,
                              SubmissionDate = ar.SubmissionDate,
                              DueDate = a.DueDate
                          })
                    .ToList<object>();

                // Update UI
                AssessmentResultsList.ItemsSource = AssessmentResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading course details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new EnrolledCoursePage());
        }
    }
}