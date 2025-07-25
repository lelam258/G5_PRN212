using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Business_Layer;
using Repositories.Interfaces;
using Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Presentation_Layer
{
    public partial class StudentAssessmentPage : Page
    {
        public StudentAssessmentPage(int studentId)
        {
            InitializeComponent();
            DataContext = new StudentAssessmentViewModel(studentId);
        }
    }

    public class StudentAssessmentViewModel : INotifyPropertyChanged
    {
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IAssessmentResultRepository _assessmentResultRepository;
        private readonly int _studentId;
        private string _filePath;
        private Assessment _selectedAssessment;
        private string _statusMessage;
        private Brush _statusColor;
        private bool _isLoading;
        private List<Assessment> _assessments;
        private List<AssessmentResult> _assessmentResults;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Assessment> Assessments
        {
            get => _assessments;
            set
            {
                _assessments = value;
                OnPropertyChanged(nameof(Assessments));
            }
        }

        public Assessment SelectedAssessment
        {
            get => _selectedAssessment;
            set
            {
                _selectedAssessment = value;
                OnPropertyChanged(nameof(SelectedAssessment));
                OnPropertyChanged(nameof(IsDownloadEnabled));
                UpdateSubmitButtonState();
                StatusMessage = _selectedAssessment == null ? "Please select an assessment." : "";
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
                UpdateSubmitButtonState();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                StatusColor = string.IsNullOrEmpty(value) || value.Contains("successfully") ? Brushes.Green : Brushes.Red;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public List<AssessmentResult> AssessmentResults
        {
            get => _assessmentResults;
            set
            {
                _assessmentResults = value;
                OnPropertyChanged(nameof(AssessmentResults));
            }
        }

        public bool IsSubmitEnabled => _selectedAssessment != null && !string.IsNullOrEmpty(_filePath) && File.Exists(_filePath);

        public bool IsDownloadEnabled => _selectedAssessment != null && !string.IsNullOrEmpty(_selectedAssessment.FilePath);

        public ICommand UploadCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand DownloadAssessmentCommand { get; }

        public StudentAssessmentViewModel(int studentId)
        {
            _studentId = studentId;
            _assessmentRepository = new AssessmentRepository();
            _assessmentResultRepository = new AssessmentResultRepository();
            UploadCommand = new RelayCommand(ExecuteUpload);
            SubmitCommand = new RelayCommand(ExecuteSubmit, CanExecuteSubmit);
            DownloadAssessmentCommand = new RelayCommand(ExecuteDownloadAssessment, CanExecuteDownloadAssessment);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                IsLoading = true;

                // Get all assessments with related course information
                var allAssessments = ApplicationDbContext.Instance.Assessments
                    .Include(a => a.LifeSkillCourse)
                    .ToList();

                // Get student's enrollments
                var studentEnrollments = ApplicationDbContext.Instance.Enrollments
                    .Where(e => e.StudentId == _studentId)
                    .Select(e => e.CourseId)
                    .ToList();

                // Get already submitted assessments for this student
                var submittedAssessmentIds = ApplicationDbContext.Instance.AssessmentResults
                    .Where(ar => ar.StudentId == _studentId && ar.SubmissionDate != null)
                    .Select(ar => ar.AssessmentId)
                    .ToList();

                // Filter assessments: enrolled courses, not yet submitted, and not past due
                Assessments = allAssessments
                    .Where(a => studentEnrollments.Contains(a.CourseId)) // Student is enrolled
                    .Where(a => !submittedAssessmentIds.Contains(a.AssessmentId)) // Not yet submitted
                    .Where(a => a.DueDate >= DateTime.Today) // Not past due
                    .OrderBy(a => a.DueDate)
                    .ToList();

                if (!Assessments.Any())
                {
                    StatusMessage = "No open assessments available for submission.";
                }
                else
                {
                    StatusMessage = $"Found {Assessments.Count} assessment(s) available for submission.";
                }

                // Load previous assessment results with related data
                AssessmentResults = ApplicationDbContext.Instance.AssessmentResults
                    .Include(ar => ar.Assessment)
                    .Where(ar => ar.StudentId == _studentId)
                    .OrderByDescending(ar => ar.SubmissionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"LoadData Error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteUpload(object parameter)
        {
            if (_selectedAssessment == null)
            {
                StatusMessage = "Please select an assessment first.";
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "ZIP files (*.zip)|*.zip",
                Title = "Select Assessment ZIP File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(selectedFile);

                // Check file size (30MB = 30 * 1024 * 1024 bytes)
                long maxSize = 30 * 1024 * 1024;
                if (fileInfo.Length > maxSize)
                {
                    StatusMessage = "File size exceeds 30MB limit. Please select a smaller file.";
                    return;
                }

                FilePath = selectedFile;
                StatusMessage = $"File selected: {Path.GetFileName(FilePath)}";
            }
        }

        private bool CanExecuteSubmit(object parameter) => IsSubmitEnabled;

        private void ExecuteSubmit(object parameter)
        {
            try
            {
                IsLoading = true;

                if (_selectedAssessment == null)
                {
                    StatusMessage = "Please select an assessment.";
                    return;
                }

                if (string.IsNullOrEmpty(_filePath) || !File.Exists(_filePath))
                {
                    StatusMessage = "Please upload a valid file.";
                    return;
                }

                if (_selectedAssessment.DueDate < DateTime.Today)
                {
                    StatusMessage = "This assessment is past due and cannot be submitted.";
                    return;
                }

                // Verify enrollment
                bool isEnrolled = ApplicationDbContext.Instance.Enrollments
                    .Any(e => e.StudentId == _studentId && e.CourseId == _selectedAssessment.CourseId);

                if (!isEnrolled)
                {
                    StatusMessage = "You are not enrolled in this course.";
                    return;
                }

                // Check if already submitted
                bool alreadySubmitted = ApplicationDbContext.Instance.AssessmentResults
                    .Any(ar => ar.StudentId == _studentId && ar.AssessmentId == _selectedAssessment.AssessmentId && ar.SubmissionDate != null);

                if (alreadySubmitted)
                {
                    StatusMessage = "You have already submitted this assessment.";
                    return;
                }

                // Create submissions directory if it doesn't exist
                string submissionsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "Submissions");
                Directory.CreateDirectory(submissionsDir);

                // Generate unique filename
                string fileExtension = Path.GetExtension(_filePath);
                string fileName = $"{_studentId}_{_selectedAssessment.AssessmentId}_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";
                string destinationPath = Path.Combine(submissionsDir, fileName);

                // Copy file to submissions directory
                File.Copy(_filePath, destinationPath, true);

                // Create assessment result record
                var assessmentResult = new AssessmentResult
                {
                    AssessmentId = _selectedAssessment.AssessmentId,
                    StudentId = _studentId,
                    SubmissionDate = DateTime.Now,
                    SubmissionFilePath = destinationPath
                    // Score remains null until graded by instructor
                };

                _assessmentResultRepository.AddAssessmentResult(assessmentResult);

                StatusMessage = $"Assessment '{_selectedAssessment.AssessmentName}' submitted successfully!";

                // Reset form
                SelectedAssessment = null;
                FilePath = "";

                // Reload data to refresh the lists
                LoadData();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error submitting assessment: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Submit Error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanExecuteDownloadAssessment(object parameter) => IsDownloadEnabled;

        private void ExecuteDownloadAssessment(object parameter)
        {
            try
            {
                if (_selectedAssessment == null)
                {
                    StatusMessage = "Please select an assessment first.";
                    return;
                }

                if (string.IsNullOrEmpty(_selectedAssessment.FilePath))
                {
                    StatusMessage = "No assessment file available for download.";
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Attempting to download: {_selectedAssessment.FilePath}");

                if (!File.Exists(_selectedAssessment.FilePath))
                {
                    StatusMessage = $"Assessment file not found. Please contact your instructor.";
                    System.Diagnostics.Debug.WriteLine($"File not found: {_selectedAssessment.FilePath}");
                    return;
                }

                // Check if it's a ZIP file
                string extension = Path.GetExtension(_selectedAssessment.FilePath).ToLower();
                if (extension != ".zip")
                {
                    StatusMessage = "Only ZIP files are supported for download.";
                    return;
                }

                // Open file with default application
                var processInfo = new ProcessStartInfo
                {
                    FileName = _selectedAssessment.FilePath,
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(processInfo);
                StatusMessage = $"Opening assessment file: {Path.GetFileName(_selectedAssessment.FilePath)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error downloading assessment file: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Download Error: {ex}");
            }
        }

        private void UpdateSubmitButtonState()
        {
            OnPropertyChanged(nameof(IsSubmitEnabled));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // RelayCommand for ICommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}