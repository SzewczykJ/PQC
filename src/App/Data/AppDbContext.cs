using Microsoft.EntityFrameworkCore;
using App.Data.Entities;

namespace App.Data
{
    public class AppDbContext: DbContext
    {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

            public DbSet<Developer> Developers { get; set; }
            public DbSet<Repository> Repositories { get; set; }
            public DbSet<Commit> Commits { get; set; }
            public DbSet<File> Files { get; set; }
            public DbSet<FileDetail> FileDetails { get; set; }
            public DbSet<Language> Languages { get; set; }
            public DbSet<Metric> Metrics { get; set; }
            public DbSet<Branch> Branches { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
            }
        }
    }
