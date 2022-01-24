using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System;
using Unity.Collections;
using Unity.Rendering;


[AlwaysUpdateSystem]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
//[UpdateAfter(typeof(DeadSystem))]

public class CleanupSystem : SystemBase
{

    //private EndSimulationEntityCommandBufferSystem ecbSystem;




    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);




        //Clean up ... Move to DestroySystem
        Entities.ForEach
        (
            (Entity e, ref DamageComponent damageComponent) =>
            {
                ecb.RemoveComponent<DamageComponent>(e);
                Debug.Log("destroy damage component");

            }
        ).Run();


        Entities.ForEach
        (
            (Entity e, ref CollisionComponent collisionComponent) =>
            {
                ecb.RemoveComponent<CollisionComponent>(e);
                Debug.Log("destroy collision from ch ef sys");

            }
        ).Run();


        Entities.ForEach
        (
            (Entity e, ref DeadComponent deadComponent) =>
            {
                if (deadComponent.isDead)
                {
                    ecb.RemoveComponent<DeadComponent>(e);
                    Debug.Log("destroy DEAD COMPONENT");
                }

            }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();



    }



}




