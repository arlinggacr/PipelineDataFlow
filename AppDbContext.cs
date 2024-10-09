using Microsoft.EntityFrameworkCore;
using PipelineDataFlow.Models;

namespace PipelineDataFlow.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<PipelineData> PipelineData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Local2AppDbContext : DbContext
    {
        public Local2AppDbContext(DbContextOptions<Local2AppDbContext> options)
            : base(options) { }

        public DbSet<PipelineTarget> PipelineTarget { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
