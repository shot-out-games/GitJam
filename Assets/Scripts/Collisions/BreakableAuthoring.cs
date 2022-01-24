﻿using UnityEngine;
using Unity.Entities;

public struct BreakableComponent : IComponentData
{
    int value;
    public float damageAmount;
    public bool enemyDamaged;
    public bool playerDamaged;
    public bool instantiated;
    public bool trigger;
    public float currentTime;
    public float spawnTime;
    public bool destroy;
    public float framesToSkip;//timer instead?
    public int frameSkipCounter;
    public int damageEffectsIndex;
    public int deathBlowEffectsIndex;


}

public class BreakableAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{


    public float damageAmount = 1;
    public float framesToSkip = 30;//timer instead?
    public int damageEffectsIndex;
    public int deathBlowEffectsIndex;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //parent needs to be fixed currently self
       // Debug.Log("break " + entity);
        BreakableComponent breakable = new BreakableComponent
        {
            damageAmount = damageAmount, framesToSkip = framesToSkip, damageEffectsIndex = damageEffectsIndex, deathBlowEffectsIndex = deathBlowEffectsIndex
        };

        dstManager.AddComponentData(entity, breakable);
    }


}