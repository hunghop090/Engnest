namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Group")]
    public partial class Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [StringLength(200)]
        public string GroupName { get; set; }

        public long? CreatedUser { get; set; }

        [StringLength(500)]
        public string Avata { get; set; }

        [StringLength(500)]
        public string Banner { get; set; }

        [StringLength(500)]
        public string InfoId { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
