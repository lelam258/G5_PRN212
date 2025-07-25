using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Repositories.Repositories;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Presentation_Layer
{
    public partial class AdminGradingPage : Page
    {
        private readonly AssessmentResultRepository _resultRepo = new();
        private List<AssessmentResult> _allResults = new();

        public AdminGradingPage()
        {
            InitializeComponent();
            LoadResults();
        }

        private void LoadResults()
        {
            try
            {
                // Lấy tất cả AssessmentResult với đầy đủ navigation properties
                _allResults = ApplicationDbContext.Instance.AssessmentResults
                    .Include(ar => ar.Student)
                    .Include(ar => ar.Assessment)
                    .ThenInclude(a => a.LifeSkillCourse)
                    .Where(ar => ar.SubmissionDate != null) // Chỉ lấy những bài đã nộp
                    .OrderByDescending(ar => ar.SubmissionDate)
                    .ToList();

                ResultsDataGrid.ItemsSource = _allResults;

                if (!_allResults.Any())
                {
                    MessageBox.Show("Chưa có bài nộp nào để chấm điểm.", "Thông báo",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"LoadResults Error: {ex}");
            }
        }

        private void ResultsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem is AssessmentResult ar)
            {
                AssessmentIdTextBox.Text = ar.AssessmentId.ToString();
                StudentCodeTextBox.Text = ar.Student?.StudentCode ?? "N/A";
                ScoreTextBox.Text = ar.Score?.ToString() ?? "";

                // Format submission date
                if (ar.SubmissionDate.HasValue)
                    SubmissionDateTextBox.Text = ar.SubmissionDate.Value.ToString("dd/MM/yyyy HH:mm");
                else
                    SubmissionDateTextBox.Text = "";

                // Debug: In ra file path để kiểm tra
                if (!string.IsNullOrEmpty(ar.SubmissionFilePath))
                {
                    System.Diagnostics.Debug.WriteLine($"File Path: {ar.SubmissionFilePath}");
                    System.Diagnostics.Debug.WriteLine($"File Exists: {File.Exists(ar.SubmissionFilePath)}");
                }
            }
            else
            {
                ClearForm();
            }
        }

        private void ClearForm()
        {
            AssessmentIdTextBox.Clear();
            StudentCodeTextBox.Clear();
            ScoreTextBox.Clear();
            SubmissionDateTextBox.Clear();
        }

        private void SaveScoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem is not AssessmentResult ar)
            {
                MessageBox.Show("Vui lòng chọn một bài nộp để chấm điểm.", "Cảnh báo",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate điểm
            if (!decimal.TryParse(ScoreTextBox.Text.Trim(), out var newScore) || newScore < 0)
            {
                MessageBox.Show("Điểm phải là một số hợp lệ và không âm.", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra điểm không vượt quá MaxScore của Assessment
            if (ar.Assessment != null && newScore > ar.Assessment.MaxScore)
            {
                MessageBox.Show($"Điểm không được vượt quá điểm tối đa ({ar.Assessment.MaxScore}).",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                ar.Score = newScore;
                _resultRepo.UpdateAssessmentResult(ar);

                MessageBox.Show("Đã lưu điểm thành công.", "Thông báo",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh data
                LoadResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu điểm: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"SaveScore Error: {ex}");
            }
        }

        private void DownloadSubmissionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = null;

                // Cách 1: Lấy từ Tag của button
                if (sender is Button btn && btn.Tag is string tagPath)
                {
                    filePath = tagPath;
                }
                // Cách 2: Lấy từ selected item
                else if (ResultsDataGrid.SelectedItem is AssessmentResult ar)
                {
                    filePath = ar.SubmissionFilePath;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Không tìm thấy đường dẫn file.", "Lỗi",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Attempting to open file: {filePath}");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File không tồn tại tại đường dẫn:\n{filePath}\n\nFile có thể đã bị di chuyển hoặc xóa.",
                                  "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra có phải file ZIP không
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension != ".zip")
                {
                    MessageBox.Show("Chỉ hỗ trợ mở file ZIP.", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Mở file với ứng dụng mặc định
                var processInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"DownloadSubmission Error: {ex}");
            }
        }

        // Method để refresh data khi cần
        public void RefreshData()
        {
            LoadResults();
        }

        // Method để filter kết quả theo assessment ID
        private void FilterByAssessment(int assessmentId)
        {
            var filtered = _allResults.Where(ar => ar.AssessmentId == assessmentId).ToList();
            ResultsDataGrid.ItemsSource = filtered;
        }

        // Method để hiển thị tất cả kết quả
        private void ShowAllResults()
        {
            ResultsDataGrid.ItemsSource = _allResults;
        }
    }
}