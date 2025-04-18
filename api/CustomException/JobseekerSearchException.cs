using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.CustomException
{
    public class JobseekerSearchException : Exception
    {
        public JobseekerSearchException() { }
        public JobseekerSearchException(string message) : base(message) { }
    }
}