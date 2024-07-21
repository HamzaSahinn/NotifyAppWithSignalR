using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotifyAppWithSignalR.Domain;

namespace NotifyAppWithSignalR.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Message> Messages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Message>().HasKey(e => e.Id);
            builder.Entity<Message>().HasOne(m => m.Reciver).WithMany().HasForeignKey(fk => fk.ReciverId);
            builder.Entity<Message>().HasOne(m => m.Sender).WithMany().HasForeignKey(fk => fk.SenderId);
            builder.Entity<Message>().Navigation(m => m.Sender).AutoInclude();
            builder.Entity<Message>().Navigation(m => m.Reciver).AutoInclude();

            base.OnModelCreating(builder);
        }
    }
}
