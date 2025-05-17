using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.VacancyDtos
{
    /// <summary>
    /// Basic dto for vacancy data to give away
    /// </summary>
    public class VacancyDto
    {
        public string CompanyUserName { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CandidateDescription { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public float SalaryMin { get; set; } = 0;
        public float SalaryMax { get; set; } = 0;
        public string WorkMode { get; set; } = VacancyWorkModes.NONE.ToString();
        public string LivingConditions { get; set; } = string.Empty;
        public DateTime EditDate { get; set; }
        public DateTime PublishDate { get; set; }
        public string ApplicationStatus { get; set; } = string.Empty;
        public string OfferStatus { get; set; } = string.Empty;
    }
}