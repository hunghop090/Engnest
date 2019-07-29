using AutoMapper;
using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.Repository;
using Engnest.Entities.ViewModels;
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
		public static User userLogin;

		private IUserRepository userRepository;

		public BaseController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
		}

		public BaseController(IUserRepository userRepository)
		{
			this.userRepository = userRepository;
		}
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var sessionCookie = Request.Cookies[Constant.USER_SESSION];

			if (sessionCookie == null)
			{
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "index" }));
			}
			else
			{
				try
				{
					if (userLogin == null || userLogin.ID != long.Parse(sessionCookie.Value))
					{
						userLogin = userRepository.GetUserByID(long.Parse(sessionCookie.Value));
					}
					sessionCookie.Expires = DateTime.Now.AddMonths(1);
					Response.Cookies.Add(sessionCookie);
					ViewBag.ProfileModel = Mapper.Map<ProfileModel>(userLogin);
				}
				catch(Exception ex)
				{
					filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "index" }));
				}

			}
			base.OnActionExecuting(filterContext);
		}
	}
}