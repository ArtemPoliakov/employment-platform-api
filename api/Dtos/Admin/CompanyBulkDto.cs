using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.CompanyDtos;
using api.Dtos.VacancyDtos;

namespace api.Dtos.Admin
{
    public class CompanyBulkDto
    {
        public RegisterDto RegisterDto { get; set; }
        public CreateCompanyDto CreateCompanyDto { get; set; }
        public List<CreateVacancyDto> CreateVacancyDtos { get; set; }
    }
}