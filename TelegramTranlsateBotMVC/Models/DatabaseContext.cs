using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TelegramTranslateBotMVC.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
             : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
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

