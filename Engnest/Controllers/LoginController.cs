using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.Repository;
using Engnest.Entities.ViewModels;

namespace Engnest.Controllers
{
	public class LoginController : Controller
	{
		private IUserRepository userRepository;

		public LoginController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
		}

		public LoginController(IUserRepository userRepository)
		{
			this.userRepository = userRepository;
		}
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(LoginModel model)
		{
			byte result = 0;
			string message = "";
			try
			{

				if (ModelState.IsValid)
				{
					User user = new User();
					var Password = EncryptorMD5.MD5Hash(model.Password);
					result = userRepository.Login(model.UserName, Password, out user);
					if (result == LoginStatus.SUCCESS)
					{
						var ticket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMonths(1), true, user.ID.ToString());
						string encryptedTicket = FormsAuthentication.Encrypt(ticket);
						Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
					}
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}
			Response.StatusCode = (int)HttpStatusCode.OK;
			return Json(new { result = result });
		}
		[HttpPost]
		public ActionResult SignIn(SignInModel model)
		{
			if (ModelState.IsValid)
			{
				byte result = 0;
				if (model.UserName.Length > 20 || model.UserName.Length < 4)
					return Json(new { result = Constant.ERROR, message = "Error!" });
				model.Password = EncryptorMD5.MD5Hash(model.Password);
				result = userRepository.SignIn(model);
				if (result == LoginStatus.SUCCESS)
				{
					userRepository.Save();
					var user = userRepository.GetUserByName(model.UserName);
					var ticket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMonths(1), true, user.ID.ToString());
					string encryptedTicket = FormsAuthentication.Encrypt(ticket);
					Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
				}
				Response.StatusCode = (int)HttpStatusCode.OK;
				return Json(new { result = Constant.SUCCESS });
			}
			return Json(new { result = Constant.ERROR, message = "Error!" });
		}

		public ActionResult SignOut()
		{
			var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			if (authCookie != null)
			{
				var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				var ticket = new FormsAuthenticationTicket(1, authTicket.Name, DateTime.Now, DateTime.Now.AddHours(-24), true, authTicket.UserData);
				string encryptedTicket = FormsAuthentication.Encrypt(ticket);
				Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
			}
			return RedirectToAction("Index");
		}
	}
}