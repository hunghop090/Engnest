using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  IMessageRepository : IDisposable
	{
        List<Message> GetMessages();
		List<MessageViewModel> LoadMessages(string date,long UserId,long TargetId);

		List<MessageViewModel> LoadMessagesNotifi(long id);
        Message GetMessageByID(long MessageId);
        List<Message> GetMessageByTargetId(long TargetId);
        void InsertMessage(Message Message);
        void DeleteMessage(long MessageID);
        void UpdateMessage(Message Message);

		void UpdateSeen(long TargetId,long UserId);
        void Save();
	}
}