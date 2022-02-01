using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneStaff : Weapon
{
    [SerializeField][Tooltip("Maximum bombs allowed")]
    private int maxBombCount = 5;
    private  List<ArcaneBomb> projectiles;
    private Queue<ArcaneBomb> projQueue;

    protected override void Start() 
    {
        base.Start();
        projQueue = new Queue<ArcaneBomb>(maxBombCount);
        projectiles = new List<ArcaneBomb>(maxBombCount);

        for (int i = 0; i < maxBombCount; i++) 
        {
            ArcaneBomb bomb = Instantiate<GameObject>(PrimaryProjectile, Vector3.zero, Quaternion.identity).GetComponent<ArcaneBomb>();
            bomb.Disable();
            projQueue.Enqueue(bomb);
        }
        Debug.Log(projQueue.Count);
    }
    public override void CastPrimary(Vector3 point)
    {
        if (projQueue.Count > 0)
        {
            ArcaneBomb bomb = projQueue.Dequeue();
            Debug.Log(projQueue.Count);
            bomb.Enable();
            projectiles.Add(bomb);
            bomb.transform.position = point;
            base.CastPrimary(point);
        }
    }

    public override void ReleasePrimary()
    {
        base.ReleasePrimary();
    }
    public override void CastSecondary(Vector3 point)
    {
        if (projectiles.Count > 0)
        {
            for (int i = projectiles.Count - 1; i >= 0; --i)
            {
                ArcaneBomb bomb = projectiles[i];
                bomb.Detonate();
                projectiles.RemoveAt(i);
                projQueue.Enqueue(bomb);
            }
            base.CastSecondary(point);
        }
 
    }

    public override void CastTertiary(RaycastHit rayhit)
    {
        BaseCharacter baseCharac;
        if (rayhit.transform.gameObject.TryGetComponent<BaseCharacter>(out baseCharac)) 
        {
            baseCharac.AddStatusEffect(StatusEffect.EffectType.MindControl);
            base.CastTertiary(rayhit);
        }
    }
}
