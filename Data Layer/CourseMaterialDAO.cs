using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class CourseMaterialDAO
    {
        private readonly ApplicationDbContext _context;
        public CourseMaterialDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<CourseMaterial> GetAllCourseMaterials()
        {
            return _context.CourseMaterials.ToList();
        }
        public CourseMaterial GetCourseMaterialById(int id)
        {
            return _context.CourseMaterials.FirstOrDefault(cm => cm.MaterialId == id);
        }
        public void AddCourseMaterial(CourseMaterial courseMaterial)
        {
            _context.CourseMaterials.Add(courseMaterial);
            _context.SaveChanges();
        }
        public void UpdateCourseMaterial(CourseMaterial courseMaterial)
        {
            _context.CourseMaterials.Update(courseMaterial);
            _context.SaveChanges();
        }
        public void DeleteCourseMaterial(int id)
        {
            var courseMaterial = _context.CourseMaterials.FirstOrDefault(cm => cm.MaterialId == id);
            if (courseMaterial != null)
            {
                _context.CourseMaterials.Remove(courseMaterial);
                _context.SaveChanges();
            }
        }
    }
}
