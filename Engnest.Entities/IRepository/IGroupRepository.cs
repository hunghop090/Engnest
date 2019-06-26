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
		List<MemberModel> GetMember(long UserId);
		List<GroupModel> GetListGroup(long Id);
        Group GetGroupByID(long GroupId);
        void InsertGroup(Group Group);
        void DeleteGroup(long GroupID);
        void UpdateGroup(Group Group);
        void Save();
	}
}