namespace Engnest.Entities.Entity
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class EngnestContext : DbContext
	{
		public EngnestContext()
			: base("name=EngnestContext")
		{
		}

		public virtual DbSet<Comment> Comments { get; set; }
		public virtual DbSet<Emotion> Emotions { get; set; }
		public virtual DbSet<Group> Groups { get; set; }
		public virtual DbSet<Message> Messages { get; set; }
		public virtual DbSet<Post> Posts { get; set; }
		public virtual DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Emotion>()
				.Property(e => e.Type)
				.IsFixedLength();

			modelBuilder.Entity<Emotion>()
				.Property(e => e.TargetType)
				.IsFixedLength();

			modelBuilder.Entity<User>()
				.Property(e => e.Email)
				.IsFixedLength();

			modelBuilder.Entity<User>()
				.Property(e => e.Phone)
				.IsFixedLength();

			modelBuilder.Entity<User>()
				.Property(e => e.Token)
				.IsFixedLength();

			modelBuilder.Entity<User>()
				.Property(e => e.Password)
				.IsFixedLength();

			modelBuilder.Entity<User>()
				.Property(e => e.ActiveCode)
				.IsFixedLength();
		}
	}
}
