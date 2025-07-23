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
    public class LifeSkillCourseRepository : ILifeSkillCourseRepository
    {
        private readonly LifeSkillCourseDAO _lifeSkillCourseDAO;
        public LifeSkillCourseRepository()
        {
            _lifeSkillCourseDAO = new LifeSkillCourseDAO();
        }
        public void AddLifeSkillCourse(LifeSkillCourse lifeSkillCourse) => _lifeSkillCourseDAO.AddLifeSkillCourse(lifeSkillCourse);
        public void DeleteLifeSkillCourse(int id) => _lifeSkillCourseDAO.DeleteLifeSkillCourse(id);
        public LifeSkillCourse GetLifeSkillCourseById(int id) => _lifeSkillCourseDAO.GetLifeSkillCourseById(id);
        public List<LifeSkillCourse> GetAllLifeSkillCourses() => _lifeSkillCourseDAO.GetAllLifeSkillCourses();
        public void UpdateLifeSkillCourse(LifeSkillCourse lifeSkillCourse) => _lifeSkillCourseDAO.UpdateLifeSkillCourse(lifeSkillCourse);
    }
}
