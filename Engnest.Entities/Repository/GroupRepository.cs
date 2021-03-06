﻿using Engnest.Entities.Common;
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
			var result = context.Groups.OrderByDescending(x=>x.CreatedTime).FirstOrDefault();
			var respone = AmazonS3Uploader.GetUrl(result.Avatar,0);
			if(!string.IsNullOrEmpty(respone))
				result.Avatar = respone;
			respone = AmazonS3Uploader.GetUrl(result.Banner,0);
			if(!string.IsNullOrEmpty(respone))
				result.Banner = respone;
			return result;
		}

		public Group GetGroupByID(long id)
		{
			var result = context.Groups.Find(id);
			var respone = AmazonS3Uploader.GetUrl(result.Avatar,0);
			if(!string.IsNullOrEmpty(respone))
				result.Avatar = respone;
			respone = AmazonS3Uploader.GetUrl(result.Banner,0);
			if(!string.IsNullOrEmpty(respone))
				result.Banner = respone;
			return result;
		}

		public List<FriendModel> SearchGroup(long UserId,string query)
		{
			var ListFriend = new List<FriendModel>();
			var result = (from c in context.Groups
						  where c.GroupName.Contains(query)
						  orderby c.GroupName descending
						  select new { c }).Take(200).ToList();
			foreach (var item in result)
			{
				var Friend = new FriendModel();
				Friend.Avatar = item.c.Avatar;
				Friend.NickName = item.c.GroupName;
				Friend.Id = item.c.ID;
				Friend.Type = TypeRequestFriend.GROUP;
				Friend.CreatedTime = item.c.CreatedTime;
				var respone = AmazonS3Uploader.GetUrl(item.c.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					Friend.Avatar = respone;
				ListFriend.Add(Friend);
			}
			return ListFriend;
		}

		public GroupMember GetGroupMemberByID(long UserId,long GroupId)
		{
			var result = context.GroupMembers.Where(x => x.GroupID == GroupId && x.UserId == UserId).FirstOrDefault();
			return result;
		}

		public Group GetGroupByIDForUpdate(long id)
		{
			var result = context.Groups.Find(id);
			return result;
		}

		public MemberModel GetMemberGroupByID(long id,long groupId)
		{
			var result = (from c in context.GroupMembers
						  join p1 in context.Users on c.UserId equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  where c.Status == StatusMember.ACCEPT && c.GroupID == groupId && c.UserId == id
						  orderby c.CreatedTime descending
						  select new MemberModel
						  {
							  CreatedTime = c.CreatedTime,
							  Avatar = p1.Avatar,
							  NickName = p1.NickName,
							  Type = c.Type,
							  Id = p1.ID
						  }).FirstOrDefault();
			if(!string.IsNullOrEmpty( result?.Avatar))
			{
				var respone = AmazonS3Uploader.GetUrl(result.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					result.Avatar = respone;
			}
			return result;
		}

		public List<MemberModel> GetMember(long UserId,string date)
		{
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var result = (from c in context.GroupMembers
						  join p1 in context.Users on c.UserId equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  where c.CreatedTime < createDate && c.Status == StatusMember.ACCEPT && c.GroupID == UserId
						  orderby c.CreatedTime descending
						  select new MemberModel
						  {
							  CreatedTime = c.CreatedTime,
							  Avatar = p1.Avatar,
							  NickName = p1.NickName,
							  Type = c.Type,
							  Id = p1.ID
						  }).Take(10).ToList();
			foreach(var item in result)
			{
				var respone = AmazonS3Uploader.GetUrl(item.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					item.Avatar = respone;
			}
			return result;
		}

		public List<MemberModel> LoadRequest(long UserId,string date)
		{
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var result = (from c in context.GroupMembers
						  join p1 in context.Users on c.UserId equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  where c.CreatedTime < createDate && c.Status == StatusMember.SENDING && c.GroupID == UserId
						  orderby c.CreatedTime descending
						  select new MemberModel
						  {
							  CreatedTime = c.CreatedTime,
							  Avatar = p1.Avatar,
							  NickName = p1.NickName,
							  Type = c.Type,
							  Id = p1.ID
						  }).Take(10).ToList();
			foreach(var item in result)
			{
				var respone = AmazonS3Uploader.GetUrl(item.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					item.Avatar = respone;
			}
			return result;
		}


		public List<MemberModel> GetMemberSending(long UserId,string date)
		{
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var result = (from c in context.GroupMembers
						  join p1 in context.Users on c.UserId equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  where c.CreatedTime < createDate && c.Status == StatusMember.SENDING && c.GroupID == UserId
						  orderby c.CreatedTime descending
						  select new MemberModel
						  {
							  CreatedTime = c.CreatedTime,
							  Avatar = p1.Avatar,
							  NickName = p1.NickName,
							  Type = c.Type,
							  Id = p1.ID
						  }).Take(10).ToList();
			foreach(var item in result)
			{
				var respone = AmazonS3Uploader.GetUrl(item.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					item.Avatar = respone;
			}
			return result;
		}

		public List<GroupModel> GetListGroup(long Id)
		{
			var result = (from c in context.GroupMembers
						  where c.Status == StatusMember.ACCEPT && c.UserId == Id
						  join p1 in context.Groups on c.GroupID equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  orderby c.CreatedTime descending
						  select new GroupModel
						  {
							  CreatedTime = c.CreatedTime,
							  Avatar = p1.Avatar,
							  GroupName = p1.GroupName,
							  Banner = p1.Banner,
							  ID = p1.ID,
							  InfoId = p1.InfoId,
							  CreatedUser = p1.CreatedUser,
							  Status = p1.Status,
							  TypeMember = c.Type
						  }).ToList();
			foreach(var item in result)
			{
				var respone = AmazonS3Uploader.GetUrl(item.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					item.Avatar = respone;
				respone = AmazonS3Uploader.GetUrl(item.Banner,0);
				if(!string.IsNullOrEmpty(respone))
					item.Banner = respone;
			}
			return result;
		}

		public long InsertGroup(Group Group)
		{
			Group.CreatedTime = DateTime.UtcNow;
			Group.Avatar = "image/default-avatar.jpg";
			Group.Banner = "image/default-backgroud.jpg";
			var group = context.Groups.Add(Group);
			Save();
			return group.ID;
		}

		public void DeleteGroup(long GroupID)
		{
			Group Group = context.Groups.Find(GroupID);
			context.Groups.Remove(Group);
			Save();
		}

		public void UpdateGroup(Group Group)
		{
			context.Entry(Group).State = EntityState.Modified;
			Save();
		}

		public void InsertGroupMember(GroupMember GroupMember)
		{
			GroupMember.CreatedTime = DateTime.UtcNow;
			context.GroupMembers.Add(GroupMember);
			Save();
		}

		public void DeleteGroupMember(long UserId,long GroupId)
		{
			GroupMember GroupMember = context.GroupMembers.Where(x=>x.UserId == UserId  && x.GroupID == GroupId).FirstOrDefault();
			context.GroupMembers.Remove(GroupMember);
			Save();
		}

		public void UpdateGroupMember(GroupMember GroupMember)
		{
			context.Entry(GroupMember).State = EntityState.Modified;
			Save();
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