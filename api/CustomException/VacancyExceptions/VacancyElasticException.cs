using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.CustomException.VacancyExceptions
{
    public class VacancyElasticException : Exception
    {
        public VacancyElasticException() { }
        public VacancyElasticException(string message) : base(message) { }
    }
}