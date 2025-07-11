using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface ICourseScheduleRepository
    {
        List<CourseSchedule> GetAllCourseSchedules();
        CourseSchedule GetCourseScheduleById(int id);
        void AddCourseSchedule(CourseSchedule courseSchedule);
        void UpdateCourseSchedule(CourseSchedule courseSchedule);
        void DeleteCourseSchedule(int id);
    }
}
