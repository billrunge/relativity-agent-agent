using System.Collections.Generic;

namespace AgentAgent.Agent
{
    class SpotsPerServerObject
    {
        public int AgentServerArtifactId { get; set; }
        public int Spots { get; set; }
        public List<ResourcePoolObject> resourcePoolObjects {get; set;}

        public SpotsPerServerObject()
        {
            resourcePoolObjects = new List<ResourcePoolObject>();             
        }

    }
}
