using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models
{
    /// <summary>
    /// Represents job application model. Stands for the jobseeker's application to a vacancy.
    /// </summary>
    public class JobApplication
    {
        public Guid Id { get; set; }
        public Guid VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
        public Guid JobseekerId { get; set; }
        public Jobseeker Jobseeker { get; set; }
        public JobApplicationStatuses Status { get; set; } = JobApplicationStatuses.PENDING;
        public string CompanyResponse { get; set; } = string.Empty;
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow.Date;
    }
}