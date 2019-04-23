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
    public class ProfileController : BaseController
    {
		private IUserRepository userRepository;

		public ProfileController()
		{
			this.userRepository = new UserRepository(new EngnestContext());
		}

		public ProfileController(IUserRepository userRepository)
		{
			this.userRepository = userRepository;
		}
        public ActionResult Index()
        {
            var model = Mapper.Map<ProfileModel>(userLogin);
            return View(model);
        }
    }
}