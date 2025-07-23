using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface IActivityLogRepository
    {
        List<ActivityLog> GetAllActivityLogs();
        ActivityLog GetActivityLogById(int id);
        void AddActivityLog(ActivityLog activityLog);
        void UpdateActivityLog(ActivityLog activityLog);
        void DeleteActivityLog(int id);
    }
}
