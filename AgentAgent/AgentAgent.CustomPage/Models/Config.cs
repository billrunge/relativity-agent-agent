using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgentAgent.CustomPage.Models
{
    public class Config
    {
        public int PoolId { get; set; }
        public int ServerAdjustmentFactor { get; set; }
        public bool IgnoreSearchServer { get; set; }
        public bool UseApiCreate { get; set; }
        public bool UseApiDelete { get; set; }
    }
}
 