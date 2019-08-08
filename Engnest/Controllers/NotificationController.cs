using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.Repository;
using Engnest.Entities.ViewModels;

namespace Engnest.Controllers
{
	public class NotificationController : BaseController
	{
		private IUserRepository userRepository;
		private INotificationRepository NotificationRepository;

		public NotificationController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.NotificationRepository = new NotificationRepository(new EngnestContext());
		}

		public NotificationController(IUserRepository userRepository, INotificationRepository NotificationRepository)
		{
			this.userRepository = userRepository;
			this.NotificationRepository = NotificationRepository;
		}
		public ActionResult LoadNotification(string date,int quantity)
		{
			try
			{
				var id = userLogin.ID;
				var data = NotificationRepository.LoadNotifications(date,id, quantity);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}


		[HttpPost]
		[ValidateInput(false)]
		public ActionResult CreatedNotification(string Content,long UserId,byte? Type,long? TargetId)
		{
			var newNotification = new Notification(); 
			newNotification.Content = Content;
			newNotification.UserId = UserId;
			newNotification.Type = Type;
			newNotification.TargetId = TargetId;
			var id = (long)0;
			try
			{
				if(TargetId != null && Type != null)
				{
					var OldNOtification = NotificationRepository.GetNotificationByTarget(Type.Value,TargetId.Value);
					if(OldNOtification != null)
					{
						OldNOtification.Content = Content;
						OldNOtification.Seen = false;
						OldNOtification.UserId = UserId;
						OldNOtification.CreatedTime = DateTime.UtcNow;
						NotificationRepository.UpdateNotification(OldNOtification);
						id = OldNOtification.ID;
					}
					else
						id = NotificationRepository.InsertNotification(newNotification);
				}
				else
					id = NotificationRepository.InsertNotification(newNotification);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS,id });
		}

		[HttpPost]
		public ActionResult SeenNotification(long Id)
		{
			try
			{
				NotificationRepository.UpdateSeen(Id);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}
	}
}