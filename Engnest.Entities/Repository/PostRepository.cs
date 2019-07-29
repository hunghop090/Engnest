using AutoMapper;
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

		 public List<PostViewModel> LoadPostsHome(string date,long UserId)
        {
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var result = (from c in context.Posts
				join p1 in context.Relationships on new {t1 = UserId ,t2 = c.TargetId.Value } equals new {t1 = p1.UserSentID ,t2 =  p1.UserReceiveID }  into ps1
				from p1 in ps1.DefaultIfEmpty()
				join p2 in context.Relationships on new {t1 = UserId ,t2 = c.TargetId.Value } equals new {t1 = p2.UserReceiveID ,t2 =  p2.UserSentID }  into ps2
				from p2 in ps2.DefaultIfEmpty()
				join p3 in context.GroupMembers on new {t1 = UserId ,t2 = c.TargetId.Value } equals new {t1 = p3.UserId ,t2 =  p3.GroupID }  into ps3
				from p3 in ps3.DefaultIfEmpty()
				join p4 in context.Users on c.UserId equals p4.ID  into ps4
				from p4 in ps4.DefaultIfEmpty()
				join p5 in context.Emotions on new {t1 = UserId ,t2 = c.ID } equals new {t1 = p5.UserId.Value,t2 = p5.TargetId.Value}  into ps5
				from p5 in ps5.DefaultIfEmpty()
				join p6 in context.Groups on new {t1 = c.TargetType.Value ,t2 = c.TargetId.Value } equals new {t1 = TypePost.GROUP,t2 = p6.ID}  into ps6
				from p6 in ps6.DefaultIfEmpty()
				join p7 in context.Users on new {t1 = c.TargetType.Value ,t2 = c.TargetId.Value } equals new {t1 = TypePost.USER,t2 = p7.ID}  into ps7
				from p7 in ps7.DefaultIfEmpty()
				let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
				let countComments = (from C in context.Comments where C.TargetId == c.ID select C).Count()
				where c.CreatedTime < createDate && (((c.TargetId == p1.UserReceiveID || c.TargetId == UserId || c.TargetId == p2.UserSentID) && c.TargetType == TypePost.USER) || ( c.TargetId == p3.GroupID && c.TargetType == TypePost.GROUP))
				orderby c.CreatedTime descending
				select new{c,p1,p2,p3,p4,countEmotions,p5,countComments,p6,p7 }).Take(10).ToList();
			var PostView = new List<PostViewModel>();
			foreach(var item in result)
			{
				if(createDate == item.c.CreatedTime)
					continue;
				var Post = new PostViewModel();
				Post.Id = item.c.ID ;
				Post.Audios = item.c.Audios ;
				Post.Content = item.c.Content ;
				Post.Images = item.c.Images ;
				if(!string.IsNullOrEmpty(item.c.Images))
				{
					var data = item.c.Images.Split(',');
					Post.ListImages = new List<string>();
					foreach(string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if(!string.IsNullOrEmpty(respone))
							Post.ListImages.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(item.c.Videos))
				{
					var data = item.c.Videos.Split(',');
					Post.ListVideos = new List<string>();
					foreach(string video in data)
					{
						var respone = AmazonS3Uploader.GetUrl(video);
						if(!string.IsNullOrEmpty(respone))
							Post.ListVideos.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(item.c.Audios))
				{
					var data = item.c.Audios.Split(',');
					Post.ListAudios = new List<string>();
					foreach(string audio in data)
					{
						var respone = AmazonS3Uploader.GetUrl(audio);
						if(!string.IsNullOrEmpty(respone))
							Post.ListAudios.Add(respone);
					}
				}
				Post.Tags = item.c.Tags ;
				Post.TagsUser = item.c.TagsUser ;
				Post.TargetId = item.c.TargetId ;
				Post.TargetType = item.c.TargetType ;
				Post.Type = item.c.Type ;
				Post.CreatedTime = item.c.CreatedTime;
				Post.UpdateTime = item.c.UpdateTime ;
				Post.UserId = item.c.UserId ;
				var responeImage = AmazonS3Uploader.GetUrl(item.p4?.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Post.Avatar = responeImage;
				responeImage = AmazonS3Uploader.GetUrl(item.p6 == null ? item.p7.Avatar :  item.p6.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Post.TargetAvatar = responeImage;
				Post.NickName = item.p4?.NickName ;
				Post.TargetNickName = item.p6 == null ? item.p7.NickName :  item.p6.GroupName ;
				Post.CountEmotions = item.countEmotions ;
				Post.CountComments = item.countComments;
				Post.StatusEmotion = item.p5 == null ? (byte)0: item.p5.Status;
				Post.ShowTarget = Post.UserId != Post.TargetId || item.c.TargetType != TypePost.USER ? "" : "hidden";
				PostView.Add(Post);
			}
            return PostView;
        }

		public PostViewModel LoadPostById(long? id,long? CommentId,long UserId)
        {
			var Post = new PostViewModel();
			if(id == null && CommentId != null)
			{
				var checkLoad = false;
				var index = 0;
				while (!checkLoad && index <100)
				{
					var comment = context.Comments.Find(CommentId.Value);
					if(comment.TargetType == TypeComment.COMMENT)
					{
						CommentId=comment.TargetId;
					}
					else
					{
						checkLoad = true;
						id=comment.TargetId;
					}
					index ++;
				}
			}
			if(id != null)
			{
				var result = (from c in context.Posts
					join p4 in context.Users on c.UserId equals p4.ID  into ps4
					from p4 in ps4.DefaultIfEmpty()
					join p5 in context.Emotions on new {t1 = UserId ,t2 = c.ID } equals new {t1 = p5.UserId.Value,t2 = p5.TargetId.Value}  into ps5
					from p5 in ps5.DefaultIfEmpty()
					join p6 in context.Groups on new {t1 = c.TargetType.Value ,t2 = c.TargetId.Value } equals new {t1 = TypePost.GROUP,t2 = p6.ID}  into ps6
					from p6 in ps6.DefaultIfEmpty()
					join p7 in context.Users on new {t1 = c.TargetType.Value ,t2 = c.TargetId.Value } equals new {t1 = TypePost.USER,t2 = p7.ID}  into ps7
					from p7 in ps7.DefaultIfEmpty()
					let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
					let countComments = (from C in context.Comments where C.TargetId == c.ID select C).Count()
					where c.ID ==id.Value
					select new{c,p4,countEmotions,p5,countComments,p6,p7 }).FirstOrDefault();

				Post.Id = result.c.ID ;
				Post.Audios = result.c.Audios ;
				Post.Content = result.c.Content ;
				Post.Images = result.c.Images ;
				if(!string.IsNullOrEmpty(result.c.Images))
				{
					var data = result.c.Images.Split(',');
					Post.ListImages = new List<string>();
					foreach(string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if(!string.IsNullOrEmpty(respone))
							Post.ListImages.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(result.c.Videos))
				{
					var data = result.c.Videos.Split(',');
					Post.ListVideos = new List<string>();
					foreach(string video in data)
					{
						var respone = AmazonS3Uploader.GetUrl(video);
						if(!string.IsNullOrEmpty(respone))
							Post.ListVideos.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(result.c.Audios))
				{
					var data = result.c.Audios.Split(',');
					Post.ListAudios = new List<string>();
					foreach(string audio in data)
					{
						var respone = AmazonS3Uploader.GetUrl(audio);
						if(!string.IsNullOrEmpty(respone))
							Post.ListAudios.Add(respone);
					}
				}
				Post.Tags = result.c.Tags ;
				Post.TagsUser = result.c.TagsUser ;
				Post.TargetId = result.c.TargetId ;
				Post.TargetType = result.c.TargetType ;
				Post.Type = result.c.Type ;
				Post.CreatedTime = result.c.CreatedTime;
				Post.UpdateTime = result.c.UpdateTime ;
				Post.UserId = result.c.UserId ;
				var responeImage = AmazonS3Uploader.GetUrl(result.p4?.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Post.Avatar = responeImage;
				responeImage = AmazonS3Uploader.GetUrl(result.p6 == null ? result.p7.Avatar :  result.p6.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Post.TargetAvatar = responeImage;
				Post.NickName = result.p4?.NickName ;
				Post.TargetNickName = result.p6 == null ? result.p7.NickName :  result.p6.GroupName ;
				Post.CountEmotions = result.countEmotions ;
				Post.CountComments = result.countComments;
				Post.StatusEmotion = result.p5 == null ? (byte)0: result.p5.Status;
				Post.ShowTarget = Post.UserId != Post.TargetId && result.c.TargetType == TypePost.USER ? "" : "hidden";
			}
            return Post;
        }

		public List<PostViewModel> LoadPostsProfile(string date,long UserId,long UserLogin)
        {
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var result = (from c in context.Posts
				join p4 in context.Users on c.UserId equals p4.ID  into ps4
				from p4 in ps4.DefaultIfEmpty()
				join p5 in context.Emotions on new {t1 = UserLogin ,t2 = c.ID } equals new {t1 = p5.UserId.Value,t2 = p5.TargetId.Value}  into ps5
				from p5 in ps5.DefaultIfEmpty()
				join p6 in context.Users on c.TargetId equals p6.ID  into ps6
				from p6 in ps6.DefaultIfEmpty()
				let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
				let countComments = (from C in context.Comments where C.TargetId == c.ID select C).Count()
				where c.CreatedTime < createDate && (c.TargetId != null && c.TargetId == UserId ) && c.TargetType == TypePost.USER
				orderby c.CreatedTime descending
				select new{c,p4,countEmotions,p5,countComments,p6 }).Take(10).ToList();
			var PostView = new List<PostViewModel>();

			foreach(var item in result)
			{
				if(createDate == item.c.CreatedTime)
					continue;
				var Post = new PostViewModel();
				Post.Id = item.c.ID ;
				Post.Audios = item.c.Audios ;
				Post.Content = item.c.Content ;
				Post.Images = item.c.Images ;
				if(!string.IsNullOrEmpty(item.c.Images))
				{
					var data = item.c.Images.Split(',');
					Post.ListImages = new List<string>();
					foreach(string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if(!string.IsNullOrEmpty(respone))
							Post.ListImages.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(item.c.Videos))
				{
					var data = item.c.Videos.Split(',');
					Post.ListVideos = new List<string>();
					foreach(string video in data)
					{
						var respone = AmazonS3Uploader.GetUrl(video);
						if(!string.IsNullOrEmpty(respone))
							Post.ListVideos.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(item.c.Audios))
				{
					var data = item.c.Audios.Split(',');
					Post.ListAudios = new List<string>();
					foreach(string audio in data)
					{
						var respone = AmazonS3Uploader.GetUrl(audio);
						if(!string.IsNullOrEmpty(respone))
							Post.ListAudios.Add(respone);
					}
				}
				Post.Tags = item.c.Tags ;
				Post.TagsUser = item.c.TagsUser ;
				Post.TargetId = item.c.TargetId ;
				Post.TargetType = item.c.TargetType ;
				Post.Type = item.c.Type ;
				Post.CreatedTime = item.c.CreatedTime;
				Post.UpdateTime = item.c.UpdateTime ;
				Post.UserId = item.c.UserId ;
				Post.NickName = item.p4?.NickName ;
				Post.CountEmotions = item.countEmotions ;
				Post.CountComments = item.countComments;
				Post.StatusEmotion = item.p5 == null ? (byte)0: item.p5.Status;
				var responeImage = AmazonS3Uploader.GetUrl(item.p4?.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Post.Avatar = responeImage;
				responeImage = AmazonS3Uploader.GetUrl(item.p6?.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Post.TargetAvatar = responeImage;
				Post.TargetNickName = item.p6?.NickName;
				Post.ShowTarget = Post.UserId == Post.TargetId ? "hidden" : "";
				PostView.Add(Post);
			}
            return PostView;
        }

		public List<PostViewModel> GetListImage(long Id)
        {
			var result = (from c in context.Posts
				where (c.TargetId != null && c.TargetId == Id ) && c.TargetType == TypePost.USER && !string.IsNullOrEmpty(c.Images)
				orderby c.CreatedTime descending
				select new{c}).Take(6).ToList();
			var PostView = new List<PostViewModel>();

			foreach(var item in result)
			{
				var Post = new PostViewModel();
				Post.Id = item.c.ID ;
				Post.Images = item.c.Images ;
				if(!string.IsNullOrEmpty(item.c.Images))
				{
					var data = item.c.Images.Split(',');
					Post.ListImages = new List<string>();
					foreach(string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if(!string.IsNullOrEmpty(respone))
							Post.ListImages.Add(respone);
					}
				}
				PostView.Add(Post);
			}
            return PostView;
        }

		public List<PostViewModel> LoadPostsGroup(string date,long UserId)
        {
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var result = (from c in context.Posts
				join p4 in context.Users on c.UserId equals p4.ID  into ps4
				from p4 in ps4.DefaultIfEmpty()
				join p5 in context.Emotions on new {t1 = UserId ,t2 = c.ID } equals new {t1 = p5.UserId.Value,t2 = p5.TargetId.Value}  into ps5
				from p5 in ps5.DefaultIfEmpty()
				let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
				let countComments = (from C in context.Comments where C.TargetId == c.ID select C).Count()
				where c.CreatedTime < createDate && (c.TargetId != null && c.TargetId == UserId ) && c.TargetType == TypePost.GROUP
				orderby c.CreatedTime descending
				select new{c,p4,countEmotions,p5,countComments }).Take(10).ToList();
			var PostView = new List<PostViewModel>();

			foreach(var item in result)
			{
				if(createDate == item.c.CreatedTime)
					continue;
				var Post = new PostViewModel();
				Post.Id = item.c.ID ;
				Post.Audios = item.c.Audios ;
				Post.Content = item.c.Content ;
				Post.Images = item.c.Images ;
				if(!string.IsNullOrEmpty(item.c.Images))
				{
					var data = item.c.Images.Split(',');
					Post.ListImages = new List<string>();
					foreach(string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if(!string.IsNullOrEmpty(respone))
							Post.ListImages.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(item.c.Videos))
				{
					var data = item.c.Videos.Split(',');
					Post.ListVideos = new List<string>();
					foreach(string video in data)
					{
						var respone = AmazonS3Uploader.GetUrl(video);
						if(!string.IsNullOrEmpty(respone))
							Post.ListVideos.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(item.c.Audios))
				{
					var data = item.c.Audios.Split(',');
					Post.ListAudios = new List<string>();
					foreach(string audio in data)
					{
						var respone = AmazonS3Uploader.GetUrl(audio);
						if(!string.IsNullOrEmpty(respone))
							Post.ListAudios.Add(respone);
					}
				}
				Post.Tags = item.c.Tags ;
				Post.TagsUser = item.c.TagsUser ;
				Post.TargetId = item.c.TargetId ;
				Post.TargetType = item.c.TargetType ;
				Post.Type = item.c.Type ;
				Post.CreatedTime = item.c.CreatedTime;
				Post.UpdateTime = item.c.UpdateTime ;
				Post.UserId = item.c.UserId ;
				Post.NickName = item.p4?.NickName ;
				var responeImage = AmazonS3Uploader.GetUrl(item.p4?.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Post.Avatar = responeImage;
				Post.CountEmotions = item.countEmotions ;
				Post.CountComments = item.countComments;
				Post.StatusEmotion = item.p5 == null ? (byte)0: item.p5.Status;
				Post.ShowTarget = "hidden";
				PostView.Add(Post);
			}
            return PostView;
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
            Post.CreatedTime = DateTime.UtcNow;
            context.Posts.Add(Post);
            Save();
        }

        public void DeletePost(long PostID)
        {
            Post Post = context.Posts.Find(PostID);
            context.Posts.Remove(Post);
			Save();
        }

        public void UpdatePost(Post Post)
        {
            context.Entry(Post).State = EntityState.Modified;
			Save();
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