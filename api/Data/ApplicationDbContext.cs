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
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Jobseeker> Jobseekers { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Offer> Offers { get; set; }

        /// <summary>
        /// Configures model relationships, enum mapping and role seeding
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole> roles =
            [
                new IdentityRole { Name = "Jobseeker", NormalizedName = "JOBSEEKER"},
                new IdentityRole { Name = "Company", NormalizedName = "COMPANY"},
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
                .HasForeignKey<Jobseeker>(j => j.AppUserId);


            modelBuilder.Entity<Company>()
                .HasOne(c => c.AppUser)
                .WithOne()
                .HasForeignKey<Company>(c => c.AppUserId);


            modelBuilder.Entity<Vacancy>()
                .HasOne(v => v.Company)
                .WithMany(c => c.Vacancies)
                .HasForeignKey(v => v.CompanyId);


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


            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Jobseeker)
                .WithMany(j => j.Offers)
                .HasForeignKey(o => o.JobSeekerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Vacancy)
                .WithMany(j => j.Offers)
                .HasForeignKey(o => o.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}