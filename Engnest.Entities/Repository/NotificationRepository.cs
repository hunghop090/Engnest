using AutoMapper;
using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Engnest.Entities.Repository
{
	public class NotificationRepository : INotificationRepository, IDisposable
	{
		private EngnestContext context;

		public NotificationRepository(EngnestContext context)
		{
			this.context = context;
		}

		public List<Notification> GetNotifications()
		{
			return context.Notifications.ToList();
		}

		public Comment GetCommentByUpdate(long id)
		{
			return context.Comments.Find(id);
		}
		public List<Notification> LoadNotifications(string date,long UserId, int quantity)
		{
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			return context.Notifications.Where(x=>x.CreatedTime  < createDate && UserId == x.UserId).OrderByDescending(c=>c.CreatedTime).Take(quantity).ToList();
		}

		public Notification GetNotificationByID(long id)
		{
			Notification Notification = context.Notifications.Find(id);
			return Notification;
		}

			public Notification GetNotificationByTarget(byte Type,long TargetId)
		{
			Notification Notification = context.Notifications.Where(x=>x.Type == Type && x.TargetId.Value == TargetId).FirstOrDefault();
			return Notification;
		}
		public long InsertNotification(Notification Notification)
		{
			Notification.Seen = false;
			Notification.CreatedTime = DateTime.UtcNow;
			var a = context.Notifications.Add(Notification);
			Save();
			return a.ID;
		}

		public void DeleteNotification(long NotificationID)
		{
			Notification Notification = context.Notifications.Find(NotificationID);
			context.Notifications.Remove(Notification);
			Save();
		}

		public void UpdateNotification(Notification Notification)
		{
			context.Entry(Notification).State = EntityState.Modified;
			Save();
		}

		public void UpdateSeen(long Id)
		{
			context.Notifications.Where(x => x.ID == Id).ToList().ForEach(a => a.Seen = true);
			Save();
		}

		public void Save()
		{
			context.SaveChanges();
		}

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					context.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}