using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    /// <summary>
    /// Database context for the application
    /// Manages DDL and DML operations for the whole app
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Jobseeker> Jobseekers { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }

        /// <summary>
        /// Configures model relationships, enum mapping and role seeding
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole> roles =
            [
                new IdentityRole
                {
                  Name = "JOBSEEKER",
                  NormalizedName = "JOBSEEKER",
                  Id = "8c688e00-551a-4d63-a93c-20118a9332e5"
                },
                new IdentityRole
                {
                  Name = "COMPANY",
                  NormalizedName = "COMPANY",
                  Id = "22bfe0d9-8d28-4593-ab37-5ffd3097b7ed"
                },
                new IdentityRole
                {
                  Name = "ADMIN",
                  NormalizedName = "ADMIN",
                  Id = "8f29d3c7-6a1e-4b90-925c-3f78d2e1b057"
                },
            ];
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            modelBuilder.Entity<JobApplication>()
                .Property(j => j.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Offer>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Jobseeker>()
                .Property(j => j.Education)
                .HasConversion<string>();

            modelBuilder.Entity<Vacancy>()
                .Property(j => j.WorkMode)
                .HasConversion<string>();


            modelBuilder.Entity<Jobseeker>()
                .HasOne(u => u.AppUser)
                .WithOne()
                .HasForeignKey<Jobseeker>(j => j.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Company>()
                .HasOne(c => c.AppUser)
                .WithOne()
                .HasForeignKey<Company>(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Vacancy>()
                .HasOne(v => v.Company)
                .WithMany(c => c.Vacancies)
                .HasForeignKey(v => v.CompanyId);


            modelBuilder.Entity<JobApplication>().HasKey(a => new { a.JobseekerId, a.VacancyId });

            modelBuilder.Entity<JobApplication>()
                .HasOne(a => a.Jobseeker)
                .WithMany(j => j.JobApplications)
                .HasForeignKey(a => a.JobseekerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<JobApplication>()
                .HasOne(a => a.Vacancy)
                .WithMany(j => j.JobApplications)
                .HasForeignKey(a => a.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Offer>().HasKey(o => new { o.JobseekerId, o.VacancyId });

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Jobseeker)
                .WithMany(j => j.Offers)
                .HasForeignKey(o => o.JobseekerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Vacancy)
                .WithMany(j => j.Offers)
                .HasForeignKey(o => o.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}