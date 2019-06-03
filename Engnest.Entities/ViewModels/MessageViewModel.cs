using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class MessageViewModel
	{
		public long Id { get; set; }

		public string Content { get; set; }

		public long? UserId { get; set; }

		public long? TargetUser { get; set; }

		public DateTime CreatedTime { get; set; }

		public string Audios { get; set; }

		public string Image { get; set; }

		public string Other { get; set; }

		public bool? Seen { get; set; }

		public string AvataUser { get; set; }

		public string AvataTarget { get; set; }

		public string NickNameUser { get; set; }

		public string NickNameTarget { get; set; }
	}
}