using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    /// <summary>
    /// Model for jobseeker data. Is additional to the user account data.
    /// One-to-Many with JobApplication.
    /// One-to-Many with Offer.
    /// Refers to AppUser.
    /// </summary>
    public class Jobseeker
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<JobApplication> JobApplications { get; set; } = [];
        public List<Offer> Offers { get; set; } = [];
        public string Profession { get; set; } = string.Empty;
        public float Experience { get; set; } = 0;
        public DegreeTypes Education { get; set; } = DegreeTypes.NONE;
        public string Location { get; set; } = string.Empty;
        public string PreviousWorkplace { get; set; } = string.Empty;
        public string PreviousPosition { get; set; } = string.Empty;
        public string QuitReason { get; set; } = string.Empty;
        public string FamilyConditions { get; set; } = string.Empty;
        public string LivingConditions { get; set; } = string.Empty;
        public string Preferences { get; set; } = string.Empty;
        public string SelfDescription { get; set; } = string.Empty;
        public bool IsEmployed { get; set; } = false;
        public DateTime RegisterDate { get; private set; } = DateTime.UtcNow;
    }
}