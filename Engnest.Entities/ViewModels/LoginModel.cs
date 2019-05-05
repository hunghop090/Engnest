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
	}

	public class SignInModel
	{
		[Required(ErrorMessage ="Enter your username")]
		public string UserName { get; set; }

		[Required(ErrorMessage ="Enter your password")]
		public string Password { get; set; }

		[Required(ErrorMessage ="Enter your email")]
		public string Email { get; set; }

		[Required(ErrorMessage ="Enter your first name")]
		public string FirstName { get; set; }

		[Required(ErrorMessage ="Enter your last name")]
		public string LastName { get; set; }
	}
}