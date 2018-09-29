using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class takes two input lists: a list of pools, agent types, and desired counts
//and a list of pools, agent servers assigned to those pools, and available spots
//Using this information, it will output a list of Resource Pools, Agent Servers, and allowed spots


namespace AgentAgent.Agent
{
    class GetPoolServerSpots
    {
        //[Resource Pool - Agent Type - Desired Spots]
        private List<AgentsPerPoolObject> _inputAgentsPerPool;

        // [Resource Pool - Agent Server ID - Spots]
        private List<SpotsPerPoolObject> _inputSpotsPerPool;

        //Output [Resource Pool - Agent Server ID - Spots]
        public List<SpotsPerPoolObject> SpotsPerPool { get; set; }

        public GetPoolServerSpots(List<AgentsPerPoolObject> agentsPerPool, List<SpotsPerPoolObject> spotsPerPool)
        {
            _inputAgentsPerPool = agentsPerPool;
            _inputSpotsPerPool = spotsPerPool;
            SpotsPerPool = new List<SpotsPerPoolObject>();
        }

        

    }
}


//two input buckets(lists)

//[Resource Pool - Agent Type - Desired Spots]
//[Resource Pool - Agent Server ID - Spots]

//Output object
//[Resource Pool - Agent Id - Spots]

//Intermediate objects
//[Resoure Pool - Agent Count]

//1.
//- Determine total agent counts required by each resource pool
//--New object[Resource Pool - Agent Count]

//SELECT[ResourcePoolArtifactID],
//SUM([DesiredCount]) AS[TotalDesiredCount]
//FROM[EDDS].[eddsdbo].[PoolAgentTypeCount]
//GROUP BY[ResourcePoolArtifactID]

//2.
//- Loop through each agent server that belong to exactly 1 resource pool
//-- Subtract spots from[Resource Pool - Agent Count]
//-- Add spots to[Resource Pool - AgentServerID - Spots + new spots]
//-- Remove server from[Resource Pool - Agent Server ID - Spots]

//3.
//- Are there any resource pools left that need agents?
//-- If so, loop through all shared agent servers
//--- How many resource pools do they belong to with jobs?
//---- Loop through each resource pool with their share of the share resource pool
//---- If[Resource Pool - AgentServerID - Spots].spots / pools with jobs <= [Resource Pool - Agent Server ID - Spots].spots 
//----- Add spots to[Resource Pool - AgentServerID - Spots + new spots]
//----- Remove server from[Resource Pool - Agent Server ID - Spots]
//---- If[Resource Pool - AgentServerID - Spots + new spots].spots <= [Resource Pool - Agent Server ID - Spots].spots
//-----

