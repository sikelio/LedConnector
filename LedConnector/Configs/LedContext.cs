using LedConnector.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LedConnector.Configs
{
    public class LedContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public LedContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

#if DEBUG
            string? authString = config.GetConnectionString("dev");

            optionsBuilder
                .UseMySql(authString, ServerVersion.AutoDetect(authString))
                .UseLazyLoadingProxies()
                .EnableSensitiveDataLogging();
#else
            string? authString = config.GetConnectionString("prod");

            optionsBuilder
                .UseMySql(authString, ServerVersion.AutoDetect(authString))
                .UseLazyLoadingProxies();
#endif
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Message>()
                .Property(m => m.Id)
                .HasColumnType("integer");

            builder
                .Entity<Message>()
                .Property(m => m.RawMessage)
                .HasColumnType("varchar")
                .HasMaxLength(255);

            builder
                .Entity<Message>()
                .Property(m => m.BinaryMessage)
                .HasColumnType("text");
        }
    }
}
