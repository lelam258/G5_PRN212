using Microsoft.Win32;
using Presentation_Layer;      // for NullToBoolConverter
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Business_Layer;
using System.Diagnostics;

namespace Presentation_Layer
{
    public partial class AdminAssessmentPage : Page
    {
        private readonly LifeSkillCourseRepository _courseRepo = new();
        private readonly AssessmentRepository _assessRepo = new();

        // nội bộ DTO để bind lên DataGrid
        private class AssessmentItem
        {
            public int AssessmentId { get; set; }
            public int CourseId { get; set; }
            public string CourseName { get; set; } = "";
            public string AssessmentName { get; set; } = "";
            public string? AssessmentType { get; set; }
            public string? Instructions { get; set; }
            public int MaxScore { get; set; }
            public DateTime DueDate { get; set; }
            public string? FilePath { get; set; }
        }

        private List<AssessmentItem> _allItems = new();

        public AdminAssessmentPage()
        {
            InitializeComponent();

            // Load courses vào hai ComboBox
            var courses = _courseRepo.GetAllLifeSkillCourses();
            FilterCourseComboBox.ItemsSource = courses;
            CourseFormComboBox.ItemsSource = courses;

            LoadAssessments();
        }

        private void LoadAssessments()
        {
            // Lấy tất cả từ repo và map sang DTO
            _allItems = _assessRepo.GetAllAssessments()
                        .Select(a => new AssessmentItem
                        {
                            AssessmentId = a.AssessmentId,
                            CourseId = a.CourseId,
                            CourseName = _courseRepo.GetLifeSkillCourseById(a.CourseId).CourseName,
                            AssessmentName = a.AssessmentName,
                            AssessmentType = a.AssessmentType,
                            Instructions = a.Instructions,
                            MaxScore = a.MaxScore,
                            DueDate = a.DueDate,
                            FilePath = a.FilePath
                        })
                        .ToList();

            AssessmentsDataGrid.ItemsSource = _allItems;
            ResetForm();
        }

        private IEnumerable<AssessmentItem> FilteredItems =>
            FilterCourseComboBox.SelectedValue is int cid
                ? _allItems.Where(x => x.CourseId == cid)
                : _allItems;

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            AssessmentsDataGrid.ItemsSource = FilteredItems;
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterCourseComboBox.SelectedIndex = -1;
            AssessmentsDataGrid.ItemsSource = _allItems;
        }

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
                FilePathTextBox.Text = sel.FilePath ?? "";

                AddButton.IsEnabled = false;
                UpdateButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                ResetForm();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
            => ResetForm();

        private void ResetForm()
        {
            CourseFormComboBox.SelectedIndex = -1;
            NameTextBox.Clear();
            TypeTextBox.Clear();
            InstructionsTextBox.Clear();
            MaxScoreTextBox.Clear();
            DueDatePicker.SelectedDate = null;
            FilePathTextBox.Clear();

            AddButton.IsEnabled = true;
            UpdateButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            AssessmentsDataGrid.UnselectAll();
        }

        // Browse file - chỉ cho phép ZIP files
        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select Assessment ZIP File",
                Filter = "ZIP files (*.zip)|*.zip",
                CheckFileExists = true,
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                string selectedFile = dlg.FileName;
                FileInfo fileInfo = new FileInfo(selectedFile);

                // Check file size (30MB = 30 * 1024 * 1024 bytes) - giống như StudentAssessmentPage
                long maxSize = 30 * 1024 * 1024;
                if (fileInfo.Length > maxSize)
                {
                    MessageBox.Show("File size exceeds 30MB limit. Please select a smaller file.",
                                  "File Size Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                FilePathTextBox.Text = selectedFile;
            }
        }

        // Validate input và map ra model
        private bool ValidateInput(out Assessment model)
        {
            model = null!;

            if (CourseFormComboBox.SelectedValue is not int cid)
            {
                MessageBox.Show("Vui long chon khoa hoc.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            var name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Ten bai kiem tra khong duoc de trong.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            var type = TypeTextBox.Text.Trim();
            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Loai bai kiem tra khong duoc de trong.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            var instr = InstructionsTextBox.Text.Trim();
            if (string.IsNullOrEmpty(instr))
            {
                MessageBox.Show("Huong dan khong duoc de trong.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(MaxScoreTextBox.Text.Trim(), out int max) || max <= 0)
            {
                MessageBox.Show("Diem toi da phai la so nguyen duong.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (DueDatePicker.SelectedDate is not DateTime due || due < DateTime.Today)
            {
                MessageBox.Show("Han nop phai la hom nay hoac tuong lai.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate file path nếu có - chỉ cho phép ZIP
            if (!string.IsNullOrWhiteSpace(FilePathTextBox.Text))
            {
                var filePath = FilePathTextBox.Text.Trim();
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File khong ton tai.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!Path.GetExtension(filePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Chi cho phep file ZIP.", "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            model = new Assessment
            {
                CourseId = cid,
                AssessmentName = name,
                AssessmentType = type,
                Instructions = instr,
                MaxScore = max,
                DueDate = due,
                FilePath = string.IsNullOrWhiteSpace(FilePathTextBox.Text)
                                 ? null
                                 : FilePathTextBox.Text.Trim()
            };
            return true;
        }

        // Copy file đã chọn vào thư mục dự án và trả đường dẫn mới
        private string CopyFileToStorage(string sourcePath, int assessmentId)
        {
            var destFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssessmentFiles");
            Directory.CreateDirectory(destFolder);

            var originalName = Path.GetFileName(sourcePath);
            var destName = $"{assessmentId}_{originalName}";
            var destPath = Path.Combine(destFolder, destName);

            File.Copy(sourcePath, destPath, overwrite: true);
            return destPath;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out var model)) return;

            // 1) Thêm assessment => có ID
            _assessRepo.AddAssessment(model);

            // 2) Copy file nếu có và cập nhật lại
            if (!string.IsNullOrWhiteSpace(model.FilePath))
            {
                model.FilePath = CopyFileToStorage(model.FilePath, model.AssessmentId);
                _assessRepo.UpdateAssessment(model);
            }

            LoadAssessments();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssessmentsDataGrid.SelectedItem is not AssessmentItem sel) return;
            if (!ValidateInput(out var model)) return;

            model.AssessmentId = sel.AssessmentId;
            _assessRepo.UpdateAssessment(model);

            // nếu chọn file mới
            if (!string.IsNullOrWhiteSpace(model.FilePath) &&
                model.FilePath != sel.FilePath)
            {
                model.FilePath = CopyFileToStorage(model.FilePath, model.AssessmentId);
                _assessRepo.UpdateAssessment(model);
            }

            LoadAssessments();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssessmentsDataGrid.SelectedItem is not AssessmentItem sel) return;

            var result = MessageBox.Show(
                $"Chac chan xoa \"{sel.AssessmentName}\"?",
                "Xac nhan", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _assessRepo.DeleteAssessment(sel.AssessmentId);
                LoadAssessments();
            }
        }

        private void DownloadFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is string path &&
                File.Exists(path))
            {
                // Kiểm tra xem có phải file ZIP không trước khi mở
                if (Path.GetExtension(path).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("Chi co the mo file ZIP.", "Loi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Khong tim thay file.", "Loi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    
}