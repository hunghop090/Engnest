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
				var data = groupRepository.GetGroupMemberByID(userLogin.ID,id.Value);
				if(data?.Type == TypeMember.ADMIN)
					ViewBag.ClassUpdate = true;
				else
					ViewBag.ClassUpdate = false;
				if(data != null)
				{
					ViewBag.ClassPost = true;
					ViewBag.IsMember = data.Status;
				}
				else
				{
					ViewBag.ClassPost = false;
					ViewBag.IsMember = StatusMember.NONMEMBER;
				}
				return View(model);
			}
		}

		public ActionResult Member(long? id)
		{
			GroupModel model = new GroupModel();
			if(id == null)
				return RedirectToAction("Index","Home");
			else
			{
				model = Mapper.Map<GroupModel>(groupRepository.GetGroupByID(id.Value)); 
				model.Profile = Mapper.Map<ProfileModel>(userLogin); 
				var data = groupRepository.GetMemberGroupByID(userLogin.ID,id.Value);
				if(data?.Type == TypeMember.ADMIN)
					ViewBag.ClassUpdate = true;
				else
					ViewBag.ClassUpdate = false;
				return View(model);
			}
		}
		public ActionResult LoadMember(long id,string date)
		{
			try
			{
				var data = groupRepository.GetMember(id,date);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult LoadRequest(long id,string date)
		{
			try
			{
				var data = groupRepository.LoadRequest(id,date);
				return Json(new { result = Constant.SUCCESS, data = data }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult LoadMemberSending(long id,string date)
		{
			try
			{
				var data = groupRepository.GetMemberSending(id,date);
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

		public ActionResult LoadListGroup(long? id)
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
		public ActionResult UploadAvatar(string Avatar,long id)
		{
			try
			{
				var group = groupRepository.GetGroupByIDForUpdate(id);
				//Task<string> task = Task.Run<string>(async () => await UploadImageTifiny.TinifyModulAsync(Avatar,TypeUpload.IMAGE));
				var result = AmazonS3Uploader.UploadFile(Avatar,TypeUpload.IMAGE);
				if(result == "" )
					return Json(new { result = Constant.ERROR,message = "Error!" });
				group.Avatar = result;
				groupRepository.UpdateGroup(group);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult UploadBackGround(string BackGround,long id)
		{
			try
			{
				var group = groupRepository.GetGroupByIDForUpdate(id);
				//Task<string> task = Task.Run<string>(async () => await UploadImageTifiny.TinifyModulAsync(BackGround,TypeUpload.IMAGE));
				var result = AmazonS3Uploader.UploadFile(BackGround,TypeUpload.IMAGE);
				if(result == "" )
					return Json(new { result = Constant.ERROR,message = "Error!" });
				group.Banner = result;
				groupRepository.UpdateGroup(group);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult SetAdmin(long UserId ,long id,byte Type)
		{
			try
			{
				var UserGroup = groupRepository.GetGroupMemberByID(userLogin.ID,id);
				if(UserGroup != null && UserGroup.Type == TypeMember.ADMIN)
				{
					var MemberGroup = groupRepository.GetGroupMemberByID(UserId,id);
					MemberGroup.Type = Type;
					groupRepository.UpdateGroupMember(MemberGroup);
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
		public ActionResult GetOut(long id)
		{
			try
			{
				groupRepository.DeleteGroupMember(userLogin.ID,id);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult JoinGroup(long id,byte type)
		{
			try
			{
				GroupMember request = new GroupMember();
				if(type == StatusMember.NONMEMBER)
				{
					request = groupRepository.GetGroupMemberByID(userLogin.ID,id);
					if(request != null && request.ID != 0)
						groupRepository.DeleteGroupMember(userLogin.ID,id);

				} else if(type == StatusRequestFriend.SENDING)
				{
					var member = groupRepository.GetGroupMemberByID(userLogin.ID,id);
					if(member != null)
						groupRepository.DeleteGroupMember(userLogin.ID,id);
					request.GroupID = id;
					request.UserId = userLogin.ID;
					request.Status = StatusMember.SENDING;
					request.Type = TypeMember.MEMBER;
					groupRepository.InsertGroupMember(request);
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
		public ActionResult KickOut(long UserId ,long id)
		{
			try
			{
				var UserGroup = groupRepository.GetGroupMemberByID(userLogin.ID,id);
				if(UserGroup != null && UserGroup.Type == TypeMember.ADMIN)
				{
					groupRepository.DeleteGroupMember(UserId,id);
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
		public ActionResult SaveName(string GroupName ,long Id)
		{
			try
			{
				if(GroupName.Length > 50  || GroupName.Length < 10)
					return Json(new { result = Constant.ERROR });
				var group = groupRepository.GetGroupByIDForUpdate(Id);
				if(group != null)
				{
					group.GroupName = GroupName;
				}
				groupRepository.UpdateGroup(group);
			}
			catch (Exception ex)
			{
				return Json(new { result = Constant.ERROR,message = "Error!" });
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = Constant.SUCCESS });
		}

		[HttpPost]
		public ActionResult AcceptRequestGroup(long id,long groupId,byte type)
		{
			try
			{
				GroupMember request = new GroupMember();
				if(type == StatusMember.ACCEPT)
				{
					request = groupRepository.GetGroupMemberByID(id,groupId);
					if(request != null && request.ID != 0)
					{
						request.Status = StatusMember.ACCEPT;
						groupRepository.UpdateGroupMember(request);
					}
				} else if(type == StatusRequestFriend.REJECT)
				{
					request = groupRepository.GetGroupMemberByID(id,groupId);
					if(request != null)
						groupRepository.DeleteGroupMember(id,groupId);
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