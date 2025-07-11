using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class ActivityLogDAO
    {
        private readonly ApplicationDbContext _context;
        public ActivityLogDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<ActivityLog> GetAllActivityLogs()
        {
            return _context.ActivityLogs.ToList();
        }
        public ActivityLog GetActivityLogById(int id)
        {
            return _context.ActivityLogs.FirstOrDefault(a => a.LogId == id);
        }
        public void AddActivityLog(ActivityLog activityLog)
        {
            _context.ActivityLogs.Add(activityLog);
            _context.SaveChanges();
        }
        public void UpdateActivityLog(ActivityLog activityLog)
        {
            _context.ActivityLogs.Update(activityLog);
            _context.SaveChanges();
        }
        public void DeleteActivityLog(int id)
        {
            var activityLog = _context.ActivityLogs.FirstOrDefault(a => a.LogId == id);
            if (activityLog != null)
            {
                _context.ActivityLogs.Remove(activityLog);
                _context.SaveChanges();
            }
        }
    }
}
