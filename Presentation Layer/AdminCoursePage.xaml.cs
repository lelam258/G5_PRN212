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
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for AdminCoursePage.xaml
    /// </summary>
    public partial class AdminCoursePage : Page
    {
        private readonly ILifeSkillCourseRepository _lifeSkillCourseRepository;
        private readonly IInstructorRepository _instructorRepository;
        public AdminCoursePage()
        {
            InitializeComponent();
            _lifeSkillCourseRepository = new LifeSkillCourseRepository();
            _instructorRepository = new InstructorRepository();
            LoadCourse();
        }

        public class CourseViewModel
        {
            public int CourseID { get; set; }
            public string CourseName { get; set; }
            public string Instructor { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Description { get; set; }
            public int MaxStudents { get; set; }
            public decimal Price { get; set; }
            public string Status { get; set; }
        }
        public class InstructorView
        {
            public int InstructorID { get; set; }
            public string InstructorName { get; set; }
        }
        private void LoadCourse()
        {
            var courses = _lifeSkillCourseRepository.GetAllLifeSkillCourses()
            .Select(c => new CourseViewModel
            {
                CourseID = c.CourseId,
                CourseName = c.CourseName,
                Instructor = _instructorRepository.GetInstructorById(c.InstructorId)?.InstructorName ?? "Unknown",
                StartDate = (DateTime)c.StartDate,
                EndDate = (DateTime)c.EndDate,
                Description = c.Description,
                MaxStudents = (int)c.MaxStudents,
                Price = (decimal)c.Price,
                Status = c.Status
            }).ToList();
            var instructors = _instructorRepository.GetAllInstructors()
                .Select(i => new InstructorView
                {
                    InstructorID = i.InstructorId,
                    InstructorName = i.InstructorName
                }).ToList();
            InstructorComboBox.ItemsSource = instructors;
            InstructorComboBox.DisplayMemberPath = "InstructorName";

            DataGrid.ItemsSource = courses;
        }
        private void SearchButton(object sender, RoutedEventArgs e)
        {
            string name = SearchNameTextBox.Text.Trim();

            // Fix how we get status value
            string status = "";
            if (SearchStatusComboBox.SelectedItem is ComboBoxItem selectedStatusItem)
            {
                status = selectedStatusItem.Content?.ToString() ?? "";
            }


            DateTime? startDate = SearchStartDatePicker.SelectedDate;
            DateTime? endDate = SearchEndDatePicker.SelectedDate;

            var courses = _lifeSkillCourseRepository.GetAllLifeSkillCourses();

            // Filter by name
            if (!string.IsNullOrEmpty(name))
            {
                courses = courses.Where(c =>
                    c.CourseName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                    _instructorRepository.GetInstructorById(c.InstructorId).InstructorName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status) && !status.Equals("Tất cả", StringComparison.OrdinalIgnoreCase))
            {
                courses = courses.Where(c => string.Equals(c.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by date range
            if (startDate.HasValue)
            {
                courses = courses.Where(c => c.StartDate.Value.Date >= startDate.Value.Date).ToList();
            }
            if (endDate.HasValue)
            {
                courses = courses.Where(c => c.EndDate.Value.Date <= endDate.Value.Date).ToList();
            }

            // Map results to ViewModel
            var filtered = courses
                .Select(c => new CourseViewModel
                {
                    CourseID = c.CourseId,
                    CourseName = c.CourseName,
                    Instructor = _instructorRepository.GetInstructorById(c.InstructorId)?.InstructorName ?? "Unknown",
                    StartDate = (DateTime)c.StartDate,
                    EndDate = (DateTime)c.EndDate,
                    Description = c.Description ?? string.Empty,
                    MaxStudents = (int)c.MaxStudents,
                    Price = (decimal)c.Price,
                    Status = c.Status
                }).ToList();

            DataGrid.ItemsSource = filtered;
            DataGrid.SelectedItem = null;
        }


        private void Instructor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InstructorComboBox.SelectedItem is InstructorView selectedInstructor)
            {
                InstructorTextBox.Text = selectedInstructor.InstructorName;
                InstructorTextBox.Visibility = Visibility.Collapsed;
            }
        }



        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedStatus = selectedItem.Content.ToString();
                StatusComboBox.Text = selectedStatus;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CourseNameTextBox.Text) ||
                InstructorComboBox.SelectedItem == null ||
                StartDatePicker.SelectedDate == null ||
                EndDatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(MaxStudentsTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin khóa học");
                return;
            }
            try
            {
                var newCourse = new Business_Layer.LifeSkillCourse
                {
                    CourseName = CourseNameTextBox.Text,
                    InstructorId = (InstructorComboBox.SelectedItem as InstructorView)?.InstructorID ?? 0,
                    StartDate = StartDatePicker.SelectedDate.Value,
                    EndDate = EndDatePicker.SelectedDate.Value,
                    Description = DescriptionTextBox.Text,
                    MaxStudents = int.Parse(MaxStudentsTextBox.Text),
                    Price = decimal.Parse(PriceTextBox.Text),
                    Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Unknown"
                };
                _lifeSkillCourseRepository.AddLifeSkillCourse(newCourse);
                LoadCourse();
                MessageBox.Show("Thêm khóa học thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khóa học: {ex.Message}");
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is CourseViewModel selectedCourse)
            {
                var course = _lifeSkillCourseRepository.GetLifeSkillCourseById(selectedCourse.CourseID);
                if (course != null)
                {
                    course.CourseName = CourseNameTextBox.Text;
                    course.InstructorId = (InstructorComboBox.SelectedItem as InstructorView)?.InstructorID ?? 0;
                    course.StartDate = StartDatePicker.SelectedDate;
                    course.EndDate = EndDatePicker.SelectedDate;
                    course.Description = DescriptionTextBox.Text;
                    course.MaxStudents = int.Parse(MaxStudentsTextBox.Text);
                    course.Price = decimal.Parse(PriceTextBox.Text);
                    course.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Unknown";
                    _lifeSkillCourseRepository.UpdateLifeSkillCourse(course);
                    LoadCourse();
                    MessageBox.Show("Cập nhật khóa học thành công");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khóa học");
                }
            }
            else
            {
                MessageBox.Show("Chọn 1 khóa học để cập nhật");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is CourseViewModel selectedCourse)
            {
                var courseId = selectedCourse.CourseID;
                _lifeSkillCourseRepository.DeleteLifeSkillCourse(courseId);
                LoadCourse();
                MessageBox.Show("Xóa khóa học thành công");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khóa học");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem is CourseViewModel selectedCourse)
            {
                CourseNameTextBox.Text = selectedCourse.CourseName;
                foreach (var item in InstructorComboBox.Items)
                {
                    if (item is InstructorView instructor &&
                        instructor.InstructorName == selectedCourse.Instructor)
                    {
                        InstructorComboBox.SelectedItem = item;
                        break;
                    }
                }
                StartDatePicker.SelectedDate = selectedCourse.StartDate;
                EndDatePicker.SelectedDate = selectedCourse.EndDate;
                DescriptionTextBox.Text = selectedCourse.Description ?? string.Empty;
                MaxStudentsTextBox.Text = selectedCourse.MaxStudents.ToString();
                PriceTextBox.Text = selectedCourse.Price.ToString("F2");

                // Set status
                foreach (var item in StatusComboBox.Items)
                {
                    if (item is ComboBoxItem statusItem &&
                        statusItem.Content.ToString() == selectedCourse.Status)
                    {
                        StatusComboBox.SelectedItem = statusItem;
                        break;
                    }
                }
                UpdateButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                CourseNameTextBox.Clear();
                InstructorComboBox.SelectedIndex = -1; // Reset instructor selection
                StartDatePicker.SelectedDate = null;
                EndDatePicker.SelectedDate = null;
                DescriptionTextBox.Clear();
                MaxStudentsTextBox.Clear();
                PriceTextBox.Clear();
                StatusComboBox.SelectedIndex = -1; // Reset status selection
                UpdateButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }
    }
}
