using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketEase.Domain.Entities;

namespace TicketEase.Persistence.Context
{
	public class TicketEaseDbContext : IdentityDbContext<AppUser>
	{
		public TicketEaseDbContext(DbContextOptions<TicketEaseDbContext> options):base(options){}

		public DbSet<Board> Boards { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Manager> Managers { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<Ticket> Tickets { get; set; }

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	base.OnModelCreating(modelBuilder);

		//	// Configure the relationship
		//	modelBuilder.Entity<AppUser>()
		//		.HasOne(u => u.Manager)
		//		.WithMany(o => o.Users)
		//		.HasForeignKey(u => u.ManagerId)
		//		.IsRequired(true);
		//}
		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			foreach (var item in ChangeTracker.Entries<BaseEntity>())
			{
				switch (item.State)
				{
					case EntityState.Modified:
						item.Entity.UpdatedAt = DateTime.UtcNow;
						break;
					case EntityState.Added:
						item.Entity.Id = Guid.NewGuid().ToString();
						item.Entity.CreatedAt = DateTime.UtcNow;
						break;
					default:
						break;
				}
			}
			return await base.SaveChangesAsync(cancellationToken);
		}
		
	}
    
}