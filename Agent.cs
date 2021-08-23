//Agent.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Agent : MonoBehaviour
{
    private NavMeshAgent navmeshAgent;
    private AiCharacter aiCharacter;
    private BaseCharacter target = null;
    private BaseCharacter leader = null;
    private Vector3 lastSeenTargetLocation = Vector3.zero;

    [SerializeField]
    private BaseState idleState;
    [SerializeField]
    private BaseState patrolState;
    [SerializeField]
    private BaseState pursuitState;
    [SerializeField]
    private BaseState fleeState;
    [SerializeField]
    private BaseState attackState;
    [SerializeField]
    private BaseState followState;
    [SerializeField]
    private BaseState investigateState;


    [SerializeField]
    private BaseState currentState;

    [SerializeField]
    private BaseState initialState;

    private BaseState.StateType previousStateType;

    [SerializeField]
    private Collider visionArea;

    [SerializeField]
    private float attackRange = 1.0f;

    [SerializeField]
    private float followLeaderRange = 10.0f;

    [SerializeField]
    private float attackCooldown = 1.0f;

    [SerializeField]
    private float rotationSpeed = 10.0f;


    [SerializeField]
    [Tooltip("Normalized value")]
    private float attackAngle = 0.75f;
    private bool attackReady = true;

    private Rigidbody rigidBody;
    private GameObject projectileSpawnObject;

    public BaseState.StateType PreviousStateType { get { return previousStateType; } }
    public float AttackRange { get { return attackRange; } }
    public float FollowLeaderRange { get { return followLeaderRange; } }
    public BaseCharacter Target { get { return target; } set { target = value; } }
    public BaseCharacter Leader { get { return leader; } set { leader = value; } }
    public AiCharacter aiCharacterComponent { get { return aiCharacter; } }
    public bool ReadyToAttack { get { return attackReady; } set { attackReady = value; } }
    public float AttackCooldown { get { return attackCooldown; } }

    // need to ensure this is only used to read state and not modify state somehow
    public BaseState CurrentState { get { return currentState; } }

    public float RotationSpeed { get { return rotationSpeed; } }

    public Vector3 LastKnownTargetLocation { get { return lastSeenTargetLocation; } }

    public bool IsFacingTarget()
    {
        float dotProduct = Vector3.Dot(projectileSpawnObject.transform.forward, (target.transform.position - projectileSpawnObject.transform.position).normalized);
        return dotProduct > attackAngle;
    }

    //check all objects from agent on path to target
    // if object is closer to target and is an obstacle then there is no line of sight
    public bool IsinLineOfSight(Vector3 position)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(projectileSpawnObject.transform.position, projectileSpawnObject.transform.TransformDirection(Vector3.forward), attackRange);
        float distanceToTarget = Vector3.Distance(projectileSpawnObject.transform.position, position);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.CompareTag(Constants.kObstacleTag))
            {
                float distance = Vector3.Distance(gameObject.transform.position, hit.point);
                if (distance < distanceToTarget)
                {
                    return false;
                }
            }
        }
        return true;
    }

    #region Monobehavior
    private void Awake()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        aiCharacter = GetComponent<AiCharacter>();
        projectileSpawnObject = aiCharacter.ProjectileGameObjectSpawn;
    }

    private void Start()
    {
        currentState = CreateAgentState(initialState.stateType);
        currentState.OnEnter(navmeshAgent);
    }
    private void FixedUpdate()
    {
        if (!SceneManagerSingleton.Instance.IsGamePaused)
        {
            if (currentState != null)
            {
                currentState.Tick();
            }

            AgentTransitionEntry entry = currentState.CheckTransitions();
            if (entry != null)
            {
                currentState.OnExit();
                previousStateType = currentState.stateType;
                currentState = CreateAgentState(entry.stateType);
                currentState.OnEnter(navmeshAgent);
            }
        }
    }

    #endregion

    public void Attack()
    {
        aiCharacter.ChangeState(BaseCharacter.State.Attacking, true);
        aiCharacter.Attack();
        attackReady = false;
    }


    public BaseState CreateAgentState(BaseState.StateType st)
    {
        BaseState state = null;
        switch (st)
        {
            case BaseState.StateType.IdleState:
                if (idleState)
                {
                    state = Instantiate(idleState);
                }
                else
                {
                    Debug.LogError("CreateAgentState Failed: IdleState was not created.  idleState needs to be assigned");
                }
                break;
            case BaseState.StateType.PatrolState:
                if (patrolState)
                {
                    state = Instantiate(patrolState);
                }
                else
                {
                    Debug.LogError("CreateAgentState Failed: PatrolState was not created.  patrolState needs to be assigned");

                }
                break;
            case BaseState.StateType.PursuitState:
                if (pursuitState)
                {
                    state = Instantiate(pursuitState);
                }
                else
                {
                    Debug.LogError("CreateAgentState Failed: PursuitState was not created.  pursuitState needs to be assigned");

                }
                break;
            case BaseState.StateType.FleeState:
                if (fleeState)
                {
                    state = Instantiate(fleeState);
                }
                else
                {
                    Debug.LogError("CreateAgentState Failed: FleeState was not created.  fleeState needs to be assigned");
                }
                break;
            case BaseState.StateType.AttackState:
                if (attackState)
                {
                    state = Instantiate(attackState);
                }
                else
                {
                    Debug.LogError("CreateAgentState Failed: AttackState was not created.  attackState needs to be assigned");
                }
                break;
            case BaseState.StateType.FollowState:
                if (followState)
                {
                    state = Instantiate(followState);
                }
                else
                {
                    Debug.LogError("CreateAgentState Failed: FollowState was not created.  followState needs to be assigned");
                }
                break;
            case BaseState.StateType.InvestigateState:
                if (investigateState)
                {
                    state = Instantiate(investigateState);
                }
                else 
                {
                    Debug.LogError("CreateAgentState Failed: InvestigateState was not created.  investigateState needs to be assigned");

                }
                break;
            default:
                state = null;
                Debug.LogError("BaseState: state is null");
                break;
        }
        return state;
    }

}
