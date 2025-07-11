using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface IAssessmentRepository
    {
        List<Assessment> GetAllAssessments();
        Assessment GetAssessmentById(int id);
        void AddAssessment(Assessment assessment);
        void UpdateAssessment(Assessment assessment);
        void DeleteAssessment(int id);
    }
}
