using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Engnest.Entities.ViewModels
{
	public class MemberModel
    {
        public long Id { get; set; }

        public string Avatar { get; set; }

        public string NickName { get; set; }

		public DateTime CreatedTime { get; set; }

		public byte Type { get; set; }

    }
}