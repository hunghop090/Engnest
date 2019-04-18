using System;
using System.Collections.Generic;
using System.Linq;
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

		public ActionResult Login(LoginModel model)
        {
			var Password = EncryptorMD5.MD5Hash(model.Password);
			var result = userRepository.Login(model.UserName,Password);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}