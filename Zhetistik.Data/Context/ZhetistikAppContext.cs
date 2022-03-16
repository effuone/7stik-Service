using System.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zhetistik.Data.Models;

namespace Zhetistik.Data.Context
{
    public class ZhetistikAppContext : IdentityDbContext<ZhetistikUser>
    {
        public ZhetistikAppContext()
        {

        }

        public ZhetistikAppContext(DbContextOptions<ZhetistikAppContext> options)
            : base(options)
        {
        }
        public DbSet<FileModel> FileModels {get; set;}
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<School> Schools {get; set;}
        public DbSet<Candidate> Candidates {get; set;}
        public DbSet<Portfolio> Portfolios {get; set;}
        public DbSet<Achievement> Achievements {get; set;}
        public DbSet<AchievementType> AchievementTypes {get; set;}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:localhost;Database=ZhetistikDb;User Id=SA;Password=");
            // optionsBuilder.UseNpgsql("Host=postgres;Database=blog_database;Username=docker_user;Password=password");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // UNIQUE additions
            builder.Entity<Country>().HasIndex(x=>x.CountryName).IsUnique(true);
            builder.Entity<City>().HasIndex(x=>x.CityName).IsUnique(true);
            builder.Entity<Location>().HasIndex(x=>x.CityId).IsUnique(true);
            builder.Entity<School>().HasIndex(x=>x.SchoolName).IsUnique(true);
            
            //Relationship configuring 
            builder.Entity<Candidate>().HasOne(x=>x.Portfolio).WithOne(x=>x.Candidate);
            builder.Entity<School>().HasMany(x=>x.Candidates).WithOne(x=>x.School);
            builder.Entity<Candidate>().HasOne(x=>x.School).WithMany(x=>x.Candidates);
            builder.Entity<Location>().HasOne(x=>x.City);
            builder.Entity<Location>().HasOne(x=>x.Country);
            builder.Entity<Portfolio>().HasMany(x=>x.Achievements).WithOne(x=>x.Portfolio);
            builder.Entity<Portfolio>().HasOne(x=>x.Candidate).WithOne(x=>x.Portfolio).IsRequired();
            builder.Entity<Achievement>().HasOne(x=>x.AchievementType);
            builder.Entity<Achievement>().HasOne(x=>x.FileModel);
            builder.Entity<AchievementType>().HasIndex(x=>x.AchievementTypeName).IsUnique(true);
        }      
    }
}
