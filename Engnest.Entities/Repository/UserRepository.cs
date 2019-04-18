using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
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

        public User GetUserByID(int id)
        {
            return context.Users.Find(id);
        }

        public void InsertUser(User User)
        {
            context.Users.Add(User);
        }

        public void DeleteUser(int UserID)
        {
            User User = context.Users.Find(UserID);
            context.Users.Remove(User);
        }

        public void UpdateUser(User User)
        {
            context.Entry(User).State = EntityState.Modified;
        }

		 public Byte Login(string UserName,string Password)
        {
			Byte result = 0;
			var user = context.Users.SingleOrDefault(x => x.UserName == UserName);
			if(user == null)
			{
				result = LoginStatus.NOT_EXISTS_USER;
			}
			else
			{
				if(user.Flag == false)
				{
					result = LoginStatus.BLOCKED;
				}
				else if(user.Password != Password)
				{
					result = LoginStatus.WRONG_PASSWORD;
				} else
				{
					result = LoginStatus.SUCCESS;
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