using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    /// <summary>
    /// Model for company data. Is additional to the user account data.
    /// One-to-Many with Vacancy.
    /// Refers to AppUser.
    /// </summary>
    public class Company
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<Vacancy> Vacancies { get; set; } = [];
        public string SelfDescription { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime RegisterDate { get; private set; } = DateTime.UtcNow.Date;
    }
}