using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.Repository;
using Engnest.Entities.ViewModels;

namespace Engnest.Controllers
{
    public class HomeController : BaseController
    {
		private IUserRepository userRepository;

		public HomeController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
		}

		public HomeController(IUserRepository userRepository)
		{
			this.userRepository = userRepository;
		}
        public ActionResult Index()
        {
            var fooDto = Mapper.Map<ProfileModel>(userLogin);
            return View();
        }

		[HttpPost]
        public ActionResult CreatedPost(LoginModel model)
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

    }
}