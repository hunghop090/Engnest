using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class PostViewModel
	{
		public PostViewModel()
		{
			ListComments = new List<CommentViewModel>();
		}

		public long? Id { get; set; }

		public long? UserId { get; set; }

		public byte? Type { get; set; }

		public long? TargetId { get; set; }

		public byte? TargetType { get; set; }

		public string Content { get; set; }

		public DateTime? UpdateTime { get; set; }

		public DateTime? CreatedTime { get; set; }
		public string Tags { get; set; }

		public string Images { get; set; }

		public string Audios { get; set; }

		public string TagsUser { get; set; }

		public string NickName { get; set; }

		public string Avatar { get; set; }

		public int CountEmotions { get; set; }

		public int CountComments { get; set; }

		public byte StatusEmotion { get; set; }

		public List<CommentViewModel> ListComments { get; set; }

		public List<string> ListImages { get; set; }
		public List<string> ListAudios { get; set; }
	}
}