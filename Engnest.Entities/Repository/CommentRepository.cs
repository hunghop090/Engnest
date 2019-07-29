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
	public class CommentRepository : ICommentRepository, IDisposable
	{
		private EngnestContext context;

		public CommentRepository(EngnestContext context)
		{
			this.context = context;
		}

		public List<Comment> GetComments()
		{
			return context.Comments.ToList();
		}

		public List<CommentViewModel> LoadCommentsPost(string PostIds, string date, int quantity, long LoginUser)
		{
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var CommentView = new List<CommentViewModel>();
			if (String.IsNullOrEmpty(PostIds))
				return CommentView;
			List<string> LPostsId = PostIds.Split(',').ToList();
			for (var i = 0; i < LPostsId.Count; i++)
			{
				var resultMost = new List<CommentViewModel>();
				var result = new List<CommentViewModel>();
				var PostId = Int64.Parse(LPostsId[i]);
				if (string.IsNullOrEmpty(date))
				{
					resultMost = (from c in context.Comments
								  join p4 in context.Users on c.UserId equals p4.ID into ps4
								  from p4 in ps4.DefaultIfEmpty()
								  join p5 in context.Comments on new { a1 = c.ID, a2 = TypeComment.COMMENT } equals new { a1 = p5.TargetId, a2 = p5.TargetType.Value } into ps5
								  join p6 in context.Emotions on new { t1 = LoginUser, t2 = c.ID } equals new { t1 = p6.UserId.Value, t2 = p6.TargetId.Value } into ps6
								  from p6 in ps6.DefaultIfEmpty()
								  let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
								  where PostId == c.TargetId && c.CreatedTime < createDate && c.TargetType == TypeComment.POST && countEmotions > 100
								  orderby countEmotions ascending
								  select new CommentViewModel
								  {
									  Id = c.ID,
									  Audios = c.Audios,
									  TargetId = c.TargetId,
									  CreatedTime = c.CreatedTime,
									  Images = c.Images,
									  Tags = c.Tags,
									  TagsUser = c.TagsUser,
									  Content = c.Content,
									  UserId = c.UserId,
									  Avatar = p4.Avatar,
									  NickName = p4.NickName,
									  CountReply = ps5.Count(),
									  CountEmotions = countEmotions,
									  StatusEmotion = p6 == null ? (byte)0 : p6.Status
								  }).Take(quantity + 1).ToList();
				}
				if (resultMost.Count == 0)
				{
					result = (from c in context.Comments
							  join p4 in context.Users on c.UserId equals p4.ID into ps4
							  from p4 in ps4.DefaultIfEmpty()
							  join p5 in context.Comments on new { a1 = c.ID, a2 = TypeComment.COMMENT } equals new { a1 = p5.TargetId, a2 = p5.TargetType.Value } into ps5
							  join p6 in context.Emotions on new { t1 = LoginUser, t2 = c.ID } equals new { t1 = p6.UserId.Value, t2 = p6.TargetId.Value } into ps6
							  from p6 in ps6.DefaultIfEmpty()
							  let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
							  where PostId == c.TargetId && c.CreatedTime < createDate && c.TargetType == TypeComment.POST
							  orderby c.CreatedTime descending
							  select new CommentViewModel
							  {
								  Id = c.ID,
								  Audios = c.Audios,
								  TargetId = c.TargetId,
								  CreatedTime = c.CreatedTime,
								  Images = c.Images,
								  Tags = c.Tags,
								  TagsUser = c.TagsUser,
								  Content = c.Content,
								  UserId = c.UserId,
								  Avatar = p4.Avatar,
								  NickName = p4.NickName,
								  CountReply = ps5.Count(),
								  CountEmotions = countEmotions,
								  StatusEmotion = p6 == null ? (byte)0 : p6.Status
							  }).Take(quantity + 1).ToList();
					if (result.Count > quantity)
					{
						result[result.Count - 2].MoreComment = true;
						result.RemoveAt(result.Count - 1);
					}
				}
				else
				{
					if (result.Count > quantity)
					{
						result[result.Count - 2].MoreComment = true;
						result.RemoveAt(result.Count - 1);
					}
					else
					{
						result[result.Count - 1].MoreComment = true;
					}
				}
				CommentView = CommentView.Union(result).ToList();
			}
			foreach (var item in CommentView)
			{
				var responeImage = AmazonS3Uploader.GetUrl(item.Avatar, 0);
				if (!string.IsNullOrEmpty(responeImage))
					item.Avatar = responeImage;
				if (!string.IsNullOrEmpty(item.Images))
				{
					var data = item.Images.Split(',');
					item.ListImages = new List<string>();
					foreach (string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if (!string.IsNullOrEmpty(respone))
							item.ListImages.Add(respone);
					}
				}
				if (!string.IsNullOrEmpty(item.Audios))
				{
					var data = item.Audios.Split(',');
					item.ListAudios = new List<string>();
					foreach (string audio in data)
					{
						var respone = AmazonS3Uploader.GetUrl(audio);
						if (!string.IsNullOrEmpty(respone))
							item.ListAudios.Add(respone);
					}
				}
			}
			return CommentView;
		}

		public List<CommentViewModel> LoadCommentsReply(string CommentId, string date, int quantity, string createdUser, long LoginUser)
		{
			long UserId = 0;
			if (!string.IsNullOrEmpty(createdUser))
				UserId = long.Parse(createdUser);
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var CommentView = new List<CommentViewModel>();
			if (String.IsNullOrEmpty(CommentId))
				return CommentView;

			var CommentIdI = Int64.Parse(CommentId);
			var result = (from c in context.Comments
						  join p4 in context.Users on c.UserId equals p4.ID into ps4
						  from p4 in ps4.DefaultIfEmpty()
						  join p5 in context.Comments on new { a1 = c.ID, a2 = TypeComment.COMMENT } equals new { a1 = p5.TargetId, a2 = p5.TargetType.Value } into ps5
						  join p6 in context.Emotions on new { t1 = LoginUser, t2 = c.ID } equals new { t1 = p6.UserId.Value, t2 = p6.TargetId.Value } into ps6
						  from p6 in ps6.DefaultIfEmpty()
						  let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
						  where CommentIdI == c.TargetId && c.CreatedTime < createDate && (UserId == 0 || UserId == c.UserId) && c.TargetType == TypeComment.COMMENT
						  orderby c.CreatedTime descending
						  select new CommentViewModel
						  {
							  Id = c.ID,
							  Audios = c.Audios,
							  TargetId = c.TargetId,
							  CreatedTime = c.CreatedTime,
							  Images = c.Images,
							  Tags = c.Tags,
							  TagsUser = c.TagsUser,
							  Content = c.Content,
							  UserId = c.UserId,
							  Avatar = p4.Avatar,
							  NickName = p4.NickName,
							  CountReply = ps5.Count(),
							  CountEmotions = countEmotions,
							  StatusEmotion = p6 == null ? (byte)0 : p6.Status
						  }).Take(quantity + 1).ToList();
			if (result.Count > quantity)
			{
				result[result.Count - 2].MoreComment = true;
				result.RemoveAt(result.Count - 1);
			}
			CommentView = CommentView.Union(result).ToList();

			foreach (var item in CommentView)
			{
				var responeImage = AmazonS3Uploader.GetUrl(item.Avatar, 0);
				if (!string.IsNullOrEmpty(responeImage))
					item.Avatar = responeImage;
				if (!string.IsNullOrEmpty(item.Images))
				{
					var data = item.Images.Split(',');
					item.ListImages = new List<string>();
					foreach (string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if (!string.IsNullOrEmpty(respone))
							item.ListImages.Add(respone);
					}
				}
				if (!string.IsNullOrEmpty(item.Audios))
				{
					var data = item.Audios.Split(',');
					item.ListAudios = new List<string>();
					foreach (string audio in data)
					{
						var respone = AmazonS3Uploader.GetUrl(audio);
						if (!string.IsNullOrEmpty(respone))
							item.ListAudios.Add(respone);
					}
				}
			}
			return CommentView;
		}
		public Comment GetCommentByUpdate(long id)
		{
			return context.Comments.Find(id);
		}
		public CommentViewModel GetCommentByID(long id, long LoginUser)
		{

			var CommentView = new CommentViewModel();
			var result = (from c in context.Comments
						  join p4 in context.Users on c.UserId equals p4.ID into ps4
						  from p4 in ps4.DefaultIfEmpty()
						  join p5 in context.Comments on new { a1 = c.ID, a2 = TypeComment.COMMENT } equals new { a1 = p5.TargetId, a2 = p5.TargetType.Value } into ps5
						  join p6 in context.Emotions on new { t1 = LoginUser, t2 = c.ID } equals new { t1 = p6.UserId.Value, t2 = p6.TargetId.Value } into ps6
						  from p6 in ps6.DefaultIfEmpty()
						  let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
						  where id == c.ID
						  orderby c.CreatedTime descending
						  select new CommentViewModel
						  {
							  Id = c.ID,
							  Audios = c.Audios,
							  TargetId = c.TargetId,
							  CreatedTime = c.CreatedTime,
							  Images = c.Images,
							  Tags = c.Tags,
							  TagsUser = c.TagsUser,
							  Content = c.Content,
							  UserId = c.UserId,
							  Avatar = p4.Avatar,
							  NickName = p4.NickName,
							  CountReply = ps5.Count(),
							  CountEmotions = countEmotions,
							  StatusEmotion = p6 == null ? (byte)0 : p6.Status
						  }).FirstOrDefault();
			CommentView = result;
			var responeImage = AmazonS3Uploader.GetUrl(CommentView.Avatar, 0);
			if (!string.IsNullOrEmpty(responeImage))
				CommentView.Avatar = responeImage;
			if (!string.IsNullOrEmpty(CommentView.Images))
			{
				var data = CommentView.Images.Split(',');
				CommentView.ListImages = new List<string>();
				foreach (string image in data)
				{
					var respone = AmazonS3Uploader.GetUrl(image);
					if (!string.IsNullOrEmpty(respone))
						CommentView.ListImages.Add(respone);
				}
			}
			if (!string.IsNullOrEmpty(CommentView.Audios))
			{
				var data = CommentView.Audios.Split(',');
				CommentView.ListAudios = new List<string>();
				foreach (string audio in data)
				{
					var respone = AmazonS3Uploader.GetUrl(audio);
					if (!string.IsNullOrEmpty(respone))
						CommentView.ListAudios.Add(respone);
				}
			}
			return CommentView;
		}

		public List<Comment> GetCommentByTargetId(long id)
		{
			//         DateTime createDate = DateTime.UtcNow;
			//if (!string.IsNullOrEmpty(date))
			//{
			//	createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
			//	createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			//}
			//var result = (from c in context.Comments
			//	join p1 in context.Relationships on UserId equals p1.UserReceiveID  into ps1
			//	from p1 in ps1.DefaultIfEmpty()
			//	join p2 in context.Relationships on UserId equals p2.UserSentID  into ps2
			//	from p2 in ps2.DefaultIfEmpty()
			//	join p3 in context.GroupMembers on UserId equals p3.UserID  into ps3
			//	from p3 in ps3.DefaultIfEmpty()
			//	join p4 in context.Users on c.UserId equals p4.ID  into ps4
			//	from p4 in ps4.DefaultIfEmpty()
			//	join p5 in context.Emotions on new {t1 = UserId ,t2 = c.ID } equals new {t1 = p5.UserId.Value,t2 = p5.TargetId.Value}  into ps5
			//	from p5 in ps5.DefaultIfEmpty()
			//	let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
			//	where c.CreatedTime < createDate && (c.TargetId == p1.UserSentID || c.TargetId == p2.UserReceiveID ||  c.TargetId == p3.GroupID)
			//	orderby c.CreatedTime descending
			//	select new{c,p4,countEmotions,p5 }).Take(10).ToList();
			//var CommentView = new List<CommentViewModel>();
			//foreach(var item in result)
			//{
			//	var Comment = new CommentViewModel();
			//	Comment.Id = item.c.ID ;
			//	Comment.Audios = item.c.Audios ;
			//	Comment.Content = item.c.Content ;
			//	Comment.Images = item.c.Images ;
			//	Comment.Tags = item.c.Tags ;
			//	Comment.TagsUser = item.c.TagsUser ;
			//	Comment.TargetId = item.c.TargetId ;
			//	Comment.TargetType = item.c.TargetType ;
			//	Comment.Type = item.c.Type ;
			//	Comment.CreatedTime = item.c.CreatedTime;
			//	Comment.UpdateTime = item.c.UpdateTime ;
			//	Comment.UserId = item.c.UserId ;
			//	Comment.Avatar = item.p4?.Avatar ;
			//	Comment.NickName = item.p4?.NickName ;
			//	Comment.CountEmotions = item.countEmotions ;
			//	Comment.StatusEmotion = item.p5 == null ? (byte)0: item.p5.Status;
			//	var Comments = (from com in context.Comments where com.TargetId == Comment.Id orderby com.CreatedTime descending select com).Take(3).ToList();
			//	if(Comments != null)
			//		Comment.ListComments = Mapper.Map<List<CommentViewModel>>(Comments);
			//	CommentView.Add(Comment);
			//}
			return new List<Comment>();
		}

		public long InsertComment(Comment Comment)
		{
			Comment.CreatedTime = DateTime.UtcNow;
			var a = context.Comments.Add(Comment);
			Save();
			return a.ID;
		}

		public void DeleteComment(long CommentID)
		{
			Comment Comment = context.Comments.Find(CommentID);
			context.Comments.Remove(Comment);
			Save();
		}

		public void UpdateComment(Comment Comment)
		{
			context.Entry(Comment).State = EntityState.Modified;
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