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
        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(50)]
        public string NickName { get; set; }

        [StringLength(50)]
        public string SubName { get; set; }

        [StringLength(500)]
        public string Status { get; set; }

        public string Avatar { get; set; }

        public string BackGround { get; set; }

        public byte Type { get; set; }

        public DateTime CreatedTime { get; set; }

        [StringLength(20)]
        public string ActiveCode { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool? Flag { get; set; }

		public DateTime? Birthday {get;set;}

		public string Country {get;set;}

		public string Gender {get;set;}

		public string Category {get;set;}

		public string Relationship {get;set;}

		public string AboutMe {get;set;}

		public decimal? Lat {get;set;}

		public decimal? Lng {get;set;}
    }
}
