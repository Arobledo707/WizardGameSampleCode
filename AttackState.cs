// AttackState.cs
// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
[CreateAssetMenu]
public class AttackState : BaseState
{
    [SerializeField]
    private float timer = 0.0f;

    private Agent agent;
    private BaseCharacter baseCharacter;
    private NavMeshObstacle navMeshObstacle = null;

    [SerializeField]
    private bool isBeam = false;
    private void Awake()
    {
        type = StateType.AttackState;
    }

    // remove the current path from the navmesh agent, and assign the agent component
    public override void OnEnter(NavMeshAgent navmeshAgent)
    {
        base.OnEnter(navmeshAgent);
        agent = navmeshAgent.GetComponent<Agent>();
        navmeshAgent.ResetPath();

        if (!navmeshAgent.TryGetComponent<BaseCharacter>(out baseCharacter)) 
        {
            Debug.LogError("AttackState: navmeshAgent does not have BaseCharacter Component!");
        }
        navmeshAgent.enabled = false;
        if (navmeshAgent.TryGetComponent<NavMeshObstacle>(out navMeshObstacle)) 
        {
            navMeshObstacle.enabled = true;
        }
    }
    // TODO add effect that is disabled when leaving attackstate
    public override void OnExit()
    {
        if (navMeshObstacle) 
        {
            navMeshObstacle.enabled = false;
        }
        navMeshAgent.enabled = true;
    }

    // Attacks if attack timer is ready and attack if agent is facing and can see the target
    // if agent cannot attack yet then increment the timer and rotate torwards the target
    public override void Tick()
    {
        if (agent.Target != null)
        {
            if (agent.Target.CurrentState == BaseCharacter.State.Dead) 
            {
                agent.Target = null;
                return;
            }
            if (agent.ReadyToAttack && agent.IsFacingTarget() && agent.IsinLineOfSight(agent.Target.transform.position))
            {
                agent.Attack();
                timer = 0.0f;
            }
            else
            {
                RotateToTarget();
                timer += Time.deltaTime;
                if (timer >= agent.AttackCooldown)
                {
                    baseCharacter.ChangeState(BaseCharacter.State.Idle);
                    agent.ReadyToAttack = true;
                }
            }
        }

    }

    // Rotates gameobject to the target
    private void RotateToTarget() 
    {
        Vector3 direction = (agent.Target.transform.position - agent.aiCharacterComponent.ProjectileGameObjectSpawn.transform.position).normalized;
        Quaternion aimRotation = Quaternion.LookRotation(direction);
        agent.aiCharacterComponent.ProjectileGameObjectSpawn.transform.rotation =
            Quaternion.Lerp(agent.aiCharacterComponent.ProjectileGameObjectSpawn.transform.rotation, aimRotation, Time.deltaTime * agent.RotationSpeed);
    }
}
