using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for StudentCourseMaterialPage.xaml
    /// </summary>
    public partial class StudentCourseMaterialPage : Page
    {
        private readonly LifeSkillCourseRepository _courseRepository;
        private readonly ApplicationDbContext _context;
        private readonly int _currentStudentId; // Assume this is set from login session

        public List<EnrollmentViewModel> EnrolledCourses { get; set; }

        public StudentCourseMaterialPage(int studentId)
        {
            InitializeComponent();
            _courseRepository = new LifeSkillCourseRepository();
            _context = ApplicationDbContext.Instance;
            _currentStudentId = studentId;
            LoadEnrolledCourses();
            DataContext = this;
        }

        private void LoadEnrolledCourses()
        {
            var enrollments = _context.Enrollments
                .Where(e => e.StudentId == _currentStudentId)
                .Select(e => new
                {
                    e.EnrollmentId,
                    e.StudentId,
                    e.CourseId,
                    e.CompletionStatus,
                    e.CompletionDate,
                    e.Student,
                    e.Course
                })
                .ToList();

            EnrolledCourses = enrollments.Select(e => new EnrollmentViewModel
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                CompletionStatus = e.CompletionStatus,
                CompletionDate = e.CompletionDate,
                Student = e.Student,
                Course = e.Course,
                Progress = CalculateProgress(e.CourseId, e.CompletionStatus),
                CompletionStatusText = e.CompletionStatus ? "Completed" : "In Progress"
            }).ToList();

            CoursesDataGrid.ItemsSource = EnrolledCourses;
        }

        private double CalculateProgress(int courseId, bool completionStatus)
        {
            if (completionStatus)
                return 100.0;

            var assessments = _context.Assessments
                .Where(a => a.CourseId == courseId)
                .ToList();

            if (!assessments.Any())
                return 0.0;

            var results = _context.AssessmentResults
                .Where(ar => ar.StudentId == _currentStudentId && assessments.Select(a => a.AssessmentId).Contains(ar.AssessmentId))
                .ToList();

            if (!results.Any())
                return 0.0;

            double totalScore = (double)results.Sum(r => r.Score ?? 0); // Explicitly cast decimal to double
            double maxScore = (double)assessments.Sum(a => a.MaxScore); // Explicitly cast decimal to double
            return maxScore > 0 ? (totalScore / maxScore) * 100 : 0.0;
        }
    }

    public class EnrollmentViewModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public bool CompletionStatus { get; set; }
        public DateTime? CompletionDate { get; set; }
        public Student? Student { get; set; }
        public LifeSkillCourse? Course { get; set; }
        public double Progress { get; set; }
        public string CompletionStatusText { get; set; }
    }
}