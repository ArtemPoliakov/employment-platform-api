using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.VacancyDtos.ElasticDtos
{
    /// <summary>
    /// Stands for ElasticSearch document for vacancy
    /// </summary>
    public class VacancyElasticDto
    {
        public Guid Id { get; set; }

        public string Position { get; set; } = string.Empty;

        public float MinSalary { get; set; } = 0;

        public float MaxSalary { get; set; } = 100_000_000_000;

        public string WorkMode { get; set; } = VacancyWorkModes.NONE.ToString();

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}