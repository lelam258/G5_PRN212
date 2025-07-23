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
    public class AssessmentResultRepository : IAssessmentResultRepository
    {
        private readonly AssessmentResultDAO _assessmentResultDAO;
        public AssessmentResultRepository()
        {
            _assessmentResultDAO = new AssessmentResultDAO();
        }
        public void AddAssessmentResult(AssessmentResult assessmentResult) => _assessmentResultDAO.AddAssessmentResult(assessmentResult);
        public void DeleteAssessmentResult(int id) => _assessmentResultDAO.DeleteAssessmentResult(id);
        public AssessmentResult GetAssessmentResultById(int id) => _assessmentResultDAO.GetAssessmentResultById(id);
        public List<AssessmentResult> GetAllAssessmentResults() => _assessmentResultDAO.GetAllAssessmentResults();
        public void UpdateAssessmentResult(AssessmentResult assessmentResult) => _assessmentResultDAO.UpdateAssessmentResult(assessmentResult);
    }
}
