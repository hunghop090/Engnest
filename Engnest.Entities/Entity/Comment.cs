namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        public long ID { get; set; }

        [Required]
        public string Content { get; set; }

        public byte? Type { get; set; }
		[Required]
        public long TargetId { get; set; }

        public byte? TargetType { get; set; }

        public DateTime CreatedTime { get; set; }

        public byte? Status { get; set; }

        public string Images { get; set; }

        public string Audios { get; set; }

        [StringLength(500)]
        public string Tags { get; set; }

        [StringLength(500)]
        public string TagsUser { get; set; }

		public long UserId { get; set; }
    }
}
