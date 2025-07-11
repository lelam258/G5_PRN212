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
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly FeedbackDAO _feedbackDAO;
        public FeedbackRepository()
        {
            _feedbackDAO = new FeedbackDAO();
        }
        public void AddFeedback(Feedback feedback) => _feedbackDAO.AddFeedback(feedback);
        public void DeleteFeedback(int id) => _feedbackDAO.DeleteFeedback(id);
        public Feedback GetFeedbackById(int id) => _feedbackDAO.GetFeedbackById(id);
        public List<Feedback> GetAllFeedbacks() => _feedbackDAO.GetAllFeedbacks();
        public void UpdateFeedback(Feedback feedback) => _feedbackDAO.UpdateFeedback(feedback);
    }
}
