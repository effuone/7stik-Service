using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Zhetistik.Data.Models;

namespace Zhetistik.Data.Context
{
    public class ZhetistikAppContext : DbContext
    {
        public ZhetistikAppContext()
        {

        }

        public ZhetistikAppContext(DbContextOptions<ZhetistikAppContext> options)
            : base(options)
        {
        }

        // public DbSet<Achievement> Achievements { get; set; }
        // public DbSet<AchievementType> AchievementTypes { get; set; }
        // public DbSet<Candidate> Candidates { get; set; }
        // public DbSet<City> Cities { get; set; }
        // public DbSet<Country> Countries { get; set; }
        // public DbSet<Course> Courses { get; set; }
        // public DbSet<Faculty> Faculties { get; set; }
        // public DbSet<Location> Locations { get; set; }
        // public DbSet<Portfolio> Portfolios { get; set; }
        // public DbSet<School> Schools { get; set; }
        // public DbSet<University> Universities { get; set; }
        // public DbSet<UniversityType> UniversityTypes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<School> Schools {get; set;}
        public DbSet<University> Universities { get; set;}
        public DbSet<UniversityType> UniversityTypes { get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                const string pc = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ZhetistikApp";
                const string laptop = "";
                optionsBuilder.UseSqlServer(laptop);
            }
        }
    }
}
