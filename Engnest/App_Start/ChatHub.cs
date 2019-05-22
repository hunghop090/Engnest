using Microsoft.AspNet.SignalR;

namespace Engnest
{
	public class ChatHub : Hub
	{
		public void LetsChat(string Cl_Name, string Cl_Message)
		{
			Clients.All.NewMessage(Cl_Name, Cl_Message);
		}
	}
}