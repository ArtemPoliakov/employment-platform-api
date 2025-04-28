using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.CompanyDtos
{
    /// <summary>
    /// Basic dto for company data to give away
    /// </summary>
    public class CompanyDto
    {
        public string UserName { get; set; } = string.Empty;
        public string SelfDescription { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }
        public string AppUserId { get; set; } = string.Empty;
    }
}