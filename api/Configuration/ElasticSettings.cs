using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Configuration
{
    /// <summary>
    /// Elasticsearch settings
    /// </summary>
    public class ElasticSettings
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}