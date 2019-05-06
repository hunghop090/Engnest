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
	public class PostRepository : IPostRepository, IDisposable
    {
        private EngnestContext context;

        public PostRepository(EngnestContext context)
        {
            this.context = context;
        }

        public List<Post> GetPosts()
        {
            return context.Posts.ToList();
        }

		 public List<Post> LoadPostsHome(string date,long UserId)
        {
			DateTime createDate = DateTime.Now;
			if(!string.IsNullOrEmpty(date))
				createDate = DateTime.Parse(date);
			var result = (from c in context.Posts
				join p1 in context.Relationships on UserId equals p1.UserReceiveID  into ps1
				from p1 in ps1.DefaultIfEmpty()
				join p2 in context.Relationships on UserId equals p2.UserSentID  into ps2
				from p2 in ps2.DefaultIfEmpty()
				join p3 in context.GroupMembers on UserId equals p3.UserID  into ps3
				from p3 in ps3.DefaultIfEmpty()
				where c.TargetId == p1.UserSentID || c.TargetId == p2.UserReceiveID ||  c.TargetId == p3.GroupID
				&& c.CreatedTime < createDate
				orderby c.CreatedTime
				select c).Take(10).ToList();
            return result;
        }

        public Post GetPostByID(long id)
        {
            return context.Posts.Find(id);
        }

        public List<Post> GetPostByTargetId(long id)
        {
            return context.Posts.Where(x => x.TargetId == id).ToList();
        }

        public void InsertPost(Post Post)
        {
            Post.CreatedTime = DateTime.Now;
            context.Posts.Add(Post);
            Save();
        }

        public void DeletePost(long PostID)
        {
            Post Post = context.Posts.Find(PostID);
            context.Posts.Remove(Post);
        }

        public void UpdatePost(Post Post)
        {
            context.Entry(Post).State = EntityState.Modified;
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