namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Emotion")]
    public partial class Emotion
    {
        public long ID { get; set; }

        [StringLength(10)]
        public string Type { get; set; }

        public long? UserId { get; set; }

        public long? TargetId { get; set; }

        [StringLength(10)]
        public string TargetType { get; set; }

        public DateTime CreatedTime { get; set; }

		public byte Status { get; set; }
    }
}
