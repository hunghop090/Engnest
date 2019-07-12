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
	public class MessageController : BaseController
	{
		private IUserRepository userRepository;
		private IMessageRepository MessageRepository;

		public MessageController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.MessageRepository = new MessageRepository(new EngnestContext());
		}

		public MessageController(IUserRepository userRepository, IMessageRepository MessageRepository)
		{
			this.userRepository = userRepository;
			this.MessageRepository = MessageRepository;
		}
		public ActionResult LoadMessages(long UserId,string date,long TargetId)
		{
			try
			{
				var id = userLogin.ID;
				var data = MessageRepository.LoadMessages(date, UserId, TargetId);
				if(data.Count == 0)
				{
					var Message = new MessageViewModel();
					var userTaget = userRepository.GetUserByID(TargetId);
					Message.AvataTarget = userTaget.Avatar;
					Message.NickNameTarget = userTaget.NickName ;
					data.Add(Message);
				}
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult LoadMessagesNotifi()
		{
			try
			{
				var id = userLogin.ID;
				var data = MessageRepository.LoadMessagesNotifi(userLogin.ID);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult CreatedMessage(Message model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					MessageRepository.InsertMessage(model);
				}
				catch (Exception ex)
				{
					return Json(new { result = Constant.ERROR });
				}
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult SeenMessage(long TargetId)
		{
			try
			{
				MessageRepository.UpdateSeen(TargetId,userLogin.ID);
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