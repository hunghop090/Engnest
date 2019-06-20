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
	public class CommentController : BaseController
	{
		private IUserRepository userRepository;
		private ICommentRepository commentRepository;

		public CommentController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.commentRepository = new CommentRepository(new EngnestContext());
		}

		public CommentController(IUserRepository userRepository, ICommentRepository commentRepository)
		{
			this.userRepository = userRepository;
			this.commentRepository = commentRepository;
		}
		public ActionResult LoadCommentsPost(string PostIds,string date,int quantity)
		{
			try
			{
				var id = userLogin.ID;
				var data = commentRepository.LoadCommentsPost(PostIds, date, quantity,string.Empty,id);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		public ActionResult LoadCommentsReply(string PostIds,string date,int quantity)
		{
			try
			{
				var id = userLogin.ID;
				var data = commentRepository.LoadCommentsPost(PostIds, date, quantity,string.Empty,id);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		[HttpPost]
		public ActionResult CreatedComment(Comment model)
		{
			var data = new List<CommentViewModel>();
			if (ModelState.IsValid)
			{
				try
				{
					var DateTime = commentRepository.InsertComment(model);
					data = commentRepository.LoadCommentsPost(model.TargetId.ToString(), DateTime, 1,model.UserId.ToString(),userLogin.ID);
				}
				catch (Exception ex)
				{
					return Json(new { result = Constant.ERROR });
				}
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS,data });
		}
	}
}