﻿using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.Repository;
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
		public class HubUser
		{
			public static long UserId { get; set; }
			public static string HubId { get; set; }
		};
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
			var session = Session[Constant.USER_SESSION];
			if (session == null)
			{
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "index" }));
			}
			else if (userLogin == null || userLogin.ID != (long)session)
			{
				userLogin = userRepository.GetUserByID((long)session);
				HubUser.UserId = userLogin.ID;
			}
			base.OnActionExecuting(filterContext);
		}
	}
}