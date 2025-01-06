using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class BaseCharacter : MonoBehaviour
{
    public enum State
    {
        Moving,
        Attacking,
        Idle,
        Stunned,
        Dead
    }

    public enum Team
    {
        Zero,
        One,
        Two,
        Three
    }
    #region variables
    [SerializeField]
    private float movementSpeed = 3.0f;
    private float movementSpeedMultiplier = 1.0f;

    protected NavMeshAgent navMeshAgent;
    protected Rigidbody rigidBody;
    protected Collider objectCollider;
    protected MeshRenderer meshRenderer;

    [SerializeField]
    private float maxHealth = 100.0f;

    [SerializeField]
    protected float currentHealth;

    [SerializeField]
    private Team team;

    [SerializeField]
    protected State state = State.Idle;

    [SerializeField]
    ParticleSystem[] statusEffectParticles = new ParticleSystem[(int)StatusEffect.EffectType.Invalid];

    [SerializeField]
    public UnityEvent deathEvent;

    [SerializeField]
    protected float timeBeforeDelete = 1.0f;

    protected Dictionary<StatusEffect.EffectType, StatusEffect> statusEffectPairs = new Dictionary<StatusEffect.EffectType, StatusEffect>();
    #endregion

    #region properties
    public ParticleSystem[] StatusEffectParticles { get { return statusEffectParticles; } }
    public Dictionary<StatusEffect.EffectType, StatusEffect> StatusEffectPairs { get { return statusEffectPairs; } }
    public State CurrentState { get { return state; } }

    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }

    public Team CurrentTeam { get { return team; } set { team = value; } }

    public NavMeshAgent NavMeshAgentVar { get { return navMeshAgent; } }

    public float MovementSpeedMultiplier
    {
        get { return movementSpeedMultiplier; }
        set
        {
            movementSpeedMultiplier = value;
            navMeshAgent.speed = movementSpeed * movementSpeedMultiplier;
        }
    }
    #endregion

    #region monobehavior
    virtual protected void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = movementSpeed * movementSpeedMultiplier;
        rigidBody = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    virtual protected void Start()
    {
        currentHealth = maxHealth;
    }

    virtual public void FixedUpdate()
    {
        if (state != State.Dead)
        {
            if (navMeshAgent.velocity == Vector3.zero && state != State.Attacking)
            {
                ChangeState(State.Idle);
            }
            else if (navMeshAgent.velocity != Vector3.zero)
            {
                ChangeState(State.Moving);
            }

            var keys = statusEffectPairs.Keys;

            foreach (var key in keys)
            {
                if (statusEffectPairs[key].Tick())
                {
                    if (statusEffectPairs[key].EffectLifeTime >= statusEffectPairs[key].MaxLifeTime)
                    {
                        statusEffectPairs[key].OnEnd(this);
                        //statusEffectPairs.Remove(key);
                    }
                }
            }
        }

    }

    virtual protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BaseCharacter>(out _))
        {
            rigidBody.isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BaseCharacter>(out _))
        {
            //rigidBody.isKinematic = false;
        }
    }
    #endregion
    public void TakeDamage(float damage, StatusEffect.EffectType effectType = StatusEffect.EffectType.Invalid)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }
        if (effectType != StatusEffect.EffectType.Invalid)
        {
            AddStatusEffect(effectType);
        }
    }
    public void AddHealth(float hp)
    {
        currentHealth += hp;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void SetDestination(Vector3 target)
    {
        navMeshAgent.destination = target;
        navMeshAgent.isStopped = false;
        ChangeState(State.Moving);
    }

    virtual public void ChangeState(State newState, bool force = false)
    {
        state = newState;
    }

    public void SetMovementSpeedMultiplier(float multiplier)
    {
        movementSpeedMultiplier = multiplier;
    }

    public void AddStatusEffect(StatusEffect.EffectType effectType)
    {
        if (statusEffectPairs.ContainsKey(effectType))
        {
            statusEffectPairs[effectType].ResetLifeTime();
        }
        else
        {
            StatusEffect effect;
            switch (effectType)
            {
                case StatusEffect.EffectType.Burning:
                    effect = Instantiate(GameDataSingleton.Instance.burningEffect);
                    break;
                case StatusEffect.EffectType.Wet:
                    effect = Instantiate(GameDataSingleton.Instance.wetEffect);
                    break;
                case StatusEffect.EffectType.Stun:
                    effect = Instantiate(GameDataSingleton.Instance.stunEffect);
                    break;
                case StatusEffect.EffectType.MindControl:
                    effect = Instantiate(GameDataSingleton.Instance.friendlyMindControlEffect);
                    break;
                case StatusEffect.EffectType.Poison:
                    effect = Instantiate(GameDataSingleton.Instance.poisonEffect);
                    break;
                case StatusEffect.EffectType.Electric:
                    effect = Instantiate(GameDataSingleton.Instance.electricEffect);
                    break;
                case StatusEffect.EffectType.SpeedBoost:
                    effect = Instantiate(GameDataSingleton.Instance.speedEffect);
                    break;
                case StatusEffect.EffectType.AttackSpeed:
                    effect = Instantiate(GameDataSingleton.Instance.attackEffect);
                    break;
                default:
                    effect = null;
                    break;
            }
            statusEffectPairs.Add(effectType, effect);
        }
        statusEffectPairs[effectType].OnAdd(this);
    }

    virtual protected IEnumerator DelayForDeath()
    {
        yield return new WaitForSeconds(timeBeforeDelete);
    }

    virtual public void Death()
    {
        //Destroy(gameObject);
    }
}
