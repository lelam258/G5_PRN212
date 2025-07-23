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
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ActivityLogDAO _activityLogDAO;
        public ActivityLogRepository()
        {
            _activityLogDAO = new ActivityLogDAO();
        }
        public void AddActivityLog(ActivityLog activityLog) => _activityLogDAO.AddActivityLog(activityLog);

        public void DeleteActivityLog(int id) => _activityLogDAO.DeleteActivityLog(id);

        public ActivityLog GetActivityLogById(int id) => _activityLogDAO.GetActivityLogById(id);

        public List<ActivityLog> GetAllActivityLogs() => _activityLogDAO.GetAllActivityLogs();

        public void UpdateActivityLog(ActivityLog activityLog) => _activityLogDAO.UpdateActivityLog(activityLog);
    }
}
