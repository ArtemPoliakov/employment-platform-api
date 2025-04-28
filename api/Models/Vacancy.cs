using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models
{
    /// <summary>
    /// Vacancy model
    /// One-to-Many with JobApplication.
    /// One-to-Many with Offer.
    /// </summary>
    public class Vacancy
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public List<Offer> Offers { get; set; } = [];
        public List<JobApplication> JobApplications { get; set; } = [];
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CandidateDescription { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public float SalaryMin { get; set; } = 0;
        public float SalaryMax { get; set; } = 0;
        public VacancyWorkModes WorkMode { get; set; } = VacancyWorkModes.OTHER;
        public string LivingConditions { get; set; } = string.Empty;
        public DateTime EditDate { get; set; }
        public DateTime PublishDate { get; private set; } = DateTime.UtcNow;
    }
}