using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Demo2.Core
{
    public class Entry
    {
        [Key]
        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public decimal Difference { get; set; }
    }

    public class Context(IConfiguration configuration) : DbContext
    {
        public DbSet<Entry> Entries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql(configuration.GetConnectionString(nameof(Context))!);
        }
    }
}
