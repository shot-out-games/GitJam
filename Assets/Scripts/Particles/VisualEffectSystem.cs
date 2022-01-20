using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;



public struct VisualEffectComponent : IComponentData
{
    public Entity entity;
    public bool enemyDamaged;
    public bool playerDamaged;
    public bool instantiated;
    public bool trigger;
    public float currentTime;
    public float spawnTime;
}

public struct ParticleSpawnerComponent : IComponentData
{
    public Entity entity;
    public bool enemyDamaged;
    public bool playerDamaged;
    public bool instantiated;
    public bool trigger;
    public float currentTime;
    public float spawnTime;
    public bool destroy;
}

[UpdateAfter(typeof(ParticleRaycastSystem))]

public class VisualEffectSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);




        Entities.WithoutBurst().ForEach(
        (
            ref ParticleSpawnerComponent visualEffectComponent,
            in Entity entity

        ) =>
        {
            if (visualEffectComponent.instantiated)
            {
                visualEffectComponent.currentTime += Time.DeltaTime;
                Debug.Log("visual TIME " + visualEffectComponent.currentTime);
                if (visualEffectComponent.currentTime > visualEffectComponent.spawnTime)
                {
                    Debug.Log("destroy");
                    visualEffectComponent.currentTime = 0;
                    visualEffectComponent.instantiated = false;
                    visualEffectComponent.destroy = true;
                }

            }
            else if (visualEffectComponent.trigger == true)
            {
                //var translation = GetComponent<Translation>(entity);
                //var rotation = GetComponent<Rotation>(entity);
                //var e = ecb.Instantiate(visualEffectComponent.entity);
                //ecb.SetComponent<Translation>(e, translation);
                //ecb.SetComponent<Rotation>(e, rotation);
                visualEffectComponent.instantiated = true;
                visualEffectComponent.trigger = false;
                //Debug.Log("e " + e);
            }


        }
        ).Run();

        Entities.ForEach((Entity e, ParticleSpawnerComponent visualEffectComponent) =>
        {
            if(visualEffectComponent.destroy) ecb.DestroyEntity(e);

        }).Run();



        ecb.Playback(EntityManager);
        ecb.Dispose();


    }

}
