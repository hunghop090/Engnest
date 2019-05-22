using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(Engnest.Startup))]
namespace Engnest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}