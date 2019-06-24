using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Engnest.Entities.Repository
{
	public class GroupRepository : IGroupRepository, IDisposable
	{
		private EngnestContext context;

		public GroupRepository(EngnestContext context)
		{
			this.context = context;
		}

		public IEnumerable<Group> GetGroups()
		{
			return context.Groups.ToList();
		}

		public Group GetLastGroups()
		{
			return context.Groups.OrderByDescending(x=>x.CreatedTime).FirstOrDefault();
		}

		public Group GetGroupByID(long id)
		{
			return context.Groups.Find(id);
		}

		public List<MemberModel> GetMember(long UserId)
		{
			var result = (from c in context.GroupMembers
						  join p1 in context.Users on c.UserID equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  where c.Status == StatusMember.ACCEPT
						  orderby c.CreatedTime descending
						  select new MemberModel
						  {
							  CreatedTime = c.CreatedTime,
							  Avatar = p1.Avatar,
							  NickName = p1.NickName,
							  Type = c.Type,
							  Id = p1.ID
						  }).ToList();
			return result;
		}

		public void InsertGroup(Group Group)
		{
			Group.CreatedTime = DateTime.UtcNow;
			context.Groups.Add(Group);
			Save();
		}

		public void DeleteGroup(long GroupID)
		{
			Group Group = context.Groups.Find(GroupID);
			context.Groups.Remove(Group);
		}

		public void UpdateGroup(Group Group)
		{
			context.Entry(Group).State = EntityState.Modified;
		}

		public void Save()
		{
			context.SaveChanges();
		}

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					context.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}