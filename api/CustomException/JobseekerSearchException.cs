using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.CustomException
{
    /// <summary>
    /// Custom exception for jobseeker search in elastic
    /// </summary>
    public class JobseekerSearchException : Exception
    {
        public JobseekerSearchException() { }
        public JobseekerSearchException(string message) : base(message) { }
    }
}