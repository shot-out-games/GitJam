using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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


    //public GameObject ParticlePrefab;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<VisualEffectEntityComponent>(entity,
                new VisualEffectEntityComponent
                {
                    //entity = conversionSystem.GetPrimaryEntity(ParticlePrefab)
                    enemyDamaged = enemyDamaged, playerDamaged = playerDamaged,
                    spawnTime = spawnTime
                }
            );
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        //referencedPrefabs.Add(ParticlePrefab);
    }

}

