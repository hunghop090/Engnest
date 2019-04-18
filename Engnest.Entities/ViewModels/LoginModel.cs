using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class LoginModel
	{
		[Required(ErrorMessage ="Enter your username")]
		public string UserName { get; set; }

		[Required(ErrorMessage ="Enter your password")]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}