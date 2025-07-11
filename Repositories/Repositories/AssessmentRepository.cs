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
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly AssessmentDAO _assessmentDAO;
        public AssessmentRepository()
        {
            _assessmentDAO = new AssessmentDAO();
        }
        public void AddAssessment(Assessment assessment) => _assessmentDAO.AddAssessment(assessment);
        public void DeleteAssessment(int id) => _assessmentDAO.DeleteAssessment(id);
        public Assessment GetAssessmentById(int id) => _assessmentDAO.GetAssessmentById(id);
        public List<Assessment> GetAllAssessments() => _assessmentDAO.GetAllAssessments();
        public void UpdateAssessment(Assessment assessment) => _assessmentDAO.UpdateAssessment(assessment);
    }
}
