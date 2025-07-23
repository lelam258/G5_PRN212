using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface IAssessmentResultRepository
    {
        List<AssessmentResult> GetAllAssessmentResults();
        AssessmentResult GetAssessmentResultById(int id);
        void AddAssessmentResult(AssessmentResult assessmentResult);
        void UpdateAssessmentResult(AssessmentResult assessmentResult);
        void DeleteAssessmentResult(int id);
    }
}
