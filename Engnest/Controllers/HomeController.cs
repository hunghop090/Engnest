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
    }
}