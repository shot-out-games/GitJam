using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.VFX;

public struct VisualEffectEntitySpawnerComponent : IComponentData
{
    public Entity entity;
    //public bool enemyDamaged;
    //public bool playerDamaged;
    public bool instantiated;
    //public bool trigger;
    //public float currentTime;
    //public float spawnTime;
}

public struct VisualEffectEntityComponent : IComponentData
{
    //public Entity entity;
    public float damageAmount;
    public bool enemyDamaged;
    public bool playerDamaged;
    public bool instantiated;
    public bool trigger;
    public float currentTime;
    public float spawnTime;
    public bool destroy;
    public float destroyCountdown;
    public float framesToSkip;//timer instead?
    public int frameSkipCounter;
    public int effectsIndex;
    public int deathBlowEffectsIndex;


}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(ParticleRaycastSystem))]

public class VisualEffectSystem : SystemBase
{

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);



        Entities.WithoutBurst().ForEach(
        (
            ref VisualEffectEntityComponent visualEffectComponent,
            in Entity entity,
            in VisualEffect ve

        ) =>
        {
            //Debug.Log("ve" + ve.GetComponent<VisualEffect>());
            //ve.GetComponent<VisualEffect>();
            if (visualEffectComponent.instantiated)
            {
                visualEffectComponent.currentTime += Time.DeltaTime;
                //Debug.Log("visual TIME " + visualEffectComponent.currentTime);
                if (visualEffectComponent.currentTime > visualEffectComponent.spawnTime)
                {
                    //Debug.Log("destroy");
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

        Entities.WithoutBurst().ForEach((Entity e, VisualEffect ve, ref VisualEffectEntityComponent visualEffectComponent) =>
        {
            if (visualEffectComponent.destroy && visualEffectComponent.destroyCountdown > 0)
            {
                ve.SetFloat("Spawn Rate", 0);
                visualEffectComponent.destroyCountdown -= Time.DeltaTime;
                //ecb.DestroyEntity(e);
            }
            else if (visualEffectComponent.destroy && visualEffectComponent.destroyCountdown <= 0)
            {
                //Debug.Log("destroy");
                ecb.DestroyEntity(e);
            }

        }).Run();



        ecb.Playback(EntityManager);
        ecb.Dispose();


    }

}

