namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GroupMember")]
    public partial class GroupMember
    {
        public long ID { get; set; }

        [Required]
        public long GroupID { get; set; }

         [Required]
        public long UserID { get; set; }

        public byte Type { get; set; }

		public byte Status { get; set; }

        public DateTime CreatedTime { get; set; }

    }
}
