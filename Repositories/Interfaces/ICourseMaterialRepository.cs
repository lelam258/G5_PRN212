using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface ICourseMaterialRepository
    {
        List<CourseMaterial> GetAllCourseMaterials();
        CourseMaterial GetCourseMaterialById(int id);
        void AddCourseMaterial(CourseMaterial courseMaterial);
        void UpdateCourseMaterial(CourseMaterial courseMaterial);
        void DeleteCourseMaterial(int id);
    }
}
