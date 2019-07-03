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
	public class MessageRepository : IMessageRepository, IDisposable
    {
        private EngnestContext context;

        public MessageRepository(EngnestContext context)
        {
            this.context = context;
        }
		 public List<MessageViewModel> LoadMessages(string date,long UserId,long TargetId)
        {
			DateTime createDate = DateTime.UtcNow;
			if (!string.IsNullOrEmpty(date))
			{
				createDate = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
				createDate = createDate.AddMilliseconds(double.Parse(date)).ToLocalTime();
			}
			var result = (from c in context.Messages
				join p1 in context.Users on UserId equals p1.ID  into ps1
				from p1 in ps1.DefaultIfEmpty()
				join p2 in context.Users on TargetId equals p2.ID  into ps2
				from p2 in ps2.DefaultIfEmpty()
				where c.CreatedTime < createDate && ((c.TargetUser == TargetId && c.UserID == UserId) ||  (c.TargetUser == UserId && c.UserID == TargetId))
				orderby c.CreatedTime descending
				select new{c ,p1,p2}).Take(10).ToList();
			var MessageView = new List<MessageViewModel>();
			foreach(var item in result)
			{
				var Message = new MessageViewModel();
				Message.Id = item.c.ID ;
				Message.Audios = item.c.Audios ;
				Message.Content = item.c.Content ;
				Message.Image = item.c.Image ;
				if(!string.IsNullOrEmpty(item.c.Image))
				{
					var data = item.c.Image.Split(',');
					Message.ListImages = new List<string>();
					foreach(string image in data)
					{
						var respone = AmazonS3Uploader.GetUrl(image);
						if(!string.IsNullOrEmpty(respone))
							Message.ListImages.Add(respone);
					}
				}
				if(!string.IsNullOrEmpty(item.c.Audios))
				{
					var data = item.c.Audios.Split(',');
					Message.ListAudios = new List<string>();
					foreach(string audio in data)
					{
						var respone = AmazonS3Uploader.GetUrl(audio);
						if(!string.IsNullOrEmpty(respone))
							Message.ListAudios.Add(respone);
					}
				}
				Message.Other = item.c.Other ;
				Message.Seen = item.c.Seen ;
				Message.TargetUser = item.c.TargetUser ;
				Message.UserId = item.c.UserID ;
				Message.CreatedTime = item.c.CreatedTime;
				var responeImage = AmazonS3Uploader.GetUrl(item.p2?.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Message.AvataTarget = responeImage;
				responeImage = AmazonS3Uploader.GetUrl(item.p1?.Avatar,0);
				if(!string.IsNullOrEmpty(responeImage))
					Message.AvataUser = responeImage;
				Message.NickNameTarget = item.p2?.NickName ;
				Message.NickNameUser = item.p1?.NickName ;
				MessageView.Add(Message);
			}
            return MessageView;
        }
        public List<Message> GetMessages()
        {
            return context.Messages.ToList();
        }


        public Message GetMessageByID(long id)
        {
            return context.Messages.Find(id);
        }

        public List<Message> GetMessageByTargetId(long id)
        {
            return new List<Message>();
        }

        public void InsertMessage(Message Message)
        {
            Message.CreatedTime = DateTime.UtcNow;
            context.Messages.Add(Message);
            Save();
        }

        public void DeleteMessage(long MessageID)
        {
            Message Message = context.Messages.Find(MessageID);
            context.Messages.Remove(Message);
			Save();
        }

        public void UpdateMessage(Message Message)
        {
            context.Entry(Message).State = EntityState.Modified;
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