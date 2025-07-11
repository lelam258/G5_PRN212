using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class AssessmentDAO
    {
        private readonly ApplicationDbContext _context;
        public AssessmentDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<Assessment> GetAllAssessments()
        {
            return _context.Assessments.ToList();
        }
        public Assessment GetAssessmentById(int id)
        {
            return _context.Assessments.FirstOrDefault(a => a.AssessmentId == id);
        }
        public void AddAssessment(Assessment assessment)
        {
            _context.Assessments.Add(assessment);
            _context.SaveChanges();
        }
        public void UpdateAssessment(Assessment assessment)
        {
            _context.Assessments.Update(assessment);
            _context.SaveChanges();
        }
        public void DeleteAssessment(int id)
        {
            var assessment = _context.Assessments.FirstOrDefault(a => a.AssessmentId == id);
            if (assessment != null)
            {
                _context.Assessments.Remove(assessment);
                _context.SaveChanges();
            }
        }
    }
}
