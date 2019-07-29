using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
		public ActionResult LoadCommentsPost(string PostIds, string date, int quantity)
		{
			try
			{
				var id = userLogin.ID;
				var data = commentRepository.LoadCommentsPost(PostIds, date, quantity, id);
				data.Reverse();
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		public ActionResult LoadCommentsReply(string CommentId, string date, int quantity)
		{
			try
			{
				var id = userLogin.ID;
				var data = commentRepository.LoadCommentsReply(CommentId, date, quantity, string.Empty, id);
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
					if (model.ListImages != null)
						foreach (var item in model.ListImages)
						{
							if (string.IsNullOrEmpty(comment.Images))
							{
								//Task<string> task = Task.Run<string>(async () => await UploadImageTifiny.TinifyModulAsync(item, TypeUpload.IMAGE));
								//if (task.Result == "")
								//	continue;
								//comment.Images += task.Result;
								comment.Images += AmazonS3Uploader.UploadFile(item,TypeUpload.IMAGE);
							}
							else
							{
								//Task<string> task = Task.Run<string>(async () => await UploadImageTifiny.TinifyModulAsync(item, TypeUpload.IMAGE));
								//if (task.Result == "")
								//	continue;
								//comment.Images += "," + task.Result;
								comment.Images += "," + AmazonS3Uploader.UploadFile(item,TypeUpload.IMAGE);
							}
						}
					var idCommnet = commentRepository.InsertComment(comment);
					data = commentRepository.GetCommentByID(idCommnet, userLogin.ID);
				}
				catch (Exception ex)
				{
					return Json(new { result = Constant.ERROR });
				}
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS, data });
		}

		public ActionResult DeleteComment(long? id)
		{
			try
			{
				var result = commentRepository.GetCommentByUpdate(id.Value);
				if(result != null && (result.UserId == userLogin.ID ))
				{
					commentRepository.DeleteComment(id.Value);
					return Json(new { result = Constant.SUCCESS }, JsonRequestBehavior.AllowGet);
				}
				return Json(new { result = Constant.ERROR }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}
	}
}