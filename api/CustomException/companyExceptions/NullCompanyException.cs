using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.CustomException.companyExceptions
{
    public class NullCompanyException : Exception
    {
        public NullCompanyException() { }
        public NullCompanyException(string msg) : base(msg) { }
    }
}