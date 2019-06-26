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
		private IGroupRepository groupRepository;
		public HomeController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.postRepository = new PostRepository(new EngnestContext());
			this.emotionRepository = new EmotionRepository(new EngnestContext());
			this.groupRepository = new GroupRepository(new EngnestContext());
		}

		public HomeController(IUserRepository userRepository, IPostRepository postRepository, IEmotionRepository emotionRepository, IGroupRepository groupRepository)
		{
			this.userRepository = userRepository;
			this.postRepository = postRepository;
			this.emotionRepository = emotionRepository;
			this.groupRepository = groupRepository;
		}
		public ActionResult Index()
		{
			HomeModel model = new HomeModel();
			model.Profile = Mapper.Map<ProfileModel>(userLogin);
			return View(model);
		}
		public ActionResult LoadMorePost(string date,long? id,byte? type)
		{
			try
			{
				List<PostViewModel> data = new List<PostViewModel>();
				if(id == null)
				{
					id = userLogin.ID;
					data = postRepository.LoadPostsHome(date, id.Value);
				}
				else
				{
					if(type == TypePost.GROUP)
						data = postRepository.LoadPostsGroup(date, id.Value);
					else
						data = postRepository.LoadPostsProfile(date, id.Value);
				}

				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		public ActionResult LoadRequestFriend()
		{
			try
			{
				var id = userLogin.ID;
				List<RequestFriendModel> data = new List<RequestFriendModel>();
				data = userRepository.GetRequestFriend(id);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult LoadImage(long? id)
		{
			try
			{
				if(id == null)
					id = userLogin.ID;
				var data = postRepository.GetListImage(id.Value);
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
		public ActionResult CreatedGroup(string GroupName,long UserID)
		{
			Group newgroup = new Group();
			try
			{
				Group group = new Group();
				group.GroupName = GroupName;
				group.CreatedUser = UserID;
				groupRepository.InsertGroup(group);
				newgroup = groupRepository.GetLastGroups();
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS,data = newgroup.ID });
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
						emotion.CreatedTime = DateTime.UtcNow;
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