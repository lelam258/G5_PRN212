using Business_Layer;
using Data_Layer;
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
    public partial class AdminCourseSchedulePage : Page
    {
        private readonly CourseScheduleRepository _scheduleRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();
        private readonly EnrollmentDAO _enrollmentDAO = new();
        private readonly NotificationDAO _notificationDAO = new();

        public AdminCourseSchedulePage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var courses = _courseRepo.GetAllLifeSkillCourses();
            FilterCourseComboBox.ItemsSource = courses;
            CourseFormComboBox.ItemsSource = courses;

            // Populate hour/minute combo boxes
            for (int h = 0; h < 24; h++)
                StartHourComboBox.Items.Add(h.ToString("D2"));
            for (int m = 0; m < 60; m += 5)
                StartMinuteComboBox.Items.Add(m.ToString("D2"));
            for (int h = 0; h < 24; h++)
                EndHourComboBox.Items.Add(h.ToString("D2"));
            for (int m = 0; m < 60; m += 5)
                EndMinuteComboBox.Items.Add(m.ToString("D2"));

            FilterSchedules();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
            => FilterSchedules();

        private void FilterSchedules()
        {
            var list = _scheduleRepo.GetAllCourseSchedules().AsQueryable();
            if (FilterCourseComboBox.SelectedItem is LifeSkillCourse c)
                list = list.Where(s => s.CourseId == c.CourseId);
            if (FilterStartDatePicker.SelectedDate is DateTime sd)
                list = list.Where(s => s.SessionDate.Date >= sd.Date);
            if (FilterEndDatePicker.SelectedDate is DateTime ed)
                list = list.Where(s => s.SessionDate.Date <= ed.Date);

            ScheduleDataGrid.ItemsSource = list.ToList();
        }

        private bool ValidateInput(out int sh, out int sm, out int eh, out int em)
        {
            sh = sm = eh = em = 0;
            // Date not in past
            if (SessionDatePicker.SelectedDate is not DateTime sessionDate)
            {
                MessageBox.Show("Vui lòng chọn ngày học."); return false;
            }
            if (sessionDate.Date < DateTime.Today)
            {
                MessageBox.Show("Ngày học phải là hôm nay hoặc tương lai."); return false;
            }
            // Hours/minutes selected
            if (StartHourComboBox.SelectedItem is null || StartMinuteComboBox.SelectedItem is null ||
                EndHourComboBox.SelectedItem is null || EndMinuteComboBox.SelectedItem is null)
            {
                MessageBox.Show("Vui lòng chọn giờ bắt đầu và giờ kết thúc."); return false;
            }
            sh = int.Parse(StartHourComboBox.SelectedItem.ToString());
            sm = int.Parse(StartMinuteComboBox.SelectedItem.ToString());
            eh = int.Parse(EndHourComboBox.SelectedItem.ToString());
            em = int.Parse(EndMinuteComboBox.SelectedItem.ToString());
            // Range check
            if (sh < 0 || sh > 23 || eh < 0 || eh > 23 || sm < 0 || sm > 59 || em < 0 || em > 59)
            {
                MessageBox.Show("Giờ hoặc phút không hợp lệ."); return false;
            }
            // Start < End
            var tsStart = new TimeSpan(sh, sm, 0);
            var tsEnd = new TimeSpan(eh, em, 0);
            if (tsEnd <= tsStart)
            {
                MessageBox.Show("Giờ kết thúc phải sau giờ bắt đầu."); return false;
            }
            return true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out int sh, out int sm, out int eh, out int em)) return;
            var course = CourseFormComboBox.SelectedItem as LifeSkillCourse;
            if (course is null) { MessageBox.Show("Vui lòng chọn khóa."); return; }
            var schedule = new CourseSchedule
            {
                CourseId = course.CourseId,
                SessionDate = SessionDatePicker.SelectedDate.Value,
                StartTime = new TimeSpan(sh, sm, 0),
                EndTime = new TimeSpan(eh, em, 0),
                Room = RoomTextBox.Text
            };
            _scheduleRepo.AddCourseSchedule(schedule);
            NotifyStudents(schedule.CourseId, $"Buổi học mới ngày {schedule.SessionDate:dd/MM/yyyy}");
            FilterSchedules(); ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleDataGrid.SelectedItem is not CourseSchedule sel) return;
            if (!ValidateInput(out int sh, out int sm, out int eh, out int em)) return;
            sel.CourseId = ((LifeSkillCourse)CourseFormComboBox.SelectedItem).CourseId;
            sel.SessionDate = SessionDatePicker.SelectedDate.Value;
            sel.StartTime = new TimeSpan(sh, sm, 0);
            sel.EndTime = new TimeSpan(eh, em, 0);
            sel.Room = RoomTextBox.Text;
            _scheduleRepo.UpdateCourseSchedule(sel);
            NotifyStudents(sel.CourseId, $"Buổi học đã được cập nhật ngày {sel.SessionDate:dd/MM/yyyy}");
            FilterSchedules();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleDataGrid.SelectedItem is not CourseSchedule sel) return;
            _scheduleRepo.DeleteCourseSchedule(sel.ScheduleId);
            NotifyStudents(sel.CourseId, $"Buổi học ngày {sel.SessionDate:dd/MM/yyyy} đã bị xóa");
            FilterSchedules(); ClearForm();
        }

        private void ScheduleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScheduleDataGrid.SelectedItem is CourseSchedule sel)
            {
                CourseFormComboBox.SelectedItem = _courseRepo.GetAllLifeSkillCourses().First(c => c.CourseId == sel.CourseId);
                SessionDatePicker.SelectedDate = sel.SessionDate;
                StartHourComboBox.SelectedItem = sel.StartTime.Hours.ToString("D2");
                StartMinuteComboBox.SelectedItem = sel.StartTime.Minutes.ToString("D2");
                EndHourComboBox.SelectedItem = sel.EndTime.Hours.ToString("D2");
                EndMinuteComboBox.SelectedItem = sel.EndTime.Minutes.ToString("D2");
                RoomTextBox.Text = sel.Room;
                UpdateButton.IsEnabled = DeleteButton.IsEnabled = true;
            }
            else ClearForm();
        }

        private void ClearForm()
        {
            CourseFormComboBox.SelectedItem = null;
            SessionDatePicker.SelectedDate = null;
            StartHourComboBox.SelectedItem = null;
            StartMinuteComboBox.SelectedItem = null;
            EndHourComboBox.SelectedItem = null;
            EndMinuteComboBox.SelectedItem = null;
            RoomTextBox.Clear();
            UpdateButton.IsEnabled = DeleteButton.IsEnabled = false;
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            FilterCourseComboBox.SelectedItem = null;
            FilterStartDatePicker.SelectedDate = null;
            FilterEndDatePicker.SelectedDate = null;
            FilterSchedules();
        }
        private void NotifyStudents(int courseId, string message)
        {
            var enrolls = _enrollmentDAO.GetAllEnrollments().Where(e => e.CourseId == courseId);
            foreach (var en in enrolls)
            {
                _notificationDAO.AddNotification(new Notification
                {
                    Title = "Thông báo lịch học",
                    Content = message,
                    StudentId = en.StudentId,
                    CreatedDate = DateTime.Now
                });
            }
        }
    }
}
