namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Relationship")]
    public partial class Relationship
    {
        public long ID { get; set; }

        [Required]
        public long UserSentID { get; set; }

         [Required]
        public long UserReceiveID { get; set; }

        public byte Type { get; set; }

		public byte Status { get; set; }

        public DateTime CreatedTime { get; set; }
		public DateTime? UpdateTime { get; set; }
		public DateTime? AcceptTime { get; set; }

    }
}
