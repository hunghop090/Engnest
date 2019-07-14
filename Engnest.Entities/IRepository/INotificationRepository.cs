using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  INotificationRepository : IDisposable
	{
        List<Notification> GetNotifications();
		List<Notification> LoadNotifications(string date,long UserId, int quantity);
        Notification GetNotificationByID(long NotificationId);
		Notification GetNotificationByTarget(byte Type,long TargetId);
        long InsertNotification(Notification Notification);
        void DeleteNotification(long NotificationID);
        void UpdateNotification(Notification Notification);
		void UpdateSeen(long Id);
        void Save();
	}
}