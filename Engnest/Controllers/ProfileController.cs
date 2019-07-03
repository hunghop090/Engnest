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
			if(id == null)
				model = Mapper.Map<ProfileModel>(userLogin);
			else
			{
				model = Mapper.Map<ProfileModel>(userRepository.GetUserByID(id.Value)); 
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
				var OldPassword = EncryptorMD5.MD5Hash(model.OldPassword);
				var NewPassword = EncryptorMD5.MD5Hash(model.NewPassword);
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
		public ActionResult UploadAvatar(string Avatar)
		{
			try
			{
				var user = userRepository.GetUserByIDForUpdate(userLogin.ID);
				user.Avatar = AmazonS3Uploader.UploadFile(Avatar,TypeUpload.IMAGE);
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
				user.BackGround = AmazonS3Uploader.UploadFile(BackGround,TypeUpload.IMAGE);
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
	}
}