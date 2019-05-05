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
            return context.Users.Find(id);
        }

        public User GetUserByName(string UserName)
        {
            return context.Users.Where(x => x.UserName == UserName).FirstOrDefault();
        }

        public void InsertUser(User User)
        {
            context.Users.Add(User);
        }

        public void DeleteUser(long UserID)
        {
            User User = context.Users.Find(UserID);
            context.Users.Remove(User);
        }

        public void UpdateUser(User User)
        {
            context.Entry(User).State = EntityState.Modified;
        }

		public Byte Login(string UserName,string Password, out User user)
        {
			Byte result = 0;
			user = context.Users.SingleOrDefault(x => x.UserName == UserName);
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
				else if(user.Password.Trim() != Password)
				{
					result = LoginStatus.WRONG_PASSWORD;
				} else
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
			if(listuser.Count > 0)
			{
				result = SignInStatus.EXISTS_USER;
			}
			else
			{
				try {
				var user = new User();
				user.UserName = model.UserName;
                user.NickName = model.UserName;
                user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.Email = model.Email;
				user.Password = model.Password;
                user.CreatedTime = DateTime.Now;
				context.Users.Add(user);
				}
				catch(Exception ex)
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