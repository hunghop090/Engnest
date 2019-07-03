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
	public class UserRepository : IUserRepository, IDisposable
	{
		private EngnestContext context;

		public UserRepository(EngnestContext context)
		{
			this.context = context;
		}

		public IEnumerable<User> GetUsers()
		{
			return context.Users.ToList();
		}

		public User GetUserByID(long id)
		{
			var result = context.Users.Find(id);
			var respone = AmazonS3Uploader.GetUrl(result.Avatar,0);
			if(!string.IsNullOrEmpty(respone))
				result.Avatar = respone;
			respone = AmazonS3Uploader.GetUrl(result.BackGround,0);
			if(!string.IsNullOrEmpty(respone))
				result.BackGround = respone;
			return result;
		}

		public User GetUserByIDForUpdate(long id)
		{
			var result = context.Users.Find(id);
			return result;
		}

		public User GetUserByName(string UserName)
		{
			var result = context.Users.Where(x => x.UserName == UserName).FirstOrDefault();
			var respone = AmazonS3Uploader.GetUrl(result.Avatar,0);
			if(!string.IsNullOrEmpty(respone))
				result.Avatar = respone;
			respone = AmazonS3Uploader.GetUrl(result.BackGround,0);
			if(!string.IsNullOrEmpty(respone))
				result.BackGround = respone;
			return result;
		}

		public List<FriendModel> GetFriend(long UserId)
		{
			var ListFriend = new List<FriendModel>();
			var result = (from c in context.Relationships
						  join p1 in context.Users on c.UserReceiveID equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  join p2 in context.Users on c.UserSentID equals p2.ID into ps2
						  from p2 in ps2.DefaultIfEmpty()
						  where c.UserReceiveID == UserId || c.UserSentID == UserId
						  orderby c.CreatedTime descending
						  select new { c, p1, p2 }).ToList();
			foreach (var item in result)
			{
				var Friend = new FriendModel();
				if (item.p1.ID == UserId)
				{
					Friend.Avatar = item.p2.Avatar;
					Friend.NickName = item.p2.NickName;
					Friend.Id = item.p2.ID;
					Friend.CreatedTime = item.c.CreatedTime;
					var respone = AmazonS3Uploader.GetUrl(item.p2.Avatar,0);
					if(!string.IsNullOrEmpty(respone))
						Friend.Avatar = respone;
				}
				else
				{
					Friend.Avatar = item.p1.Avatar;
					Friend.NickName = item.p1.NickName;
					Friend.Id = item.p1.ID;
					Friend.CreatedTime = item.c.CreatedTime;
					var respone = AmazonS3Uploader.GetUrl(item.p1.Avatar,0);
					if(!string.IsNullOrEmpty(respone))
						Friend.Avatar = respone;
				}
				ListFriend.Add(Friend);
			}
			return ListFriend;
		}

		public List<RequestFriendModel> GetRequestFriend(long UserId)
		{
			var ListRequestFriend = new List<RequestFriendModel>();
			var result = (from c in context.Relationships
						  join p1 in context.Users on c.UserSentID equals p1.ID into ps1
						  from p1 in ps1.DefaultIfEmpty()
						  join p2 in context.Groups on c.UserSentID equals p2.ID into ps2
						  from p2 in ps2.DefaultIfEmpty()
						  where c.UserReceiveID == UserId && c.Status == StatusRequestFriend.SENDING
						  orderby c.CreatedTime descending
						  select new RequestFriendModel
						  {
							  TargetId = p1 != null ? p1.ID : p2.ID,
							  Avatar = p1 != null ? p1.Avatar : p2.Avatar,
							  NickName = p1 != null ? p1.NickName : p2.GroupName,
							  Type = p1 != null ? TypeRequestFriend.USER : TypeRequestFriend.GROUP,
							  Id = c.ID
						  }).ToList();
			foreach(var item in result)
			{
				var respone = AmazonS3Uploader.GetUrl(item.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					item.Avatar = respone;
			}
			return result;
		}

		public void InsertUser(User User)
		{
			context.Users.Add(User);
			Save();
		}

		public void DeleteUser(long UserID)
		{
			User User = context.Users.Find(UserID);
			context.Users.Remove(User);
		}

		public void UpdateUser(User User)
		{
			context.Entry(User).State = EntityState.Modified;
			Save();
		}

		public Byte Login(string UserName, string Password, out User user)
		{
			Byte result = 0;
			user = context.Users.SingleOrDefault(x => x.UserName == UserName);
			if (user == null)
			{
				result = LoginStatus.NOT_EXISTS_USER;
			}
			else
			{
				if (user.Flag == false)
				{
					result = LoginStatus.BLOCKED;
				}
				else if (user.Password.Trim() != Password)
				{
					result = LoginStatus.WRONG_PASSWORD;
				}
				else
				{
					result = LoginStatus.SUCCESS;
				}
			}
			return result;
		}

		public Byte SignIn(SignInModel model)
		{
			Byte result = 0;
			var listuser = context.Users.Where(x => x.UserName == model.UserName || x.Email == model.Email).ToList();
			if (listuser.Count > 0)
			{
				result = SignInStatus.EXISTS_USER;
			}
			else
			{
				try
				{
					var user = new User();
					user.UserName = model.UserName;
					user.NickName = model.UserName;
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.Email = model.Email;
					user.Avatar = "image/default-avatar.jpg";
					user.BackGround = "image/default-backgroud.jpg";
					user.Password = model.Password;
					user.CreatedTime = DateTime.UtcNow;
					context.Users.Add(user);
					Save();
				}
				catch (Exception ex)
				{
					result = SignInStatus.ERROR;
				}

			}
			return result;
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