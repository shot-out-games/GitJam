using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(CollisionSystem))]



public class BossAttackerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);




        Entities.WithoutBurst().ForEach(
        (
            Animator animator,
            //HealthBar healthBar,
            in DeadComponent dead,
            in CollisionComponent collisionComponent,
            in Entity entity


        ) =>
        {
           




        }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();


    }

}

