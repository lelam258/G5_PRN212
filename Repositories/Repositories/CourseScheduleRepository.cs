using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;
using Data_Layer;
using Repositories.Interfaces;

namespace Repositories.Repositories
{
    public class CourseScheduleRepository : ICourseScheduleRepository
    {
        private readonly CourseScheduleDAO _courseScheduleDAO;
        public CourseScheduleRepository()
        {
            _courseScheduleDAO = new CourseScheduleDAO();
        }
        public void AddCourseSchedule(CourseSchedule courseSchedule) => _courseScheduleDAO.AddCourseSchedule(courseSchedule);
        public void DeleteCourseSchedule(int id) => _courseScheduleDAO.DeleteCourseSchedule(id);
        public CourseSchedule GetCourseScheduleById(int id) => _courseScheduleDAO.GetCourseScheduleById(id);
        public List<CourseSchedule> GetAllCourseSchedules() => _courseScheduleDAO.GetAllCourseSchedules();
        public void UpdateCourseSchedule(CourseSchedule courseSchedule) => _courseScheduleDAO.UpdateCourseSchedule(courseSchedule);
    }
}
