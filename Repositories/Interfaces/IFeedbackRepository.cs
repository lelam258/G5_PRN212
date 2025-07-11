using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        List<Feedback> GetAllFeedbacks();
        Feedback GetFeedbackById(int id);
        void AddFeedback(Feedback feedback);
        void UpdateFeedback(Feedback feedback);
        void DeleteFeedback(int id);
    }
}
