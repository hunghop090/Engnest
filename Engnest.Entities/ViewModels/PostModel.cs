using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class PostModel
	{
        public long? UserId { get; set; }

        public byte? Type { get; set; }

        public long? TargetId { get; set; }

        public byte? TargetType { get; set; }

        public string Content { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string Tags { get; set; }

        public string Images { get; set; }

        public string Audios { get; set; }

        public string TagsUser { get; set; }

		public List<string> ListImages { get; set; }

		public List<string> ListVideos { get; set; }
	}
}