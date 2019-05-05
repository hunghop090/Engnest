namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Message")]
    public partial class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public string Content { get; set; }

        public long? UserID { get; set; }

        public long? TargetUser { get; set; }

        public DateTime CreatedTime { get; set; }

        [StringLength(500)]
        public string Audios { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(500)]
        public string Other { get; set; }

        public bool? Seen { get; set; }
    }
}
