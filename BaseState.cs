using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AgentTransitionEntry
{
    public BaseState.StateType stateType;
    public Transition transition;
}

[CreateAssetMenu]
[System.Serializable]
public abstract class BaseState : ScriptableObject
{
    public enum StateType
    {
        IdleState,
        PatrolState,
        PursuitState,
        FleeState,
        AttackState,
        FollowState,
        InvestigateState,
    }

    [SerializeField]
    private AgentTransitionEntry[] TransitionPairs;

    [SerializeField]
    protected StateType type;

    public StateType stateType { get { return type; } }

    protected NavMeshAgent navMeshAgent { get; set; }
    public virtual void OnEnter(NavMeshAgent navmeshAgent)
    {
        navMeshAgent = navmeshAgent;
    }

    public AgentTransitionEntry CheckTransitions()
    {
        foreach (AgentTransitionEntry agentTransition in TransitionPairs)
        {
            Agent agent;
            if (navMeshAgent.TryGetComponent<Agent>(out agent))
            {
                if (agentTransition.transition.DoesStateTransition(agent))
                {
                    return agentTransition;
                }
            }
            else 
            {
                Debug.LogError("BaseState.CheckTransitions: agent is null");
            }
        }
        return null;
    }

    public abstract void OnExit();
    public abstract void Tick();
}
