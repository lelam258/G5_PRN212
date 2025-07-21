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
        // Danh sách hiển thị lên DataGrid


        // Các dependency
        private readonly int _studentId;
        private readonly EnrollmentDAO _enrollmentDAO = new();
        private readonly CourseScheduleRepository _scheduleRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();

        public StudentCourseSchedulePage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
        }
    }
}