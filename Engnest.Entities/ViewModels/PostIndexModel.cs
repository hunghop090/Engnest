using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class PostIndexModel
	{
		public ProfileModel Profile { get; set; }

		public long? Id { get; set; }

		public long? CommentId { get; set; }
	}
}