using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for AdminBackupRestorePage.xaml
    /// </summary>
    public partial class AdminBackupRestorePage : Page
    {
        private readonly ApplicationDbContext _context;

        public AdminBackupRestorePage()
        {
            InitializeComponent();
            _context = ApplicationDbContext.Instance ?? throw new InvalidOperationException("Database context is not initialized.");
            ValidateDatabaseConnection();
        }

        private void ValidateDatabaseConnection()
        {
            try
            {
                _context.Database.CanConnect();
                StatusText.Text = "Database connection is active.";
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Database connection failed: {ex.Message}";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
                BackupButton.IsEnabled = false;
                RestoreButton.IsEnabled = false;
            }
        }

        private async void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save Backup File",
                FileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ProgressBar.Visibility = Visibility.Visible;
                StatusText.Text = "Initializing backup...";
                await Task.Delay(100); // Brief delay for UI feedback

                try
                {
                    var backupData = await GetBackupData();
                    StatusText.Text = "Serializing data to JSON...";
                    var jsonString = JsonSerializer.Serialize(backupData, new JsonSerializerOptions { WriteIndented = true });
                    StatusText.Text = "Saving file...";
                    await File.WriteAllTextAsync(saveFileDialog.FileName, jsonString);
                    StatusText.Text = $"Backup completed successfully at {DateTime.Now:HH:mm:ss}.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Green;
                }
                catch (UnauthorizedAccessException)
                {
                    StatusText.Text = "Permission denied. Please run as administrator or check file access.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"Backup failed: {ex.Message}";
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                }
                finally
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Select Backup File to Restore"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var result = MessageBox.Show(
                    "Restoring will overwrite all existing data. This action cannot be undone. Proceed?",
                    "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                ProgressBar.Visibility = Visibility.Visible;
                StatusText.Text = "Validating backup file...";
                await Task.Delay(100);

                try
                {
                    var jsonString = await File.ReadAllTextAsync(openFileDialog.FileName);
                    var backupData = JsonSerializer.Deserialize<BackupData>(jsonString);
                    if (backupData == null || !IsBackupDataValid(backupData))
                    {
                        throw new InvalidDataException("Invalid or corrupted backup file.");
                    }

                    StatusText.Text = "Preparing to restore data...";
                    await RestoreData(backupData);
                    StatusText.Text = $"Restore completed successfully at {DateTime.Now:HH:mm:ss}.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Green;
                }
                catch (UnauthorizedAccessException)
                {
                    StatusText.Text = "Permission denied. Please check file access or run as administrator.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                }
                catch (InvalidDataException ex)
                {
                    StatusText.Text = $"Restore failed: {ex.Message}";
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"Restore failed: {ex.Message}";
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                }
                finally
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async Task<BackupData> GetBackupData()
        {
            return new BackupData
            {
                Students = await _context.Students.ToListAsync(),
                Instructors = await _context.Instructors.ToListAsync(),
                LifeSkillCourses = await _context.LifeSkillCourses.ToListAsync(),
                Enrollments = await _context.Enrollments.ToListAsync(),
                Assessments = await _context.Assessments.ToListAsync(),
                AssessmentResults = await _context.AssessmentResults.ToListAsync(),
                Notifications = await _context.Notifications.ToListAsync(),
                CourseMaterials = await _context.CourseMaterials.ToListAsync(),
                Feedbacks = await _context.Feedbacks.ToListAsync(),
                Certificates = await _context.Certificates.ToListAsync(),
                CourseSchedules = await _context.CourseSchedules.ToListAsync(),
                ActivityLogs = await _context.ActivityLogs.ToListAsync(),
                Payments = await _context.Payments.ToListAsync()
            };
        }

        private async Task RestoreData(BackupData backupData)
        {
            try
            {
                // Begin transaction to ensure atomicity
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Clear existing data
                    _context.Students.RemoveRange(_context.Students);
                    _context.Instructors.RemoveRange(_context.Instructors);
                    _context.LifeSkillCourses.RemoveRange(_context.LifeSkillCourses);
                    _context.Enrollments.RemoveRange(_context.Enrollments);
                    _context.Assessments.RemoveRange(_context.Assessments);
                    _context.AssessmentResults.RemoveRange(_context.AssessmentResults);
                    _context.Notifications.RemoveRange(_context.Notifications);
                    _context.CourseMaterials.RemoveRange(_context.CourseMaterials);
                    _context.Feedbacks.RemoveRange(_context.Feedbacks);
                    _context.Certificates.RemoveRange(_context.Certificates);
                    _context.CourseSchedules.RemoveRange(_context.CourseSchedules);
                    _context.ActivityLogs.RemoveRange(_context.ActivityLogs);
                    _context.Payments.RemoveRange(_context.Payments);
                    await _context.SaveChangesAsync();

                    // Restore data
                    await _context.Students.AddRangeAsync(backupData.Students);
                    await _context.Instructors.AddRangeAsync(backupData.Instructors);
                    await _context.LifeSkillCourses.AddRangeAsync(backupData.LifeSkillCourses);
                    await _context.Enrollments.AddRangeAsync(backupData.Enrollments);
                    await _context.Assessments.AddRangeAsync(backupData.Assessments);
                    await _context.AssessmentResults.AddRangeAsync(backupData.AssessmentResults);
                    await _context.Notifications.AddRangeAsync(backupData.Notifications);
                    await _context.CourseMaterials.AddRangeAsync(backupData.CourseMaterials);
                    await _context.Feedbacks.AddRangeAsync(backupData.Feedbacks);
                    await _context.Certificates.AddRangeAsync(backupData.Certificates);
                    await _context.CourseSchedules.AddRangeAsync(backupData.CourseSchedules);
                    await _context.ActivityLogs.AddRangeAsync(backupData.ActivityLogs);
                    await _context.Payments.AddRangeAsync(backupData.Payments);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database update error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private bool IsBackupDataValid(BackupData backupData)
        {
            return backupData.Students != null && backupData.Instructors != null && backupData.LifeSkillCourses != null &&
                   backupData.Enrollments != null && backupData.Assessments != null && backupData.AssessmentResults != null &&
                   backupData.Notifications != null && backupData.CourseMaterials != null && backupData.Feedbacks != null &&
                   backupData.Certificates != null && backupData.CourseSchedules != null && backupData.ActivityLogs != null &&
                   backupData.Payments != null;
        }
    }

    public class BackupData
    {
        public List<Student> Students { get; set; }
        public List<Instructor> Instructors { get; set; }
        public List<LifeSkillCourse> LifeSkillCourses { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Assessment> Assessments { get; set; }
        public List<AssessmentResult> AssessmentResults { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<CourseMaterial> CourseMaterials { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public List<Certificate> Certificates { get; set; }
        public List<CourseSchedule> CourseSchedules { get; set; }
        public List<ActivityLog> ActivityLogs { get; set; }
        public List<Payment> Payments { get; set; }
    }
}