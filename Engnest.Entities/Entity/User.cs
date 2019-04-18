namespace Engnest.Entities.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string Token { get; set; }

        [Required]
        [StringLength(20)]
        public string Password { get; set; }

        [StringLength(50)]
        public string NickName { get; set; }

        [StringLength(50)]
        public string SubName { get; set; }

        [StringLength(500)]
        public string Status { get; set; }

        [StringLength(200)]
        public string Avatar { get; set; }

        [StringLength(200)]
        public string BackGround { get; set; }

        public byte Type { get; set; }

        public DateTime CreatedTime { get; set; }

        [StringLength(20)]
        public string ActiveCode { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool? Flag { get; set; }
    }
}
