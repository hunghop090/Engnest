using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  IGroupRepository : IDisposable
	{
		IEnumerable<Group> GetGroups();
		Group GetLastGroups();
		List<MemberModel> GetMember(long UserId,string date);
		List<MemberModel> GetMemberSending(long UserId,string date);
		MemberModel GetMemberGroupByID(long id,long groupId);
		GroupMember GetGroupMemberByID(long id,long groupId);
		List<GroupModel> GetListGroup(long Id);
        Group GetGroupByID(long GroupId);
		Group GetGroupByIDForUpdate(long GroupId);
        void InsertGroup(Group Group);
        void DeleteGroup(long GroupID);
        void UpdateGroup(Group Group);

		void InsertGroupMember(GroupMember GroupMember);
        void DeleteGroupMember(long id,long GroupId);
        void UpdateGroupMember(GroupMember GroupMember);
        void Save();
	}
}