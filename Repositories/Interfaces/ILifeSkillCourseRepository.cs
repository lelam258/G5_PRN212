using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface ILifeSkillCourseRepository
    {
        List<LifeSkillCourse> GetAllLifeSkillCourses();
        LifeSkillCourse GetLifeSkillCourseById(int id);
        void AddLifeSkillCourse(LifeSkillCourse lifeSkillCourse);
        void UpdateLifeSkillCourse(LifeSkillCourse lifeSkillCourse);
        void DeleteLifeSkillCourse(int id);
    }
}
