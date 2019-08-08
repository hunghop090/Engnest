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
	public class ProfileController : BaseController
	{
		private IUserRepository userRepository;
		private IGroupRepository groupRepository;
		public ProfileController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.groupRepository = new GroupRepository(new EngnestContext());
		}

		public ProfileController(IUserRepository userRepository,IGroupRepository groupRepository)
		{
			this.userRepository = userRepository;
			this.groupRepository = groupRepository;
		}
		public ActionResult Index(long? id)
		{
			ProfileModel model = new ProfileModel();
			ViewBag.IsFriend = StatusRequestFriend.CANCEL;
			if(id == null)
				model = Mapper.Map<ProfileModel>(userLogin);
			else
			{
				model = Mapper.Map<ProfileModel>(userRepository.GetUserByID(id.Value)); 
				var request = userRepository.GetRequestFriendByUser(id.Value,userLogin.ID);
				if(request != null)
				{
					ViewBag.IsFriend = request.Status;
				}
					
			}
			if(model.ID == userLogin.ID)	
				ViewBag.ClassUpdate = "";
			else
				ViewBag.ClassUpdate = "hidden";
			return View(model);
		}

		public ActionResult GeneralSetting()
		{
			ProfileModel model = new ProfileModel();
			var user = userRepository.GetUserByID(userLogin.ID);
			model = Mapper.Map<ProfileModel>(user);
			return View(model);
		}

		public ActionResult LoadFriend(long? id)
		{
			try
			{
				if(id == null)
					id = userLogin.ID;
				var data = userRepository.GetFriend(id.Value);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		public ActionResult SearchFriend(string query)
		{
			try
			{
				var data = userRepository.SearchFriend(userLogin.ID,query);
				var dataGroup = groupRepository.SearchGroup(userLogin.ID,query);
				foreach(var item in dataGroup)
				{
					data.Add(item);
				}
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		public ActionResult GetSuggestFriend(string query,bool? location,string category)
		{
			try
			{
				var data = userRepository.GetSuggestFriend(userLogin.ID,query,location,category);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}

		public ActionResult LoadGroup(long? id)
		{
			try
			{
				if(id == null)
					id = userLogin.ID;
				var data = groupRepository.GetListGroup(id.Value);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult Update(ProfileModel model)
		{
			try
			{
				if(model.NickName.Length > 20 || model.NickName.Length < 4)
					return Json(new { result = Constant.ERROR,message = "Error!" });
				var NewPassword = "";
				var OldPassword = EncryptorMD5.MD5Hash(model.OldPassword);
				if(string.IsNullOrEmpty(model.NewPassword))
					NewPassword = OldPassword;
				else
					NewPassword = EncryptorMD5.MD5Hash(model.NewPassword);
				var user = userRepository.GetUserByIDForUpdate(userLogin.ID);
				if(OldPassword == user.Password.Trim())
				{
					user.Email = model.Email;
					user.Password = NewPassword;
					user.Phone = model.Phone;
					user.Birthday = model.Birthday;
					user.Country = model.Country;
					user.Gender = model.Gender;
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.Relationship = model.Relationship;
					user.AboutMe = model.AboutMe;
					user.Lat = model.Lat;
					user.Lng = model.Lng;
					user.Category = model.Category;
					user.NickName = model.NickName;
					userRepository.UpdateUser(user);
					userLogin = userRepository.GetUserByID(userLogin.ID);
				}
				else
				{
					return Json(new { result = Constant.ERROR,message = "Password incorrect!" });
				}
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult SaveHobbies(string data)
		{
			try
			{
				var user = userRepository.GetUserByIDForUpdate(userLogin.ID);
				user.Category = data;
				userRepository.UpdateUser(user);
				userLogin = userRepository.GetUserByID(userLogin.ID);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult UploadAvatar(string Avatar)
		{
			try
			{
				var user = userRepository.GetUserByIDForUpdate(userLogin.ID);
				var result = AmazonS3Uploader.UploadFile(Avatar,TypeUpload.IMAGE);
				//Task<string> task = Task.Run<string>(async () => await UploadImageTifiny.TinifyModulAsync(Avatar,TypeUpload.IMAGE));
				if(result == "" )
					return Json(new { result = Constant.ERROR,message = "Error!" });
				user.Avatar = result;
				userRepository.UpdateUser(user);
				userLogin = userRepository.GetUserByID(userLogin.ID);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult UploadBackGround(string BackGround)
		{
			try
			{
				var user = userRepository.GetUserByIDForUpdate(userLogin.ID);
				var result = AmazonS3Uploader.UploadFile(BackGround,TypeUpload.IMAGE);
				//Task<string> task = Task.Run<string>(async () => await UploadImageTifiny.TinifyModulAsync(BackGround,TypeUpload.IMAGE));
				//if(result == "" )
				//	return Json(new { result = Constant.ERROR,message = "Error!" });
				user.BackGround = result;
				userRepository.UpdateUser(user);
				userLogin = userRepository.GetUserByID(userLogin.ID);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult AcceptRequest(long id)
		{
			try
			{
				var request = userRepository.GetRequestFriendByID(id);
				request.Status = StatusRequestFriend.ACCEPT;
				userRepository.UpdateRequestFriend(request);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult RejectRequest(long id)
		{
			try
			{
				var request = userRepository.GetRequestFriendByID(id);
				request.Status = StatusRequestFriend.REJECT;
				userRepository.UpdateRequestFriend(request);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult AddFriend(long id,byte type)
		{
			try
			{
				Relationship request = new Relationship();
				if(type == StatusRequestFriend.CANCEL)
				{
					request = userRepository.GetRequestFriendByUser(id,userLogin.ID);
					if(request != null && request.ID != 0)
						userRepository.DeleteRequestFriend(request.ID);

				} else if(type == StatusRequestFriend.SENDING)
				{
					request.Type = TypeRequestFriend.USER;
					request.Status = StatusRequestFriend.SENDING;
					request.UserReceiveID = id;
					request.UserSentID = userLogin.ID;
					userRepository.InsertRequestFriend(request);
				}
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}
	}
}