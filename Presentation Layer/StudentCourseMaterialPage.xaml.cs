using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Business_Layer;
using Data_Layer;
using Microsoft.Win32;
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class StudentCourseMaterialPage : Page
    {
        public StudentCourseMaterialPage(int studentId)
        {
            InitializeComponent();
            if (studentId <= 0) throw new ArgumentException("Invalid student ID.", nameof(studentId));
            DataContext = new StudentCourseMaterialViewModel(studentId);
        }
    }

    public class StudentCourseMaterialViewModel : INotifyPropertyChanged
    {
        private readonly ICourseMaterialRepository _courseMaterialRepository;
        private readonly int _studentId;
        private ObservableCollection<CourseMaterial> _courseMaterials = new ObservableCollection<CourseMaterial>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CourseMaterial> CourseMaterials
        {
            get => _courseMaterials;
            set
            {
                _courseMaterials = value;
                OnPropertyChanged(nameof(CourseMaterials));
            }
        }

        public ICommand DownloadCommand { get; }

        public StudentCourseMaterialViewModel(int studentId)
        {
            _studentId = studentId;
            _courseMaterialRepository = new CourseMaterialRepository() ?? throw new InvalidOperationException("Failed to initialize CourseMaterialRepository.");
            DownloadCommand = new RelayCommand<CourseMaterial>(ExecuteDownload);
            LoadCourseMaterials();
        }

        private void LoadCourseMaterials()
        {
            try
            {
                var context = ApplicationDbContext.Instance;
                if (context == null) throw new InvalidOperationException("ApplicationDbContext is not initialized.");

                var enrollments = context.Enrollments
                    ?.Where(e => e.StudentId == _studentId)
                    .Select(e => e.CourseId)
                    .ToList() ?? new List<int>();

                if (!enrollments.Any())
                {
                    MessageBox.Show($"No enrolled courses found for Student ID {_studentId}.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CourseMaterials.Clear();
                    return;
                }

                var materials = _courseMaterialRepository.GetAllCourseMaterials()
                    ?.Where(cm => enrollments.Contains(cm.CourseId))
                    .ToList() ?? new List<CourseMaterial>();

                CourseMaterials.Clear();
                foreach (var material in materials)
                {
                    System.Diagnostics.Debug.WriteLine($"Loaded material: {material.Title}, FilePath: {material.FilePath}");
                    CourseMaterials.Add(material);
                }

                if (!CourseMaterials.Any())
                {
                    MessageBox.Show($"No course materials found for enrolled courses of Student ID {_studentId}.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading course materials: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error loading course materials: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CourseMaterials.Clear();
            }
        }

        private void ExecuteDownload(CourseMaterial material)
        {
            if (material == null) throw new ArgumentNullException(nameof(material));

            try
            {
                if (string.IsNullOrEmpty(material.FilePath))
                {
                    MessageBox.Show("File path is empty or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Attempting to download file: {material.FilePath}");

                // Adjust this base path based on where your files are stored
                string basePath = @"C:\CourseMaterials\"; // Example: Local path or network share like @"\\server\shared\course_materials\"
                string fullPath = Path.Combine(basePath, material.FilePath.TrimStart('\\', '/'));

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show($"File not found at: {fullPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    FileName = Path.GetFileName(material.FilePath),
                    Filter = "All Files (*.*)|*.*",
                    Title = "Save Course Material",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(fullPath, saveFileDialog.FileName, true);
                    MessageBox.Show("File downloaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error downloading file: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => parameter is T && (_canExecute == null || _canExecute((T)parameter));

        public void Execute(object parameter)
        {
            if (parameter is T item) _execute(item);
            else throw new ArgumentException("Parameter is not of the expected type.", nameof(parameter));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}