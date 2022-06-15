/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_AI_MODULE
namespace BinaryEgo.ToolBox
{
    public class NavMeshUtils
    {
        static void SetAreaWalkable(UnityEngine.AI.NavMeshAgent p_agent, string p_area, bool p_walkable)
        {
            int mask = UnityEngine.AI.NavMesh.GetAreaFromName(p_area);
            if (p_walkable)
            {
                if ((p_agent.areaMask & 1 << mask) != 1 << mask)
                {
                    p_agent.areaMask += 1 << mask;
                }
            }
            else
            {
                if ((p_agent.areaMask & 1 << mask) == 1 << mask)
                {
                    p_agent.areaMask -= 1 << mask;
                }   
            }
        }
    }
}
#endif