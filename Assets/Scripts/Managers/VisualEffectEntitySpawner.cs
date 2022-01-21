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

public class VisualEffectEntitySpawner : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    //public bool enemyDamaged = false;
    //public bool playerDamaged = true;
    //public float spawnTime = 3;


    public GameObject VisualEffectPrefab;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<VisualEffectEntitySpawnerComponent>(entity,
                new VisualEffectEntitySpawnerComponent()
                {
                    entity = conversionSystem.GetPrimaryEntity(VisualEffectPrefab)
                    //enemyDamaged = enemyDamaged, playerDamaged = playerDamaged,
                    //spawnTime = spawnTime
                }
            );
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
  
        referencedPrefabs.Add(VisualEffectPrefab);
    }

}

