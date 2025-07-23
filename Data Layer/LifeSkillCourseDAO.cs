using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer
{
    public class LifeSkillCourseDAO
    {
        private readonly ApplicationDbContext _context;
        public LifeSkillCourseDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<LifeSkillCourse> GetAllLifeSkillCourses()
        {
            return _context.LifeSkillCourses.Include(c => c.Instructor).ToList();
        }
        public LifeSkillCourse GetLifeSkillCourseById(int id)
        {
            return _context.LifeSkillCourses.FirstOrDefault(c => c.CourseId == id);
        }
        public void AddLifeSkillCourse(LifeSkillCourse course)
        {
            _context.LifeSkillCourses.Add(course);
            _context.SaveChanges();
        }
        public void UpdateLifeSkillCourse(LifeSkillCourse course)
        {
            _context.LifeSkillCourses.Update(course);
            _context.SaveChanges();
        }
        public void DeleteLifeSkillCourse(int id)
        {
            var course = _context.LifeSkillCourses.FirstOrDefault(c => c.CourseId == id);
            if (course != null)
            {
                _context.LifeSkillCourses.Remove(course);
                _context.SaveChanges();
            }
        }
    }
}
