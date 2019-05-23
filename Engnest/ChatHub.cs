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

namespace Engnest
{
    public class ChatHub : Hub
    {
    public override Task OnConnected()
    {
		Section.Add(Constant.USER_SESSION, user.ID);
		BaseController.HubUser.HubId = Context.ConnectionId;
        return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        return base.OnDisconnected(stopCalled);
    }

    public void Send(string name, string message)
    {
		var ids = name.Split(',').ToList();
		if(ids.Contains(BaseController.HubUser.UserId.ToString()))
			Clients.Client(Context.ConnectionId).addNewMessageToPage(name,message);
    }

    }
}