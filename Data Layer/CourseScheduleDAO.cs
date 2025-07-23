using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class CourseScheduleDAO
    {
        private readonly ApplicationDbContext _context;
        public CourseScheduleDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<CourseSchedule> GetAllCourseSchedules()
        {
            return _context.CourseSchedules.ToList();
        }
        public CourseSchedule GetCourseScheduleById(int id)
        {
            return _context.CourseSchedules.FirstOrDefault(cs => cs.ScheduleId == id);
        }
        public void AddCourseSchedule(CourseSchedule courseSchedule)
        {
            _context.CourseSchedules.Add(courseSchedule);
            _context.SaveChanges();
        }
        public void UpdateCourseSchedule(CourseSchedule courseSchedule)
        {
            _context.CourseSchedules.Update(courseSchedule);
            _context.SaveChanges();
        }
        public void DeleteCourseSchedule(int id)
        {
            var courseSchedule = _context.CourseSchedules.FirstOrDefault(cs => cs.ScheduleId == id);
            if (courseSchedule != null)
            {
                _context.CourseSchedules.Remove(courseSchedule);
                _context.SaveChanges();
            }
        }
    }
}
