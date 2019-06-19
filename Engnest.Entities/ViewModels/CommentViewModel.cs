using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class CommentViewModel
	{
		public long Id { get; set; }

		public string Content { get; set; }

		public byte? Type { get; set; }

		public long TargetId { get; set; }

		public byte? TargetType { get; set; }

		public DateTime CreatedTime { get; set; }

		public byte? Status { get; set; }

		public string Images { get; set; }

		public string Audios { get; set; }

		public string Tags { get; set; }

		public string TagsUser { get; set; }

		public long UserId { get; set; }

		public string NickName { get; set; }

		public string Avatar { get; set; }

		public int CountReply { get; set; }

		public int CountEmotions {get;set;}
	}
}