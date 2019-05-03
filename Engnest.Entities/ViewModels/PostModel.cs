using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class PostModel
	{
		[Required(ErrorMessage ="Enter your username")]

        public long? UserId { get; set; }

        public byte? Type { get; set; }

        public long? TargetId { get; set; }

        public byte? TargetType { get; set; }

        public string Content { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        [StringLength(500)]
        public string Tags { get; set; }

        public string Images { get; set; }

        [StringLength(500)]
        public string Audios { get; set; }

        public string TagsUser { get; set; }
	}
}