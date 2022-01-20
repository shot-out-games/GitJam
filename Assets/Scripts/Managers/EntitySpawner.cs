using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;



public struct VisualEffectComponent : IComponentData
{
    public Entity entity;
    public bool instantiated;
}
public class EntitySpawner : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject VisualEffectPrefab;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<VisualEffectComponent>(entity,
                new VisualEffectComponent()
                {
                    entity = conversionSystem.GetPrimaryEntity(VisualEffectPrefab)
                }
            );
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(VisualEffectPrefab);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}



public class VisualEffectSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);




        Entities.WithoutBurst().ForEach(
        (
            ref VisualEffectComponent visualEffectComponent,
            in Entity entity

        ) =>
        {
            if (visualEffectComponent.instantiated) return;
            var e = ecb.Instantiate(visualEffectComponent.entity);
            visualEffectComponent.instantiated = true;
            Debug.Log("e " + e);


        }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();


    }

}

