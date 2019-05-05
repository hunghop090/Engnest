using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            if (ModelState.IsValid)
            {
                User user = new User();
                var Password = EncryptorMD5.MD5Hash(model.Password);
                result = userRepository.Login(model.UserName, Password, out user);
                if (result == LoginStatus.SUCCESS)
                    Session.Add(Constant.USER_SESSION, user.ID);
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { result });
        }

        public ActionResult SignIn(SignInModel model)
        {
            model.Password = EncryptorMD5.MD5Hash(model.Password);
			var result = userRepository.SignIn(model);
			if(result == LoginStatus.SUCCESS)
            {
                userRepository.Save();
                var user = userRepository.GetUserByName(model.UserName);
                Session.Add(Constant.USER_SESSION, user.ID);
            }
				
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SignOut()
        {
            Session.Remove(Constant.USER_SESSION);
            return RedirectToAction("Index");
        }
    }
}