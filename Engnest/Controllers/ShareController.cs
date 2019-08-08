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
	public class ShareController : Controller
	{
		private IUserRepository userRepository;
		private IPostRepository postRepository;
		private IGroupRepository groupRepository;
		private ICommentRepository commentRepository;

		public ShareController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.postRepository = new PostRepository(new EngnestContext());
			this.groupRepository = new GroupRepository(new EngnestContext());
			this.commentRepository = new CommentRepository(new EngnestContext());
		}

		public ShareController(IUserRepository userRepository, IPostRepository postRepository, IGroupRepository groupRepository, ICommentRepository commentRepository)
		{
			this.userRepository = userRepository;
			this.postRepository = postRepository;
			this.groupRepository = groupRepository;
			this.commentRepository = commentRepository;
		}
		public ActionResult Index(long? id)
		{
			if(id == null)
				return RedirectToAction("Index","Login");
			else
			{
				PostIndexModel model = new PostIndexModel();
				model.Id = id;
				return View(model);
			}
		}

		public ActionResult LoadPost(long? id)
		{
			try
			{
				List<PostViewModel> data = new List<PostViewModel>();
				var result = postRepository.LoadPostById(id,null);
				data.Add(result);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult LoadCommentsPost(string PostIds, string date, int quantity)
		{
			try
			{
				var data = commentRepository.LoadCommentsPost(PostIds, date, quantity);
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
				var data = commentRepository.LoadCommentsReply(CommentId, date, quantity, string.Empty);
				data.Reverse();
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}
	}
}