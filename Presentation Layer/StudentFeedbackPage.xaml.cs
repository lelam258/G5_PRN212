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
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for StudentFeedbackPage.xaml
    /// </summary>
    public partial class StudentFeedbackPage : Page
    {
        private readonly IFeedbackRepository _feedback;
        private readonly IEnrollmentRepository _enrollment;
        private readonly ILifeSkillCourseRepository _lifeSkillCourseRepository;
        private readonly IInstructorRepository _instructorRepository;
        private int _studentId;
        public StudentFeedbackPage(int studentId)
        {
            InitializeComponent();
            _feedback = new FeedbackRepository();
            _enrollment = new EnrollmentRepository();
            _lifeSkillCourseRepository = new LifeSkillCourseRepository();
            _instructorRepository = new InstructorRepository();
            _studentId = studentId;
            LoadData();
        }
        public class CourseView
        {
            public int CourseId { get; set; }
            public string? CourseName { get; set; }
            public string? Instructor { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string? Description { get; set; }
        }

        private void LoadData()
        {
            var enrolledCourseIds = _enrollment.GetAllEnrollments()
                                           .Where(e => e.StudentId == _studentId)
                                           .Select(e => e.CourseId)
                                           .Distinct()
                                           .ToList();
            var details = _lifeSkillCourseRepository.GetAllLifeSkillCourses()
                .Where(course => enrolledCourseIds.Contains(course.CourseId))
                .Select(course => new CourseView
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    Instructor = _instructorRepository.GetInstructorById(course.InstructorId)?.InstructorName ?? "Unknown",
                    StartDate = (DateTime)course.StartDate,
                    EndDate = (DateTime)course.EndDate,
                    Description = course.Description
                })
                .ToList();
            DataGrid.ItemsSource = details;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem is CourseView course)
            {
                CourseTextBox.Text = course.CourseName;
                RatingComboBox.SelectedIndex = -1;
                AddButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
            }
            else
            {
                CourseTextBox.Text = string.Empty;
                RatingComboBox.SelectedIndex = -1;
                FeedbackTextBox.Text = string.Empty;
                AddButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CourseTextBox.Text) || RatingComboBox.SelectedIndex == -1 || string.IsNullOrEmpty(FeedbackTextBox.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin phản hồi!");
                return;
            }

            var selectedCourse = DataGrid.SelectedItem as CourseView;
            if (selectedCourse == null)
                return;

            var selectedRatingItem = RatingComboBox.SelectedItem as ComboBoxItem;
            if (selectedRatingItem == null || !int.TryParse(selectedRatingItem.Content.ToString(), out int rating))
            {
                MessageBox.Show("Mức đánh giá không hợp lệ!");
                return;
            }

            var feedback = new Feedback
            {
                StudentId = _studentId,
                CourseId = selectedCourse.CourseId,
                Rating = rating,
                Comment = FeedbackTextBox.Text,
                FeedbackDate = DateTime.Now
            };

            try
            {
                _feedback.AddFeedback(feedback);
                MessageBox.Show("Phản hồi đã được thêm thành công!");

                CourseTextBox.Text = string.Empty;
                RatingComboBox.SelectedIndex = -1;
                FeedbackTextBox.Text = string.Empty;
                AddButton.IsEnabled = false;
                CancelButton.IsEnabled = false;

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi thêm phản hồi: {ex.Message}");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CourseTextBox.Text = string.Empty;
            RatingComboBox.SelectedIndex = -1;
            FeedbackTextBox.Text = string.Empty;
            AddButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            LoadData();
        }
    }
}
