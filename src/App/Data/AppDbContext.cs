using App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Data
{
    public class AppDbContext : DbContext
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
            Language[] LanguageList;
            LanguageList = new[]
            {
                new Language {LanguageId = 1, Name = "Java"}, new Language {LanguageId = 2, Name = "C#"},
                new Language {LanguageId = 3, Name = "PHP"}, new Language {LanguageId = 4, Name = "JavaScript"},
                new Language {LanguageId = 5, Name = "TypeScript"}, new Language {LanguageId = 6, Name = "Kotlin"},
                new Language {LanguageId = 7, Name = "Ruby"}, new Language {LanguageId = 8, Name = "Go"},
                new Language {LanguageId = 9, Name = "Scala"}, new Language {LanguageId = 10, Name = "Flex"},
                new Language {LanguageId = 11, Name = "Python"}, new Language {LanguageId = 12, Name = "HTML"},
                new Language {LanguageId = 13, Name = "CSS"}, new Language {LanguageId = 14, Name = "XML"},
                new Language {LanguageId = 15, Name = "VB.NET"}, new Language {LanguageId = 16, Name = "Other"}
            };
            modelBuilder.Entity<Language>().HasData(LanguageList);
            base.OnModelCreating(modelBuilder);
        }
    }
}
