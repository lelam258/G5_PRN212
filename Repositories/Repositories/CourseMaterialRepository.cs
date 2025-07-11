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
    public class CourseMaterialRepository : ICourseMaterialRepository
    {
        private readonly CourseMaterialDAO _courseMaterialDAO;
        public CourseMaterialRepository()
        {
            _courseMaterialDAO = new CourseMaterialDAO();
        }
        public void AddCourseMaterial(CourseMaterial courseMaterial) => _courseMaterialDAO.AddCourseMaterial(courseMaterial);
        public void DeleteCourseMaterial(int id) => _courseMaterialDAO.DeleteCourseMaterial(id);
        public CourseMaterial GetCourseMaterialById(int id) => _courseMaterialDAO.GetCourseMaterialById(id);
        public List<CourseMaterial> GetAllCourseMaterials() => _courseMaterialDAO.GetAllCourseMaterials();
        public void UpdateCourseMaterial(CourseMaterial courseMaterial) => _courseMaterialDAO.UpdateCourseMaterial(courseMaterial);
    }
}
