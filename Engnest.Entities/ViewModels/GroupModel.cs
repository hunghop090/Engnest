using Engnest.Entities.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	[NotMapped]
	public class GroupModel : Group
    {
		public ProfileModel Profile {get;set;}
    }
}