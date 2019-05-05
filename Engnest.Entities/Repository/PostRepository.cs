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