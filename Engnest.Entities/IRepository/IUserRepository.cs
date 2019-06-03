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
		List<FriendModel> GetFriend(long UserId);
        User GetUserByName(string UserName);
        void InsertUser(User User);
        void DeleteUser(long UserID);
        void UpdateUser(User User);
        void Save();
		Byte Login(string UserName,string Password, out User user);
		Byte SignIn(SignInModel model);
	}
}