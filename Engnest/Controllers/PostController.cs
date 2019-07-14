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
	public class PostController : BaseController
	{
		private IUserRepository userRepository;
		private IPostRepository postRepository;

		public PostController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.postRepository = new PostRepository(new EngnestContext());
		}

		public PostController(IUserRepository userRepository, IPostRepository postRepository)
		{
			this.userRepository = userRepository;
			this.postRepository = postRepository;
		}
		public ActionResult Index(long? id,long? CommentId)
		{
			if(id == null && CommentId == null)
				return RedirectToAction("Index","Home");
			else
			{
				PostIndexModel model = new PostIndexModel();
				model.Id = id;
				model.CommentId = CommentId;
				model.Profile = Mapper.Map<ProfileModel>(userLogin);
				return View(model);
			}
		}

		public ActionResult LoadPost(long? id,long? CommentId)
		{
			try
			{
				List<PostViewModel> data = new List<PostViewModel>();
				var result = postRepository.LoadPostById(id,CommentId,userLogin.ID);
				data.Add(result);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}
	}
}