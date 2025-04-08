using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models
{
    /// <summary>
    /// Represents job offer model. Stands for the company's offer to a jobseeker for a certain vacancy
    /// </summary>
    public class Offer
    {
        public Guid Id { get; set; }
        public Guid VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
        public Guid JobSeekerId { get; set; }
        public Jobseeker Jobseeker { get; set; }
        public JobApplicationStatuses Status { get; set; } = JobApplicationStatuses.PENDING;
        public string CompanyMessage { get; set; } = string.Empty;
        public string JobSeekerResponse { get; set; } = string.Empty;
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow.Date;
    }
}