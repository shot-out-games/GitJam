using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;






//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class ToggleColliderSystem : SystemBase
{
    protected override void OnCreate()
    {

    }


    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        Entities.WithoutBurst().ForEach((Entity e, PlayerComponent player, ref PlayerDashComponent playerDashComponent) =>
        {
            if (HasComponent<PhysicsCollider>(e))
            {
                var collider = GetComponent<PhysicsCollider>(e);
                playerDashComponent.box = collider.Value;
            }

            if(playerDashComponent.DashTimeTicker > .1 && playerDashComponent.DashTimeTicker < 1)
            {
            
                ecb.RemoveComponent<PhysicsCollider>(e);
            }
            else if(playerDashComponent.DashTimeTicker >= 1)
            {
                ecb.AddComponent<PhysicsCollider>(e, new PhysicsCollider { Value = playerDashComponent.box });
                Debug.Log("cv " + playerDashComponent.box);

            }

        }
        ).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();


    }
}



