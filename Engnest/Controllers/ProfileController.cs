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
	}
}