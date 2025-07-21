using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class StudentCourseSchedulePage : Page, INotifyPropertyChanged
    {
        // Danh sách hiển thị lên DataGrid
        private ObservableCollection<ScheduleItem> _scheduleItems = new();
        public ObservableCollection<ScheduleItem> ScheduleItems
        {
            get => _scheduleItems;
            set { _scheduleItems = value; OnPropertyChanged(); }
        }


        // Các dependency
        private readonly int _studentId;
        private readonly EnrollmentDAO _enrollmentDAO = new();
        private readonly CourseScheduleRepository _scheduleRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();

        public StudentCourseSchedulePage(int studentId)
        {
            InitializeComponent();

            // 2) Gán DataContext cho binding hoạt động
            this.DataContext = this;

            _studentId = studentId;
            LoadSchedules();
        }

        private void LoadSchedules()
        {
            // 3) Lấy danh sách enrollments của sinh viên
            var enrolls = _enrollmentDAO.GetEnrollmentsByStudentId(_studentId);
            foreach (var en in enrolls)
            {
                // 4) Lấy thông tin khóa
                var course = _courseRepo.GetLifeSkillCourseById(en.CourseId);

                // 5) Lấy từng lịch trong khóa đó
                var schedules = _scheduleRepo
                    .GetAllCourseSchedules()
                    .Where(s => s.CourseId == en.CourseId);

                foreach (var s in schedules)
                {
                    ScheduleItems.Add(new ScheduleItem
                    {
                        CourseName = course.CourseName,
                        SessionDate = s.SessionDate,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Room = s.Room
                    });
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
