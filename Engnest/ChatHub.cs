using System;
using System.Net;
using System.Web.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Engnest.Controllers;
using Engnest.Entities.Common;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Engnest
{
    public class ChatHub : Hub
    {
		public class MessageDetail
		{
			public string UserName { get; set; }

			public string Message { get; set; }
		}
		public class UserDetail
		{
			public string ConnectionId { get; set; }
			public string UserName { get; set; }
		}
        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

		public void SendChatMessage(string who, string message)
        {
			if(who != string.Empty || message != string.Empty)
			{
				var User = ConnectedUsers.Where(x=>x.UserName == who).FirstOrDefault();
				Clients.Client(User.ConnectionId).addChatMessage(message);
			}
        }

        public void Connected(string who)
        {
			if(who != string.Empty)
			{
				UserDetail User = ConnectedUsers.Where(x=>x.UserName == who).FirstOrDefault();
				if(User == null || User.ConnectionId == Context.ConnectionId)
				{
					ConnectedUsers.Remove(User);
					ConnectedUsers.Add(new UserDetail{ConnectionId = Context.ConnectionId,UserName = who});
				}
			}
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            ConnectedUsers.Remove(ConnectedUsers.Where(x=>x.ConnectionId == Context.ConnectionId).FirstOrDefault());
            return base.OnDisconnected(stopCalled);
        }
    }
}