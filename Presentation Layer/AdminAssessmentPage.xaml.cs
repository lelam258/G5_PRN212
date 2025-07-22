using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for AdminAssessmentPage.xaml
    /// </summary>
    public partial class AdminAssessmentPage : Page
    {
        private readonly LifeSkillCourseRepository _courseRepo = new();
        private readonly AssessmentRepository _assessRepo = new();

        private List<AssessmentItem> _allItems = new();

        // DTO để bind
        private class AssessmentItem
        {
            public int AssessmentId { get; set; }
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public string AssessmentName { get; set; }
            public string AssessmentType { get; set; }
            public string Instructions { get; set; }
            public int MaxScore { get; set; }
            public DateTime DueDate { get; set; }
        }

        public AdminAssessmentPage()
        {
            InitializeComponent();

            // Load courses lên cả 2 ComboBox
            var courses = _courseRepo.GetAllLifeSkillCourses();
            FilterCourseComboBox.ItemsSource = courses;
            CourseFormComboBox.ItemsSource = courses;

            // Load toàn bộ assessments
            LoadAssessments();
        }

        private void LoadAssessments()
        {
            // Lấy từ repo
            var models = _assessRepo.GetAllAssessments();
            _allItems = models.Select(a => new AssessmentItem
            {
                AssessmentId = a.AssessmentId,
                CourseId = a.CourseId,
                CourseName = _courseRepo.GetLifeSkillCourseById(a.CourseId).CourseName,
                AssessmentName = a.AssessmentName,
                AssessmentType = a.AssessmentType,
                Instructions = a.Instructions,
                MaxScore = a.MaxScore,
                DueDate = a.DueDate
            }).ToList();

            // Hiển thị tất cả
            AssessmentsDataGrid.ItemsSource = _allItems;
            ResetForm();
        }

        private IEnumerable<AssessmentItem> FilteredItems =>
            FilterCourseComboBox.SelectedValue is int cid
                ? _allItems.Where(x => x.CourseId == cid)
                : _allItems;

        // Lọc
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            AssessmentsDataGrid.ItemsSource = FilteredItems;
        }

        // Reset filter
        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterCourseComboBox.SelectedIndex = -1;
            AssessmentsDataGrid.ItemsSource = _allItems;
        }

        // Khi chọn dòng DataGrid
        private void AssessmentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssessmentsDataGrid.SelectedItem is AssessmentItem sel)
            {
                CourseFormComboBox.SelectedValue = sel.CourseId;
                NameTextBox.Text = sel.AssessmentName;
                TypeTextBox.Text = sel.AssessmentType;
                InstructionsTextBox.Text = sel.Instructions;
                MaxScoreTextBox.Text = sel.MaxScore.ToString();
                DueDatePicker.SelectedDate = sel.DueDate;

                AddButton.IsEnabled = false;
                UpdateButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                ResetForm();
            }
        }

        // Clear form
        private void ClearButton_Click(object sender, RoutedEventArgs e) => ResetForm();

        private void ResetForm()
        {
            CourseFormComboBox.SelectedIndex = -1;
            NameTextBox.Clear();
            TypeTextBox.Clear();
            InstructionsTextBox.Clear();
            MaxScoreTextBox.Clear();
            DueDatePicker.SelectedDate = null;

            AddButton.IsEnabled = true;
            UpdateButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            AssessmentsDataGrid.UnselectAll();
        }

        // Thêm mới
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out var model))
            {
                _assessRepo.AddAssessment(model);
                LoadAssessments();
            }
        }

        // Cập nhật
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssessmentsDataGrid.SelectedItem is AssessmentItem sel
                && ValidateInput(out var model))
            {
                model.AssessmentId = sel.AssessmentId;
                _assessRepo.UpdateAssessment(model);
                LoadAssessments();
            }
        }

        // Xóa
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssessmentsDataGrid.SelectedItem is AssessmentItem sel)
            {
                var ok = MessageBox.Show(
                    $"Chắc chắn xóa bài kiểm tra “{sel.AssessmentName}”?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ok == MessageBoxResult.Yes)
                {
                    _assessRepo.DeleteAssessment(sel.AssessmentId);
                    LoadAssessments();
                }
            }
        }

        // Validate input và map về model Business_Layer.Assessment
        private bool ValidateInput(out Business_Layer.Assessment m)
        {
            m = null!;

            // Khóa
            if (CourseFormComboBox.SelectedValue is not int cid)
            {
                MessageBox.Show("Vui lòng chọn khóa học.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Tên bài kiểm tra không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var type = TypeTextBox.Text.Trim();
            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Loại bài kiểm tra không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var instr = InstructionsTextBox.Text.Trim();
            if (string.IsNullOrEmpty(instr))
            {
                MessageBox.Show("Hướng dẫn không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(MaxScoreTextBox.Text.Trim(), out int max) || max <= 0)
            {
                MessageBox.Show("Điểm tối đa phải là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DueDatePicker.SelectedDate is not DateTime due || due < DateTime.Today)
            {
                MessageBox.Show("Hạn nộp phải là ngày hôm nay hoặc tương lai.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Tạo model để Add/Update
            m = new Business_Layer.Assessment
            {
                CourseId = cid,
                AssessmentName = name,
                AssessmentType = type,
                Instructions = instr,
                MaxScore = max,
                DueDate = due
            };
            return true;
        }
    }
}
