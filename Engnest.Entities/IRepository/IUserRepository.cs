using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  IUserRepository : IDisposable
	{
		IEnumerable<User> GetUsers();
        User GetUserByID(long UserId);
		User GetUserByIDForUpdate(long UserId);
		Relationship GetRequestFriendByID(long id);
		Relationship GetRequestFriendByUser(long id,long userid);
		List<FriendModel> GetFriend(long UserId);

		List<FriendModel> SearchFriend(long UserId,string query);

		List<FriendModel> GetSuggestFriend(long UserId,string query,bool? location,string category);
		List<RequestFriendModel> GetRequestFriend(long UserId);
        User GetUserByName(string UserName);
        void InsertUser(User User);
        void DeleteUser(long UserID);
        void UpdateUser(User User);
		void InsertRequestFriend(Relationship Relationship);
        void DeleteRequestFriend(long id);
        void UpdateRequestFriend(Relationship Relationship);
        void Save();
		Byte Login(string UserName,string Password, out User user);
		Byte SignIn(SignInModel model);
	}
}