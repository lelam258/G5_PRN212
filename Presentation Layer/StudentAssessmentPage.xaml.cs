using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer;
using Microsoft.Win32;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>  
    /// Interaction logic for StudentAssessmentPage.xaml  
    /// </summary>  
    public partial class StudentAssessmentPage : Page
    {
        private readonly AssessmentRepository _assessmentRepository;
        private readonly AssessmentResultRepository _assessmentResultRepository;
        private readonly ApplicationDbContext _context;
        private readonly int _currentStudentId; // Assume this is set from login session  
        private readonly Notification _notification; // For adding notification  

        public List<AssessmentResultViewModel> AssessmentResults { get; set; }

        public StudentAssessmentPage(int studentId)
        {
            InitializeComponent();
            _assessmentRepository = new AssessmentRepository();
            _assessmentResultRepository = new AssessmentResultRepository();
            _context = ApplicationDbContext.Instance;
            _currentStudentId = studentId;
            _notification = new Notification
            {
                Title = string.Empty, // Initialize required property  
                Content = string.Empty // Initialize required property  
            };
            LoadAssessments();
            LoadAssessmentResults();
            DataContext = this;
        }

        private void LoadAssessments()
        {
            var assessments = _context.Assessments
                .Where(a => a.LifeSkillCourse.Enrollments.Any(e => e.StudentId == _currentStudentId))
                .ToList();
            AssessmentComboBox.ItemsSource = assessments;
        }

        private void LoadAssessmentResults()
        {
            AssessmentResults = _context.AssessmentResults
                .Where(ar => ar.StudentId == _currentStudentId)
                .Select(ar => new AssessmentResultViewModel
                {
                    ResultId = ar.ResultId,
                    AssessmentId = ar.AssessmentId,
                    StudentId = ar.StudentId,
                    Score = ar.Score,
                    SubmissionDate = ar.SubmissionDate,
                    Assessment = ar.Assessment
                }).ToList();
            ScoresDataGrid.ItemsSource = AssessmentResults;
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new() // Simplified 'new' expression  
            {
                Filter = "All files (*.*)|*.*",
                Title = "Select a File"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
                SubmitButton.IsEnabled = true;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssessmentComboBox.SelectedItem is Assessment selectedAssessment)
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime > selectedAssessment.DueDate)
                {
                    StatusTextBlock.Text = "Submission failed: Deadline has passed.";
                    return;
                }

                var assessmentResult = new AssessmentResult
                {
                    AssessmentId = selectedAssessment.AssessmentId,
                    StudentId = _currentStudentId,
                    Score = null, // Score will be set by instructor later  
                    SubmissionDate = currentTime
                };

                _assessmentResultRepository.AddAssessmentResult(assessmentResult);
                LoadAssessmentResults();

                // Add notification  
                _notification.Title = "Assessment Submitted";
                _notification.Content = $"You have successfully submitted {selectedAssessment.AssessmentName} on {currentTime:dd/MM/yyyy HH:mm}.";
                _notification.StudentId = _currentStudentId;
                _notification.CreatedDate = currentTime;
                _context.Notifications.Add(_notification);
                _context.SaveChanges();

                StatusTextBlock.Text = "Assessment submitted successfully!";
                StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                FilePathTextBox.Text = "";
                SubmitButton.IsEnabled = false;
            }
        }
    }

    public class AssessmentResultViewModel
    {
        public int ResultId { get; set; }
        public int AssessmentId { get; set; }
        public int StudentId { get; set; }
        public decimal? Score { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public Assessment? Assessment { get; set; }
    }
}