using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{ 
    public void LogicUpdate(Ray PositionTo, NavMeshAgent agent)
    {
        Vector3 mousePos;
        RaycastHit hit;
        
        if(Physics.Raycast(PositionTo, out hit))
        {
            mousePos = hit.point;
            agent.SetDestination(mousePos);
        }
    }
}
