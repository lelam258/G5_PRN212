
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

        public ICommand UploadCommand { get; }
        public ICommand SubmitCommand { get; }

        public StudentAssessmentViewModel(int studentId)
        {
            _studentId = studentId;
            _assessmentRepository = new AssessmentRepository();
            _assessmentResultRepository = new AssessmentResultRepository();
            UploadCommand = new RelayCommand(ExecuteUpload);
            SubmitCommand = new RelayCommand(ExecuteSubmit, CanExecuteSubmit);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                IsLoading = true;
                var allAssessments = _assessmentRepository.GetAllAssessments();
                var enrollments = ApplicationDbContext.Instance.Enrollments
                    .Where(e => e.StudentId == _studentId).ToList();
                var submittedResults = _assessmentResultRepository.GetAllAssessmentResults()
                    .Where(ar => ar.StudentId == _studentId && ar.SubmissionDate != null)
                    .Select(ar => ar.AssessmentId).ToList();

                Assessments = allAssessments
                    .Where(a => a.DueDate >= DateTime.Today)
                    .Where(a => enrollments.Any(e => e.CourseId == a.CourseId))
                    .Where(a => !submittedResults.Contains(a.AssessmentId))
                    .ToList();

                if (!Assessments.Any())
                {
                    StatusMessage = "No open assessments available for this student.";
                }
                else
                {
                    StatusMessage = "";
                }

                // Load previous results
                AssessmentResults = _assessmentResultRepository.GetAllAssessmentResults()
                    .Where(ar => ar.StudentId == _studentId && ar.SubmissionDate != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
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
                Filter = "PDF files (*.pdf)|*.pdf|Word files (*.doc;*.docx)|*.doc;*.docx|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                StatusMessage = "";
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
                    StatusMessage = "This assessment is past due.";
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

                // Save file
                string fileName = $"{_studentId}_{_selectedAssessment.AssessmentId}_{Path.GetFileName(_filePath)}";
                string destinationPath = Path.Combine("AppData/Submissions", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                File.Copy(_filePath, destinationPath, true);

                // Save to AssessmentResults
                var assessmentResult = new AssessmentResult
                {
                    AssessmentId = _selectedAssessment.AssessmentId,
                    StudentId = _studentId,
                    SubmissionDate = DateTime.Now,
                    SubmissionFilePath = destinationPath
                    // Score remains null until graded
                };

                _assessmentResultRepository.AddAssessmentResult(assessmentResult);
                StatusMessage = "Assessment submitted successfully!";

                // Reset UI
                SelectedAssessment = null;
                FilePath = "";
                LoadData();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error submitting assessment: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
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