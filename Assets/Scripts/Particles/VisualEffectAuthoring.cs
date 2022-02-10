using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.VFX;

//public Entity entity;
//public bool enemyDamaged;
//public bool playerDamaged;
//public bool instantiated;
//public bool trigger;

public class VisualEffectAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public bool enemyDamaged = false;
    public bool playerDamaged = true;
    public float spawnTime = 3;
    public float damageAmount = 1;
    public float framesToSkip = 12;
    [Header("Effects Index")]
    public int effectsIndex = 0;
    public int deathBlowEffectsIndex;
    public float destroyCountdown = 2;




    //public GameObject ParticlePrefab;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<VisualEffectEntityComponent>(entity,
                new VisualEffectEntityComponent
                {
                    //entity = conversionSystem.GetPrimaryEntity(ParticlePrefab)
                    enemyDamaged = enemyDamaged, playerDamaged = playerDamaged, damageAmount = damageAmount,
                    spawnTime = spawnTime,
                    framesToSkip = framesToSkip,
                    effectsIndex = effectsIndex,
                    deathBlowEffectsIndex = deathBlowEffectsIndex,
                    destroyCountdown = destroyCountdown
                }
            );
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        //referencedPrefabs.Add(ParticlePrefab);
    }

}

