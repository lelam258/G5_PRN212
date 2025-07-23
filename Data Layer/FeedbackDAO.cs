using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer
{
    public class FeedbackDAO
    {
        private readonly ApplicationDbContext _context;

        public FeedbackDAO()
        {
            _context = ApplicationDbContext.Instance;
        }

        public List<Feedback> GetAllFeedbacks()
        {
            return _context.Feedbacks
                .Include(f => f.Student)
                .Include(f => f.LifeSkillCourse)
                .ToList();
        }

        public Feedback GetFeedbackById(int id)
        {
            return _context.Feedbacks
                .Include(f => f.Student)
                .Include(f => f.LifeSkillCourse)
                .FirstOrDefault(f => f.FeedbackId == id);
        }

        public void AddFeedback(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
        }

        public void UpdateFeedback(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            _context.SaveChanges();
        }

        public void DeleteFeedback(int id)
        {
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.FeedbackId == id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                _context.SaveChanges();
            }
        }

        // Thêm method để lấy feedback theo khóa học
        public List<Feedback> GetFeedbacksByCourse(int courseId)
        {
            return _context.Feedbacks
                .Include(f => f.Student)
                .Include(f => f.LifeSkillCourse)
                .Where(f => f.CourseId == courseId)
                .ToList();
        }

        // Thêm method để lấy feedback theo tháng
        public List<Feedback> GetFeedbacksByMonth(int month, int year)
        {
            return _context.Feedbacks
                .Include(f => f.Student)
                .Include(f => f.LifeSkillCourse)
                .Where(f => f.FeedbackDate.Month == month && f.FeedbackDate.Year == year)
                .ToList();
        }
    }
}