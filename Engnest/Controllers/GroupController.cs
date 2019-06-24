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
	public class GroupController : BaseController
	{
		private IUserRepository userRepository;
		private IGroupRepository groupRepository;

		public GroupController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
			this.groupRepository = new GroupRepository(new EngnestContext());
		}

		public GroupController(IUserRepository userRepository,IGroupRepository groupRepository)
		{
			this.userRepository = userRepository;
			this.groupRepository = groupRepository;
		}
		public ActionResult Index(long? id)
		{
			GroupModel model = new GroupModel();
			if(id == null)
				return RedirectToAction("Index","Home");
			else
			{
				model = Mapper.Map<GroupModel>(groupRepository.GetGroupByID(id.Value)); 
				model.Profile = Mapper.Map<ProfileModel>(userLogin); 
				return View(model);
			}
		}

		public ActionResult LoadMember(long id)
		{
			try
			{
				var data = groupRepository.GetMember(id);
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
				var data = userRepository.GetFriend(id.Value);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}
	}
}