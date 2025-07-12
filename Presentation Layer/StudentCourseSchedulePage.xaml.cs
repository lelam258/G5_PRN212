using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class StudentCourseSchedulePage : Page
    {
        public ObservableCollection<ScheduleDisplayItem> ScheduleItems { get; set; } = new();

        private readonly int _studentId;
        private readonly EnrollmentDAO _enrollmentDAO = new();
        private readonly CourseScheduleRepository _courseScheduleRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();

        public StudentCourseSchedulePage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            DataContext = this;
            LoadSchedule();
        }

        private void LoadSchedule()
        {
            var enrollments = _enrollmentDAO.GetAllEnrollments()
                                .Where(e => e.StudentId == _studentId)
                                .ToList();

            foreach (var enrollment in enrollments)
            {
                var course = _courseRepo.GetLifeSkillCourseById(enrollment.CourseId);
                if (course == null) continue;

                var schedules = _courseScheduleRepo.GetAllCourseSchedules()
                                   .Where(s => s.CourseId == course.CourseId)
                                   .ToList();

                foreach (var schedule in schedules)
                {
                    ScheduleItems.Add(new ScheduleDisplayItem
                    {
                        CourseName = course.CourseName,
                        SessionDate = schedule.SessionDate.ToString("dd/MM/yyyy"),
                        StartTime = schedule.StartTime.ToString(@"hh\:mm"),
                        EndTime = schedule.EndTime.ToString(@"hh\:mm"),
                        Room = schedule.Room
                    });
                }
            }
        }

        public class ScheduleDisplayItem
        {
            public string CourseName { get; set; }
            public string SessionDate { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Room { get; set; }
        }
    }
}
