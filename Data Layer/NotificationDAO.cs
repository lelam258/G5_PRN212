using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Data_Layer
{
    public class NotificationDAO
    {
        private readonly ApplicationDbContext _context;
        public NotificationDAO()
        {
            _context = ApplicationDbContext.Instance;
        }
        public List<Notification> GetAllNotifications()
        {
            return _context.Notifications.ToList();
        }
        public Notification GetNotificationById(int id)
        {
            return _context.Notifications.FirstOrDefault(n => n.NotificationId == id);
        }
        public void AddNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }
        public void UpdateNotification(Notification notification)
        {
            _context.Notifications.Update(notification);
            _context.SaveChanges();
        }
        public void DeleteNotification(int id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.NotificationId == id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }
        }
    }
}
