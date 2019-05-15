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
		public ActionResult LoadCommentsPost(string PostIds)
		{
			try
			{
				var id = userLogin.ID;
				var data = commentRepository.LoadCommentsPost(PostIds);
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
			if (ModelState.IsValid)
			{
				try
				{
					commentRepository.InsertComment(model);
				}
				catch (Exception ex)
				{
					return Json(new { result = Constant.ERROR });
				}
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}
	}
}