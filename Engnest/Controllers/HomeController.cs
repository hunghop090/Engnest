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
	public class HomeController : BaseController
	{
		private IUserRepository userRepository;
		private IPostRepository postRepository;

		public HomeController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.postRepository = new PostRepository(new EngnestContext());
		}

		public HomeController(IUserRepository userRepository, IPostRepository postRepository)
		{
			this.userRepository = userRepository;
			this.postRepository = postRepository;
		}
		public ActionResult Index()
		{
			var fooDto = Mapper.Map<ProfileModel>(userLogin);
			return View();
		}
		public ActionResult LoadMorePost(string date)
		{
			try
			{
				var id = userLogin.ID;
				var data = postRepository.LoadPostsHome(date, userLogin.ID);
				return Json(new { result = Constant.SUCCESS, data = data },JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message },JsonRequestBehavior.AllowGet);
			}

			
		}

		[HttpPost]
		public ActionResult CreatedPost(PostModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					Post post = new Post();
					post = Mapper.Map<Post>(model);
					postRepository.InsertPost(post);
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