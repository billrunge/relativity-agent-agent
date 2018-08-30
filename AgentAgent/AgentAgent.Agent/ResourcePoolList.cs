using System.Collections.Generic;


namespace AgentAgent.Agent
{
    class ResourcePoolList
    {
        public List<ResourcePoolObject> OutputList { get; }

        public ResourcePoolList()
        {
            OutputList = new List<ResourcePoolObject>();
        }

        public void Add(ResourcePoolObject resourcePool)
        {
            OutputList.Add(resourcePool);
        }

    }
}
