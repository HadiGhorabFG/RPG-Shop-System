using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject shopCanvas;
    
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
        if (shopCanvas.activeSelf)
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
