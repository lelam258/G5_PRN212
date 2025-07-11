using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer;

namespace Repositories.Interfaces
{
    public interface INotificationRepository
    {
        List<Notification> GetAllNotifications();
        Notification GetNotificationById(int id);
        void AddNotification(Notification notification);
        void UpdateNotification(Notification notification);
        void DeleteNotification(int id);
    }
}
