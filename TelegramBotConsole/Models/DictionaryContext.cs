using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TelegramBotConsole.Models
{
    public class DictionaryContext : DbContext
    {
        public DictionaryContext()
        {
            Database.EnsureCreated();
        }
        public DictionaryContext(DbContextOptions<DictionaryContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)//Подключение базы данных
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("config.json");
            var configuration = builder.Build();
            string connectionString = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
            string path = Path.GetPathRoot(Directory.GetCurrentDirectory());
            string constr = connectionString.Replace("=", "=" + path);
            optionsBuilder.UseSqlite(constr);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Define the Table(s) and References to be created automatically
            modelBuilder.Entity<Word>(b =>
            {
                b.HasKey(e => e.OriginalWord);
                b.Property(e => e.OriginalWord).IsRequired().HasMaxLength(255);
                b.Property(e => e.TranslatedWord).IsRequired().HasMaxLength(255);
                b.ToTable("Words");
            });
        }
        public DbSet<Word> Words { get; set; }
    }
}
