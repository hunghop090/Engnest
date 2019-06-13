using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Engnest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
			
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			MapperConfig.RegisterMappers();
			ProfileManager.RegisterProfile("AWSProfileName", "AKIAJZMY3NCCEJQXACJA", "dbVrcf7AQ01Nt5qz03oxgNpvpIRhvmrmcXVUAUbg");
        }
    }
}
