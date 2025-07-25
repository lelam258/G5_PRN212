using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer
{
    public class AssessmentResultDAO
    {
        private readonly ApplicationDbContext _context;
        public AssessmentResultDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<AssessmentResult> GetAllAssessmentResults()
        {
            return _context.AssessmentResults
                          .Include(ar => ar.Student)
                          .ToList();
        }
        public AssessmentResult GetAssessmentResultById(int id)
        {
            return _context.AssessmentResults.FirstOrDefault(ar => ar.ResultId == id);
        }
        public void AddAssessmentResult(AssessmentResult assessmentResult)
        {
            _context.AssessmentResults.Add(assessmentResult);
            _context.SaveChanges();
        }
        public void UpdateAssessmentResult(AssessmentResult assessmentResult)
        {
            _context.AssessmentResults.Update(assessmentResult);
            _context.SaveChanges();
        }
        public void DeleteAssessmentResult(int id)
        {
            var assessmentResult = _context.AssessmentResults.FirstOrDefault(ar => ar.ResultId == id);
            if (assessmentResult != null)
            {
                _context.AssessmentResults.Remove(assessmentResult);
                _context.SaveChanges();
            }
        }
    }
}
