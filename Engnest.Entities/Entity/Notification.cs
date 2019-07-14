namespace Engnest.Entities.Entity
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;


	[Table("Notification")]
	public partial class Notification
	{
		public long ID { get; set; }
		[Required]
		public string Content { get; set; }

		public long UserId { get; set; }

		public byte? Type { get; set; }

		public DateTime CreatedTime { get; set; }

		public bool? Seen { get; set; }
		public string HTML { get; set; }

		public long? TargetId {get;set;}
	}
}
