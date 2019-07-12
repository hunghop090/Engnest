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
				data.Reverse();
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		public ActionResult LoadCommentsReply(string CommentId,string date,int quantity)
		{
			try
			{
				var id = userLogin.ID;
				var data = commentRepository.LoadCommentsReply(CommentId, date, quantity,string.Empty,id);
				data.Reverse();
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		[HttpPost]
		public ActionResult CreatedComment(CommentViewModel model)
		{
			var data = new CommentViewModel();
			if (ModelState.IsValid)
			{
				try
				{
					Comment comment = new Comment();
					comment = Mapper.Map<Comment>(model);
					if(model.ListImages != null)
						foreach(var item in model.ListImages)
						{
							if(string.IsNullOrEmpty(comment.Images))
								comment.Images += AmazonS3Uploader.UploadFile(item,TypeUpload.IMAGE);
							else
								comment.Images += "," + AmazonS3Uploader.UploadFile(item,TypeUpload.IMAGE);
						}
					var idCommnet = commentRepository.InsertComment(comment);
					data = commentRepository.GetCommentByID(idCommnet,userLogin.ID);
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