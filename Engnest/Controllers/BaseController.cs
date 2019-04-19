using Engnest.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Engnest.Controllers
{
    public class BaseController : Controller
    {
		// GET: Base
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var session = (string)Session[Constant.USER_SESSION];
			if(session == null)
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login",action = "index"}));
			base.OnActionExecuting(filterContext);
		}
	}
}