using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Business_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class AdminFeedbackPage : Page
    {
        private readonly FeedbackRepository _feedbackRepository;
        private List<FeedbackViewModel> _allFeedbacks;
        private List<FeedbackViewModel> _filteredFeedbacks;

        public AdminFeedbackPage()
        {
            InitializeComponent();
            _feedbackRepository = new FeedbackRepository();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load all feedbacks with related data
                var feedbacks = _feedbackRepository.GetAllFeedbacks();
                _allFeedbacks = feedbacks.Select(f => new FeedbackViewModel
                {
                    FeedbackId = f.FeedbackId,
                    StudentName = f.Student?.StudentName ?? "N/A",
                    CourseName = f.LifeSkillCourse?.CourseName ?? "N/A",
                    Rating = f.Rating,
                    Comment = f.Comment ?? "",
                    FeedbackDate = f.FeedbackDate
                }).ToList();

                _filteredFeedbacks = new List<FeedbackViewModel>(_allFeedbacks);

                // Load filters
                LoadCourseFilter();
                LoadMonthFilter();

                // Display data
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCourseFilter()
        {
            var courses = _allFeedbacks.Select(f => f.CourseName).Distinct().ToList();
            courses.Insert(0, "Tất cả khóa học");

            CourseFilterComboBox.ItemsSource = courses;
            CourseFilterComboBox.SelectedIndex = 0;
        }

        private void LoadMonthFilter()
        {
            var months = new List<string>
            {
                "Tất cả tháng",
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            };

            MonthFilterComboBox.ItemsSource = months;
            MonthFilterComboBox.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            _filteredFeedbacks = new List<FeedbackViewModel>(_allFeedbacks);

            // Course filter
            if (CourseFilterComboBox.SelectedItem != null &&
                CourseFilterComboBox.SelectedItem.ToString() != "Tất cả khóa học")
            {
                var selectedCourse = CourseFilterComboBox.SelectedItem.ToString();
                _filteredFeedbacks = _filteredFeedbacks.Where(f => f.CourseName == selectedCourse).ToList();
            }

            // Month filter
            if (MonthFilterComboBox.SelectedItem != null &&
                MonthFilterComboBox.SelectedIndex > 0)
            {
                var selectedMonth = MonthFilterComboBox.SelectedIndex;
                _filteredFeedbacks = _filteredFeedbacks.Where(f => f.FeedbackDate.Month == selectedMonth).ToList();
            }
        }

        private void RefreshDisplay()
        {
            ApplyFilters();
            FeedbackDataGrid.ItemsSource = _filteredFeedbacks;
            DrawRatingChart();
            DisplayAverageRatings();
        }

        private void DrawRatingChart()
        {
            RatingChartCanvas.Children.Clear();

            if (!_filteredFeedbacks.Any()) return;

            // Count ratings 1-10
            var ratingCounts = new int[11]; // Index 0 unused, 1-10 for ratings
            foreach (var feedback in _filteredFeedbacks)
            {
                if (feedback.Rating >= 1 && feedback.Rating <= 10)
                    ratingCounts[feedback.Rating]++;
            }

            // Draw bar chart
            double canvasWidth = RatingChartCanvas.ActualWidth > 0 ? RatingChartCanvas.ActualWidth : 400;
            double canvasHeight = RatingChartCanvas.ActualHeight > 0 ? RatingChartCanvas.ActualHeight : 200;

            if (canvasWidth <= 0) canvasWidth = 400;
            if (canvasHeight <= 0) canvasHeight = 200;

            double barWidth = (canvasWidth - 100) / 10;
            double maxCount = ratingCounts.Max();

            if (maxCount == 0) return;

            var colors = new Brush[]
            {
                Brushes.Red, Brushes.OrangeRed, Brushes.Orange, Brushes.Yellow, Brushes.YellowGreen,
                Brushes.LightGreen, Brushes.Green, Brushes.LightBlue, Brushes.Blue, Brushes.DarkBlue
            };

            for (int i = 1; i <= 10; i++)
            {
                double barHeight = (ratingCounts[i] / (double)maxCount) * (canvasHeight - 60);

                // Draw bar
                var rect = new Rectangle
                {
                    Width = barWidth - 5,
                    Height = barHeight,
                    Fill = colors[i - 1]
                };

                Canvas.SetLeft(rect, 50 + (i - 1) * barWidth);
                Canvas.SetTop(rect, canvasHeight - barHeight - 30);
                RatingChartCanvas.Children.Add(rect);

                // Draw rating label
                var label = new TextBlock
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Canvas.SetLeft(label, 50 + (i - 1) * barWidth + (barWidth - 5) / 2 - 5);
                Canvas.SetTop(label, canvasHeight - 25);
                RatingChartCanvas.Children.Add(label);

                // Draw count label
                if (ratingCounts[i] > 0)
                {
                    var countLabel = new TextBlock
                    {
                        Text = ratingCounts[i].ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 10
                    };
                    Canvas.SetLeft(countLabel, 50 + (i - 1) * barWidth + (barWidth - 5) / 2 - 5);
                    Canvas.SetTop(countLabel, canvasHeight - barHeight - 45);
                    RatingChartCanvas.Children.Add(countLabel);
                }
            }
        }

        private void DisplayAverageRatings()
        {
            AverageRatingPanel.Children.Clear();

            var courseAverages = _filteredFeedbacks
                .GroupBy(f => f.CourseName)
                .Select(g => new
                {
                    CourseName = g.Key,
                    AverageRating = g.Average(f => f.Rating),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .ToList();

            foreach (var course in courseAverages)
            {
                var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                var courseLabel = new TextBlock
                {
                    Text = course.CourseName,
                    Width = 150,
                    FontWeight = FontWeights.Bold
                };

                var ratingLabel = new TextBlock
                {
                    Text = $"{course.AverageRating:F1}/10",
                    Width = 60,
                    Foreground = GetRatingColor(course.AverageRating)
                };

                var countLabel = new TextBlock
                {
                    Text = $"({course.Count} đánh giá)",
                    Foreground = Brushes.Gray
                };

                panel.Children.Add(courseLabel);
                panel.Children.Add(ratingLabel);
                panel.Children.Add(countLabel);

                AverageRatingPanel.Children.Add(panel);
            }
        }

        private Brush GetRatingColor(double rating)
        {
            if (rating >= 8) return Brushes.Green;
            if (rating >= 6) return Brushes.Orange;
            return Brushes.Red;
        }

        private void CourseFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) RefreshDisplay();
        }

        private void MonthFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) RefreshDisplay();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }

    public class FeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime FeedbackDate { get; set; }
    }
}