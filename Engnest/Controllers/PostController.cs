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
		private IGroupRepository groupRepository;

		public PostController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.postRepository = new PostRepository(new EngnestContext());
			this.groupRepository = new GroupRepository(new EngnestContext());
		}

		public PostController(IUserRepository userRepository, IPostRepository postRepository, IGroupRepository groupRepository)
		{
			this.userRepository = userRepository;
			this.postRepository = postRepository;
			this.groupRepository = groupRepository;
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

		public ActionResult DeletePost(long? id)
		{
			try
			{
				var result = postRepository.GetPostByID(id.Value);
				if(result != null && ((result.TargetId == userLogin.ID && result.TargetType == TypePost.USER) || result.UserId == userLogin.ID ))
				{
					postRepository.DeletePost(id.Value);
					return Json(new { result = Constant.SUCCESS }, JsonRequestBehavior.AllowGet);
				}
				else 
				{
					if(result.TargetType == TypePost.GROUP)
					{
						var data = groupRepository.GetMemberGroupByID(userLogin.ID,result.TargetId.Value);
						if(data?.Type == TypeMember.ADMIN)
						{
							postRepository.DeletePost(id.Value);
							return Json(new { result = Constant.SUCCESS }, JsonRequestBehavior.AllowGet);
						}
					}
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