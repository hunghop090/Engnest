using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  IPostRepository : IDisposable
	{
        List<Post> GetPosts();
		List<PostViewModel> LoadPostsHome(string date,long UserId);
		List<PostViewModel> LoadPostsProfile(string date,long UserId);

		List<PostViewModel> LoadPostsGroup(string date,long UserId);
        Post GetPostByID(long PostId);
        List<Post> GetPostByTargetId(long TargetId);
        void InsertPost(Post Post);
        void DeletePost(long PostID);
        void UpdatePost(Post Post);
        void Save();
	}
}