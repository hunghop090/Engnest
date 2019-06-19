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

		 public List<CommentViewModel> LoadCommentsPost(string PostIds,string date,int quantity,string createdUser)
        {
			long UserId = 0;
			if(!string.IsNullOrEmpty(createdUser))
				UserId = long.Parse(createdUser);
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var CommentView = new List<CommentViewModel>();
			if(String.IsNullOrEmpty(PostIds))
				return CommentView;
			List<string> LPostsId = PostIds.Split(',').ToList();
			for(var i = 0; i < LPostsId.Count; i++)
			{
				var PostId = Int64.Parse(LPostsId[i]);
				var result = (from c in context.Comments
				join p4 in context.Users on c.UserId equals p4.ID  into ps4
				from p4 in ps4.DefaultIfEmpty()
				join p5 in context.Comments on new {a1 = c.ID,a2 =  TypeComment.COMMENT } equals new {a1 = p5.TargetId,a2 = p5.TargetType.Value} into ps5
				let countEmotions = (from E in context.Emotions where E.TargetId == c.ID select E).Count()
				where PostId == c.TargetId &&  c.CreatedTime < createDate && (UserId == 0 || UserId == c.UserId)
				orderby c.CreatedTime descending
				select new CommentViewModel{
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
					CountEmotions = countEmotions
					}).Take(quantity).ToList();
				CommentView = CommentView.Union(result).ToList();
			}
            return CommentView;
        }

        public Comment GetCommentByID(long id)
        {
            return context.Comments.Find(id);
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

        public string InsertComment(Comment Comment)
        {
            Comment.CreatedTime = DateTime.UtcNow;
            context.Comments.Add(Comment);
            Save();
			double totalMill = (new TimeSpan(Comment.CreatedTime.Ticks)).TotalMilliseconds;
			return totalMill.ToString();
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