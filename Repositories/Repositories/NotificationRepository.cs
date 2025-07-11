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
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDAO _notificationDAO;
        public NotificationRepository()
        {
            _notificationDAO = new NotificationDAO();
        }
        public void AddNotification(Notification notification) => _notificationDAO.AddNotification(notification);
        public void DeleteNotification(int id) => _notificationDAO.DeleteNotification(id);
        public Notification GetNotificationById(int id) => _notificationDAO.GetNotificationById(id);
        public List<Notification> GetAllNotifications() => _notificationDAO.GetAllNotifications();
        public void UpdateNotification(Notification notification) => _notificationDAO.UpdateNotification(notification);
    }
}
