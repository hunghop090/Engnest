using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
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

		public Relationship GetRequestFriendByID(long id)
		{
			var result = context.Relationships.Find(id);
			return result;
		}

		public Relationship GetRequestFriendByUser(long id,long userid)
		{
			var result = context.Relationships.Where(x => (x.UserSentID == id && x.UserReceiveID == userid) || (x.UserSentID == userid && x.UserReceiveID == id)).FirstOrDefault();
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
						  where (c.UserReceiveID == UserId || c.UserSentID == UserId) && c.Status == StatusRequestFriend.ACCEPT
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

		public List<FriendModel> SearchFriend(long UserId,string query)
		{
			var ListFriend = new List<FriendModel>();
			var result = (from c in context.Users
						  where c.NickName.Contains(query) || c.SubName.Contains(query)
						  orderby c.NickName descending
						  select new { c }).Take(200).ToList();
			foreach (var item in result)
			{
				var Friend = new FriendModel();
				Friend.Avatar = item.c.Avatar;
				Friend.NickName = item.c.NickName;
				Friend.Id = item.c.ID;
				Friend.Type = TypeRequestFriend.USER;
				Friend.CreatedTime = item.c.CreatedTime;
				var respone = AmazonS3Uploader.GetUrl(item.c.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					Friend.Avatar = respone;
				ListFriend.Add(Friend);
			}
			return ListFriend;
		}

		public List<FriendModel> GetSuggestFriend(long UserId,string query,bool? location,string category)
		{
			var List = new List<string>();
			if(!string.IsNullOrEmpty(category))
			{
				List = category.Split(',').ToList();
			}
			var User = (from c in context.Users where c.ID == UserId select c).FirstOrDefault();
			var ListFriend = new List<FriendModel>();

			var result1 = (from c in context.Users
						from t2 in context.Relationships.Where(tt2 => (c.ID == tt2.UserReceiveID && UserId == tt2.UserSentID) || (UserId == tt2.UserReceiveID && c.ID == tt2.UserSentID)).DefaultIfEmpty()
						where (string.IsNullOrEmpty(query) || (c.NickName.Contains(query) || c.SubName.Contains(query)))
						&& (c.Lat != null && c.Lng != null)
						&& c.ID != UserId
						&& (t2.ID == 0 || t2.Status != StatusRequestFriend.ACCEPT || t2.Type != TypeRequestFriend.USER)
						orderby c.NickName descending
						select new { c,distance = (c.Lat.Value - User.Lat.Value) * (c.Lat.Value - User.Lat.Value) +  (c.Lng - User.Lng.Value) * (c.Lng.Value - User.Lng.Value)}).OrderBy(x=>x.distance).Take(10000).OrderBy(x => Guid.NewGuid()).Take(50).ToList();
			foreach (var item in result1)
			{
				var Friend = new FriendModel();
				Friend.Avatar = item.c.Avatar;
				Friend.NickName = item.c.NickName;
				Friend.Id = item.c.ID;
				Friend.Type = TypeRequestFriend.USER;
				Friend.CreatedTime = item.c.CreatedTime;
				Friend.Suggest = TypeSuggest.LOCATION;
				var respone = AmazonS3Uploader.GetUrl(item.c.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					Friend.Avatar = respone;
				ListFriend.Add(Friend);
			}

			var result2 = (from c in context.Users
							from t2 in context.Relationships.Where(tt2 => (c.ID == tt2.UserReceiveID && UserId == tt2.UserSentID) || (UserId == tt2.UserReceiveID && c.ID == tt2.UserSentID)).DefaultIfEmpty()
						where (string.IsNullOrEmpty(query) || (c.NickName.Contains(query) || c.SubName.Contains(query)))
						&& c.ID != UserId
						&& List.Any(x => c.Category.Contains(x))
						&& (t2.ID == 0 || t2.Status != StatusRequestFriend.ACCEPT || t2.Type != TypeRequestFriend.USER)
						orderby c.NickName descending
						select new { c }).OrderBy(x => Guid.NewGuid()).Take(50).ToList();
			foreach (var item in result2)
			{
				var Friend = new FriendModel();
				Friend.Avatar = item.c.Avatar;
				Friend.NickName = item.c.NickName;
				Friend.Id = item.c.ID;
				Friend.Type = TypeRequestFriend.USER;
				Friend.CreatedTime = item.c.CreatedTime;
				Friend.Suggest = TypeSuggest.CATEGORY;
				var respone = AmazonS3Uploader.GetUrl(item.c.Avatar,0);
				if(!string.IsNullOrEmpty(respone))
					Friend.Avatar = respone;
				var same = ListFriend.Where(x => x.Id == Friend.Id).ToList();
	
				if(same.Count == 0)
					ListFriend.Add(Friend);
				else
					ListFriend.Where(x => x.Id == Friend.Id).FirstOrDefault().Suggest = TypeSuggest.BOTH;
			}
			ListFriend.OrderBy(a => Guid.NewGuid()).ToList();
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
			User.CreatedTime = DateTime.UtcNow;
			context.Users.Add(User);
			Save();
		}

		public void DeleteUser(long UserID)
		{
			User User = context.Users.Find(UserID);
			context.Users.Remove(User);
			Save();
		}

		public void UpdateUser(User User)
		{
			context.Entry(User).State = EntityState.Modified;
			Save();
			if(User.Lng != null && User.Lat != null)
			context.Database.ExecuteSqlCommand("update [dbo].[User] set Lng = " + User.Lng +" , Lat = " + User.Lat +"  where ID = " + User.ID +"");
		}

		public void InsertRequestFriend(Relationship Relationship)
		{
			Relationship.CreatedTime = DateTime.UtcNow;
			context.Relationships.Add(Relationship);
			Save();
		}

		public void DeleteRequestFriend(long id)
		{
			Relationship Request = context.Relationships.Find(id);
			context.Relationships.Remove(Request);
			Save();
		}

		public void UpdateRequestFriend(Relationship Relationship)
		{
			Relationship.UpdateTime = DateTime.UtcNow;
			context.Entry(Relationship).State = EntityState.Modified;
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
					context.Database.ExecuteSqlCommand("update [dbo].[User] set Lng = " + 105.82386 +" , Lat = " + 21.027444 +"  where ID = " + user.ID +"");
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