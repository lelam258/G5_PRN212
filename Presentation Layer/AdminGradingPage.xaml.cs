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
            // Lấy tất cả AssessmentResult kèm Student nav prop
            _allResults = _resultRepo.GetAllAssessmentResults();
            ResultsDataGrid.ItemsSource = _allResults;
        }

        private void ResultsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem is AssessmentResult ar)
            {
                AssessmentIdTextBox.Text = ar.AssessmentId.ToString();
                StudentCodeTextBox.Text = ar.Student?.StudentCode ?? "";
                ScoreTextBox.Text = ar.Score.ToString();

                // SubmissionDate phải là DateTime, nên ToString(format) sẽ hợp lệ
                if (ar.SubmissionDate.HasValue)
                    SubmissionDateTextBox.Text = ar.SubmissionDate.Value
                                                          .ToString("dd/MM/yyyy HH:mm");
                else
                    SubmissionDateTextBox.Text = "";
            }
            else
            {
                AssessmentIdTextBox.Clear();
                StudentCodeTextBox.Clear();
                ScoreTextBox.Clear();
                SubmissionDateTextBox.Clear();
            }
        }

        private void SaveScoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem is not AssessmentResult ar)
                return;

            // Validate điểm
            if (!decimal.TryParse(ScoreTextBox.Text.Trim(), out var newScore))
            {
                MessageBox.Show("Điểm phải là một số hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ar.Score = newScore;
            _resultRepo.UpdateAssessmentResult(ar);

            MessageBox.Show("✅ Đã lưu điểm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadResults();
        }

        private void DownloadSubmissionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string path && File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Không tìm thấy file bài làm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
