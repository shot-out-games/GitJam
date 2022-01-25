using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;




[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]



public class CrosshairRaycastSystem : SystemBase
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
        Camera = 1 << 9,
        Crosshair = 1 << 10,
        Breakable = 1 << 11
    }


    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);



        EntityQuery cameraQuery = GetEntityQuery(ComponentType.ReadOnly<CameraControlsComponent>());
        NativeArray<Entity> cameraEntityList = cameraQuery.ToEntityArray(Allocator.Temp);

        EntityQuery actorWeaponAimQuery = GetEntityQuery(ComponentType.ReadOnly<ActorWeaponAimComponent>());
        NativeArray<Entity> actorWeaponAimEntityList = actorWeaponAimQuery.ToEntityArray(Allocator.Temp);

        float fov = GetComponent<CameraControlsComponent>(cameraEntityList[0]).fov + 1;
        Translation camTranslation = GetComponent<Translation>(cameraEntityList[0]);
        //Debug.Log("cam trans " + camTranslation.Value);



        Entities.WithoutBurst().ForEach((Entity entity, in CrosshairComponent crosshair) =>
        {

            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            Translation translation = GetComponent<Translation>(entity);


            float3 start = new float3(translation.Value.x, translation.Value.y, camTranslation.Value.z);
            float3 direction = new float3(0, 0, 1);
            //if (camTranslation.Value.z < 0)
            //{

            //}    
            float distance = crosshair.raycastDistance;
            float3 end = start + direction * distance;

            //PointDistanceInput pointDistanceInput = new PointDistanceInput
            //{
            //    Position = start, 
            //    MaxDistance = distance,//radius
            //    //Filter = CollisionFilter.Default
            //    Filter = new CollisionFilter()
            //    {
            //        BelongsTo = (uint)CollisionLayer.Crosshair,
            //        CollidesWith = (uint)CollisionLayer.Enemy | (uint)CollisionLayer.Breakable,
            //        GroupIndex = 0
            //    }
            //};

            //bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);//around radius 
            //if (hasPointHit)
            //{
            //    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;
            //    if (HasComponent<EnemyComponent>(e))
            //    {
            //        var actorEntity = actorWeaponAimEntityList[0];
            //        var actorWeaponAim = GetComponent<ActorWeaponAimComponent>(actorEntity);
            //        actorWeaponAim.crosshairRaycastTarget = pointHit.Position;
            //        SetComponent(actorEntity, actorWeaponAim);
            //        Debug.Log("hit enemy position RADIUS " + pointHit.Position);

            //    }
            //    if (HasComponent<BreakableComponent>(e))
            //    {
            //        var actorEntity = actorWeaponAimEntityList[0];
            //        var actorWeaponAim = GetComponent<ActorWeaponAimComponent>(actorEntity);
            //        actorWeaponAim.crosshairRaycastTarget = pointHit.Position;
            //        SetComponent(actorEntity, actorWeaponAim);
            //        Debug.Log("hit breakable position RADIUS " + pointHit.Position);
            //    }

            //}



            RaycastInput inputForward = new RaycastInput()
            {
                Start = start,
                End = end,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)CollisionLayer.Crosshair,
                    CollidesWith = (uint)CollisionLayer.Enemy | (uint)CollisionLayer.Breakable,
                    //CollidesWith = (uint)CollisionLayer.Breakable,
                    //CollidesWith = (uint)CollisionLayer.Enemy,
                    GroupIndex = 0
                }
            };

            Debug.Log("start " + (int)start.z);
            Debug.Log("end " + (int)end.z);

            Unity.Physics.RaycastHit hitForward = new Unity.Physics.RaycastHit();
            Debug.DrawRay(start, direction, Color.green, distance);

            bool hasPointHitForward = collisionWorld.CastRay(inputForward, out hitForward);
            var actorEntity = actorWeaponAimEntityList[0];
            var actorWeaponAim = GetComponent<ActorWeaponAimComponent>(actorEntity);
            //actorWeaponAim.crosshairRaycastTarget = end;
            //actorWeaponAim.crosshairRaycastTarget.z = -fov + distance;


            if (hasPointHitForward)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitForward.RigidBodyIndex].Entity;
                if (HasComponent<EnemyComponent>(e))
                {
                    actorWeaponAim.crosshairRaycastTarget = hitForward.Position;
                    Debug.Log("hit enemy position ");
                }
                if (HasComponent<BreakableComponent>(e))
                {
                    actorWeaponAim.crosshairRaycastTarget = hitForward.Position;
                    Debug.Log("hit breakable position ");
                }

            }

            SetComponent(actorEntity, actorWeaponAim);





        }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();




    }
}











