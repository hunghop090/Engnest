using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class ProfileModel
    {
        public long ID { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }

        public string NickName { get; set; }

        public string SubName { get; set; }

        public string Status { get; set; }

        public string Avatar { get; set; }

        public string BackGround { get; set; }

        public byte Type { get; set; }

        public DateTime CreatedTime { get; set; }

        public string ActiveCode { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool? Flag { get; set; }
    }
}