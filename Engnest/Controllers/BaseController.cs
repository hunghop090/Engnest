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
using System.Web.Security;

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
			var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

		if (authCookie == null)
			{
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "index" }));
			}
			else
			{
				try
				{
					var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
					if (authTicket.Expired)
					{
						filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "index" }));
					} else
					{
						if (userLogin == null || userLogin.ID != long.Parse(authTicket.UserData))
						{
							userLogin = userRepository.GetUserByID(long.Parse(authTicket.UserData));
						}
						var ticket = new FormsAuthenticationTicket(1, authTicket.Name, DateTime.Now, DateTime.Now.AddMonths(1), true, authTicket.UserData);
						string encryptedTicket = FormsAuthentication.Encrypt(ticket);
						Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
						ViewBag.ProfileModel = Mapper.Map<ProfileModel>(userLogin);
					}
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