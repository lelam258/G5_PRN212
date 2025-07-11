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
using Business_Layer;
using Microsoft.Win32;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for StudentCourseSchedulePage.xaml
    /// </summary>
    public partial class StudentCourseSchedulePage : Page
    {
        private readonly CourseMaterialRepository _materialRepository;
        private readonly ApplicationDbContext _context;
        private readonly int _currentStudentId; // Assume this is set from login session

        public List<CourseMaterialViewModel> CourseMaterials { get; set; } = new List<CourseMaterialViewModel>();

        public StudentCourseSchedulePage(int studentId)
        {
            InitializeComponent();
            _materialRepository = new CourseMaterialRepository();
            _context = ApplicationDbContext.Instance;
            _currentStudentId = studentId;
            LoadCourseMaterials();
            DataContext = this;
        }

        private void LoadCourseMaterials()
        {
            CourseMaterials = _context.CourseMaterials
                .Where(cm => cm.LifeSkillCourse.Enrollments.Any(e => e.StudentId == _currentStudentId))
                .Select(cm => new CourseMaterialViewModel
                {
                    MaterialId = cm.MaterialId,
                    CourseId = cm.CourseId,
                    Title = cm.Title,
                    FilePath = cm.FilePath,
                    UploadDate = cm.UploadDate,
                    LifeSkillCourse = cm.LifeSkillCourse
                }).ToList();
            MaterialsDataGrid.ItemsSource = CourseMaterials;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int materialId)
            {
                var material = _context.CourseMaterials.FirstOrDefault(cm => cm.MaterialId == materialId);
                if (material != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = material.Title,
                        Filter = "All files (*.*)|*.*",
                        Title = "Save Material File"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // Note: This is a placeholder. Actual file download would require file system access or server logic.
                        // For demonstration, we'll simulate saving a file path.
                        MessageBox.Show($"File saved to: {saveFileDialog.FileName}", "Download Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
    }

    public class CourseMaterialViewModel
    {
        public int MaterialId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public LifeSkillCourse LifeSkillCourse { get; set; }
    }
}
