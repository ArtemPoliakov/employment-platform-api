using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.VacancyDtos
{
    /// <summary>
    /// Compact dto with the most important preview info
    /// </summary>
    public class VacancyCompactDto
    {
        public string CompanyUserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public float SalaryMin { get; set; } = 0;
        public float SalaryMax { get; set; } = 0;
        public string WorkMode { get; set; } = VacancyWorkModes.NONE.ToString();
        public Guid Id { get; set; }
    }
}