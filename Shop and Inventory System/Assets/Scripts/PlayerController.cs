using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private AgentMovement agentMovement;

    private enum PlayerState
    {
        moving, shopping
    }

    private PlayerState playerState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agentMovement = GetComponent<AgentMovement>();
        
        playerState = PlayerState.moving;
    }

    private void Update()
    {
        if (UIStorageController.menuOpen)
        {
            playerState = PlayerState.shopping;
        }
        else
        {
            playerState = PlayerState.moving;
        }
        
        if(Input.GetMouseButtonDown(0) && playerState == PlayerState.moving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            agentMovement.LogicUpdate(ray, agent);
        }
    }
}
