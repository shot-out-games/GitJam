using SandBox.Player;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using SphereCollider = Unity.Physics.SphereCollider;




[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]



public class ParticleRaycastSystem : SystemBase
{


    private enum CollisionLayer
    {
        Player = 1 << 0,
        Ground = 1 << 1,
        Enemy = 1 << 2,
        WeaponItem = 1 << 3,
        Obstacle = 1 << 4,
        NPC = 1 << 5,
        PowerUp = 1 << 6,
        Stairs = 1 << 7,
        Particle = 1 << 8
    }


    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);



        Entities.WithoutBurst().ForEach((Entity entity,
            ref PhysicsCollider collider, ref Rotation rotation, in Translation translation, in PhysicsVelocity pv, in AmmoComponent ammoComponent) =>
        {

            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;


            float3 start = translation.Value + new float3(0f, 0, 0);
            float3 direction = new float3(0, 0, 0);
            float distance = .4f;
            float3 end = start + direction * distance;



            PointDistanceInput pointDistanceInput = new PointDistanceInput
            {
                Position = start,
                MaxDistance = distance,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)CollisionLayer.Particle,
                    CollidesWith = (uint)CollisionLayer.Ground,
                    GroupIndex = 0
                }
            };

            bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);//bump left / right n/a

            start = translation.Value + new float3(0, -.5f, 0);
            direction = new float3(0, -1, 0);
            distance = .6f;
            end = start + direction * distance;


            RaycastInput inputDown = new RaycastInput()
            {
                Start = start,
                End = end,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)CollisionLayer.Particle,
                    CollidesWith = (uint)CollisionLayer.Ground,
                    GroupIndex = 0
                }
            };
            Unity.Physics.RaycastHit hitDown = new Unity.Physics.RaycastHit();
            Debug.DrawRay(inputDown.Start, direction, Color.white, distance);

            bool hasPointHitDown = collisionWorld.CastRay(inputDown, out hitDown);


            start = translation.Value + new float3(0, 1f, 0);
            direction = new float3(0, 1f, 0);
            distance = .20f;
            end = start + direction * distance;

            RaycastInput inputUp = new RaycastInput()
            {
                Start = start,
                End = end,
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)CollisionLayer.Particle,
                    CollidesWith = (uint)CollisionLayer.Ground,
                    GroupIndex = 0
                }

            };

            Unity.Physics.RaycastHit hitUp = new Unity.Physics.RaycastHit();

            bool hasPointHitUp = collisionWorld.CastRay(inputUp, out hitUp);


            //Debug.Log("hit fwd " + hasPointHit);
            //Debug.Log("hit up " + hasPointHitUp);
            Debug.Log("hit dwn pre " + hasPointHitDown);

            //hasPointHit = false; 
            if (hasPointHit)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;



            }
            else if (hasPointHitDown)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitDown.RigidBodyIndex].Entity;

                //Debug.Log("hit dwn b4 " + hasPointHitDown);

                if (HasComponent<VisualEffectEntitySpawnerComponent>(entity))
                {


                    var visualEffectComponentSpawner = GetComponent<VisualEffectEntitySpawnerComponent>(entity);
                    if (visualEffectComponentSpawner.instantiated == false)
                    {
                        Debug.Log("hit dwn " + hasPointHitDown);
                        visualEffectComponentSpawner.instantiated = true;
                        SetComponent<VisualEffectEntitySpawnerComponent>(entity, visualEffectComponentSpawner);
                        var spawn = ecb.Instantiate(visualEffectComponentSpawner.entity);
                        ecb.SetComponent<Translation>(spawn, new Translation { Value = hitDown.Position });//spawn visual effect component entity 
                        //if (HasComponent<VisualEffectEntityComponent>(spawn) == true)
                        //{
                          //  Debug.Log("hit dwn 0 " + hasPointHitDown);
                            //    var visualEffectComponent =  GetComponent<VisualEffectEntityComponent>(spawn);
                            //    visualEffectComponent.trigger = true;
                            //    SetComponent<VisualEffectEntityComponent>(spawn, visualEffectComponent);
                        //}
                    }


                }

            }
            else if (hasPointHitUp)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitUp.RigidBodyIndex].Entity;



            }








        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();


    }
}











