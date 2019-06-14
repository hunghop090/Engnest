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
		private IEmotionRepository emotionRepository;

		public HomeController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.postRepository = new PostRepository(new EngnestContext());
			this.emotionRepository = new EmotionRepository(new EngnestContext());
		}

		public HomeController(IUserRepository userRepository, IPostRepository postRepository, IEmotionRepository emotionRepository)
		{
			this.userRepository = userRepository;
			this.postRepository = postRepository;
			this.emotionRepository = emotionRepository;
		}
		public ActionResult Index()
		{
			HomeModel model = new HomeModel();
			model.ProfileModel = Mapper.Map<ProfileModel>(userLogin);
			return View(model);
		}
		public ActionResult LoadMorePost(string date)
		{
			try
			{
				var id = userLogin.ID;
				var data = postRepository.LoadPostsHome(date, userLogin.ID);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
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
					if(model.ListImages != null)
						foreach(var item in model.ListImages)
						{
							if(string.IsNullOrEmpty(post.Images))
								post.Images += AmazonS3Uploader.UploadFile(item,TypeUpload.IMAGE);
							else
								post.Images += "," + AmazonS3Uploader.UploadFile(item,TypeUpload.IMAGE);
						}
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

		[HttpPost]
		public ActionResult CreatedAudio(HttpPostedFileBase file)
		{
			var url = string.Empty;
			var respons = AmazonS3Uploader.UploadFileStream(file.InputStream,TypeUpload.AUDIO);
			if (!string.IsNullOrEmpty(respons))
			{
				url = AmazonS3Uploader.GetUrl(respons);
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS,data =  url,key = respons});
		}

		[HttpPost]
		public ActionResult LikeAction(long PostId)
		{
			byte status = 0;
			if (ModelState.IsValid)
			{
				try
				{
					var oldEmotion = emotionRepository.GetEmotionByTargetId(PostId);
					if (oldEmotion.Count != 0)
					{
						if(oldEmotion.FirstOrDefault().Status == 0)
							oldEmotion.FirstOrDefault().Status = 1;
						else
							oldEmotion.FirstOrDefault().Status = 0;
						status = oldEmotion.FirstOrDefault().Status;
						emotionRepository.UpdateEmotion(oldEmotion.First());
					}
					else
					{
						Emotion emotion = new Emotion();
						emotion.TargetId = PostId;
						emotion.TargetType = "post";
						emotion.UserId = userLogin.ID;
						emotion.CreatedTime = DateTime.Now;
						emotion.Status = 1;
						emotionRepository.InsertEmotion(emotion);
						status = 1;
					}
				}
				catch (Exception ex)
				{
					return Json(new { result = Constant.ERROR });
				}
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS, status});
		}
	}
}