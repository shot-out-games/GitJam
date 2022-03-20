using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct DestroyComponent : IComponentData
{
    public int frames;
}



//[UpdateInGroup((typeof(PresentationSystemGroup)))]
[UpdateInGroup(typeof(SimulationSystemGroup))]

public class DestroySystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    protected override void OnUpdate()
    {
        //var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Persistent);



        Entities.WithoutBurst().ForEach((ref DestroyComponent destroyComponent, ref Translation translation, in Entity e) =>
        {
            translation.Value = new float3(0, -50, 0);
            ecb.AddComponent(e, new DisableRendering());//not needed because it doesn't turn off child particle system render
            //if(HasComponent<UseItem1>(e))
            //{
              //  ecb.RemoveComponent<UseItem1>(e);
            //}

            destroyComponent.frames++;
            if (destroyComponent.frames > 0)
            {
                //ecb.RemoveComponent<C>(e);
                ecb.DestroyEntity(e);
            }

        }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();
        // Make sure that the ECB system knows about our job
        //m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }
}
