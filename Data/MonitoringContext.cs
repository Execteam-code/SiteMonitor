using Microsoft.EntityFrameworkCore;
using ServiceMonitor.Web.Models.Entities;

namespace ServiceMonitor.Web.Data
{
    public class MonitoringContext : DbContext
    {
        public MonitoringContext(DbContextOptions<MonitoringContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TargetUrl)
                    .IsRequired()
                    .HasMaxLength(2048);
            });

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "YouTube", TargetUrl = "https://youtube.com", IsOnline = false },
                new Service { Id = 2, Name = "GitHub", TargetUrl = "https://github.com", IsOnline = false },
                new Service { Id = 3, Name = "Reddit", TargetUrl = "https://reddit.com", IsOnline = false },
                new Service { Id = 4, Name = "Discord", TargetUrl = "https://discord.com", IsOnline = false },
                new Service { Id = 5, Name = "ChatGPT", TargetUrl = "https://chatgpt.com", IsOnline = false }
            );
        }
    }
}
