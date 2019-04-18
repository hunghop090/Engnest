using Engnest.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  IUserRepository : IDisposable
	{
		IEnumerable<User> GetUsers();
        User GetUserByID(int UserId);
        void InsertUser(User User);
        void DeleteUser(int UserID);
        void UpdateUser(User User);
        void Save();
		Byte Login(string UserName,string Password);
	}
}