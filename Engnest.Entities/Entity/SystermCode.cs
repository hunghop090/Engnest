namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SystermCode")]
    public partial class SystermCode
    {
        public long ID { get; set; }

        public long? SysID { get; set; }

        [StringLength(20)]
        public string Value { get; set; }

        [StringLength(200)]
        public string Text { get; set; }

        public string Note { get; set; }
    }
}
