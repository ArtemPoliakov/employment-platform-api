using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.CustomException
{
    /// <summary>
    /// Custom exception for jobseeker elastic operations
    /// </summary>
    public class JobseekerElasticException : Exception
    {
        public JobseekerElasticException() { }
        public JobseekerElasticException(string message) : base(message) { }
    }
}