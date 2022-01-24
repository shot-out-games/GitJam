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



public class CameraRaycastSystem : SystemBase
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
        Particle = 1 << 8,
        Camera = 1 << 9
    }


    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);





        Entities.WithoutBurst().ForEach((Entity entity, CameraControlsComponent cameraControls) =>
        {

            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            Translation translation = GetComponent<Translation>(entity);
            var parentEntity = GetComponent<TriggerComponent>(entity).ParentEntity;
           


            //float3 start = translation.Value;
            //float3 direction = new float3(0, 0, 0);
            //float distance = cameraControls.fov + 100;
            //float3 end = start + direction * distance;



            //PointDistanceInput pointDistanceInput = new PointDistanceInput
            //{
            //    Position = start,
            //    MaxDistance = distance,
            //    //Filter = CollisionFilter.Default
            //    Filter = new CollisionFilter()
            //    {
            //        BelongsTo = (uint)CollisionLayer.Camera,
            //        CollidesWith = (uint)CollisionLayer.Enemy,
            //        GroupIndex = 0
            //    }
            //};



            //bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);//bump left / right n/a


            //Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();


            //if (hasPointHit)
            //{
            //    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;

            //    Debug.Log("hit cam " + hit.Position);


            //}


            //bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);//bump left / right n/a



            float3 start = translation.Value + new float3(0, 0, 0);
            float3 direction = new float3(0, 0, 1);
            float distance = 100 + cameraControls.fov;
            float3 end = start + direction * distance;


            RaycastInput inputForward = new RaycastInput()
            {
                Start = start,
                End = end,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)CollisionLayer.Camera,
                    CollidesWith = (uint)CollisionLayer.Enemy,
                    GroupIndex = 0
                }
            };
            Unity.Physics.RaycastHit hitForward = new Unity.Physics.RaycastHit();
            Debug.DrawRay(inputForward.Start, direction, Color.green, distance);

            bool hasPointHitForward = collisionWorld.CastRay(inputForward, out hitForward);

            if (hasPointHitForward)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitForward.RigidBodyIndex].Entity;
                Debug.Log("hit cam " + hitForward.Position);

            }




        }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();




    }
}











