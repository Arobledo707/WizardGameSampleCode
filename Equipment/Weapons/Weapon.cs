using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : CastableEquipment
{

    [SerializeField]
    private GameObject primaryProjectile;
    [SerializeField]
    private GameObject secondaryProjectile;
    [SerializeField]
    private GameObject tertiaryProjectile;

    protected GameObject PrimaryProjectile { get { return primaryProjectile;} }
    protected GameObject SecondaryProjectile { get { return secondaryProjectile; } }
    protected GameObject TertiaryProjectile { get { return tertiaryProjectile; } }

}
