using Business_Layer;
using Microsoft.Win32;
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
using System.IO;


namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for AdminCourseMaterialPage.xaml
    /// </summary>
    public partial class AdminCourseMaterialPage : Page
    {
        // repositories
        private readonly LifeSkillCourseRepository _courseRepo = new();
        private readonly CourseMaterialRepository _materialRepo = new();

        // giữ data
        private List<MaterialItem> _allMaterials = new();
        private List<MaterialItem> _filteredMaterials =>
            FilterCourseComboBox.SelectedValue is int cid
            ? _allMaterials.Where(m => m.CourseId == cid).ToList()
            : _allMaterials.ToList();

        // lớp hiển thị
        private class MaterialItem
        {
            public int MaterialId { get; set; }
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public string Title { get; set; }
            public string FilePath { get; set; }
            public string FileName => System.IO.Path.GetFileName(FilePath);
            public DateTime UploadDate { get; set; }
        }

        public AdminCourseMaterialPage()
        {
            InitializeComponent();

            // load khóa
            var courses = _courseRepo.GetAllLifeSkillCourses();
            FilterCourseComboBox.ItemsSource = courses;
            CourseFormComboBox.ItemsSource = courses;

            // load tài liệu
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            // Đổ toàn bộ từ repo
            var list = _materialRepo.GetAllCourseMaterials();
            _allMaterials = list.Select(m => new MaterialItem
            {
                MaterialId = m.MaterialId,
                CourseId = m.CourseId,
                CourseName = _courseRepo.GetLifeSkillCourseById(m.CourseId).CourseName,
                Title = m.Title,
                FilePath = m.FilePath,
                UploadDate = m.UploadDate
            }).ToList();

            MaterialsDataGrid.ItemsSource = _filteredMaterials;
            ResetForm();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
            => MaterialsDataGrid.ItemsSource = _filteredMaterials;

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterCourseComboBox.SelectedIndex = -1;
            MaterialsDataGrid.ItemsSource = _allMaterials;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "PDF, DOCX, PPTX|*.pdf;*.docx;*.pptx|All files|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                FilePathTextBlock.Text = dlg.FileName;
                TitleTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (CourseFormComboBox.SelectedValue is not int cid)
            {
                MessageBox.Show("Chọn khóa học.");
                return;
            }
            var title = TitleTextBox.Text.Trim();
            var path = FilePathTextBlock.Text.Trim();
            if (string.IsNullOrEmpty(title) || !File.Exists(path))
            {
                MessageBox.Show("Nhập tiêu đề và chọn file hợp lệ.");
                return;
            }

            var newMat = new CourseMaterial
            {
                CourseId = cid,
                Title = title,
                FilePath = path,
                UploadDate = DateTime.Now
            };
            _materialRepo.AddCourseMaterial(newMat);
            LoadMaterials();
        }

        private void MaterialsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is MaterialItem sel)
            {
                CourseFormComboBox.SelectedValue = sel.CourseId;
                TitleTextBox.Text = sel.Title;
                FilePathTextBlock.Text = sel.FilePath;

                AddButton.IsEnabled = false;
                UpdateButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else ResetForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is not MaterialItem sel) return;
            if (CourseFormComboBox.SelectedValue is not int cid) return;

            sel.CourseId = cid;
            sel.Title = TitleTextBox.Text.Trim();
            sel.FilePath = FilePathTextBlock.Text.Trim();
            sel.UploadDate = DateTime.Now;

            // Cập nhật lên DB
            var m = new CourseMaterial
            {
                MaterialId = sel.MaterialId,
                CourseId = sel.CourseId,
                Title = sel.Title,
                FilePath = sel.FilePath,
                UploadDate = sel.UploadDate
            };
            _materialRepo.UpdateCourseMaterial(m);
            LoadMaterials();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is not MaterialItem sel) return;
            _materialRepo.DeleteCourseMaterial(sel.MaterialId);
            LoadMaterials();
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
            => ResetForm();

        private void ResetForm()
        {
            CourseFormComboBox.SelectedIndex = -1;
            TitleTextBox.Clear();
            FilePathTextBlock.Text = "(chưa chọn file)";
            AddButton.IsEnabled = true;
            UpdateButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            MaterialsDataGrid.UnselectAll();
        }
    }
}
