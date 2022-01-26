using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;




//[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateInGroup(typeof(TransformSystemGroup))]




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

        EntityQuery actorWeaponAimQuery = GetEntityQuery(ComponentType.ReadOnly<ActorWeaponAimComponent>());//player 0
        NativeArray<Entity> actorWeaponAimEntityList = actorWeaponAimQuery.ToEntityArray(Allocator.Temp);


        float fov = GetComponent<CameraControlsComponent>(cameraEntityList[0]).fov + 0;
        Translation camTranslation = GetComponent<Translation>(cameraEntityList[0]);
        Translation playerTranslation = GetComponent<Translation>(actorWeaponAimEntityList[0]);
        //Debug.Log("cam trans " + camTranslation.Value);
        //NativeList<Unity.Physics.RaycastHit> allHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);



        Entities.WithoutBurst().ForEach((Entity entity, in CrosshairComponent crosshair) =>
        {


            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var actorEntity = actorWeaponAimEntityList[0];
            var actorWeaponAim = GetComponent<ActorWeaponAimComponent>(actorEntity);

            float distance = crosshair.raycastDistance;
            Translation translation = GetComponent<Translation>(entity);

            //float3 xHairPosition = new float3(translation.Value.x, translation.Value.y, camTranslation.Value.z + distance + fov);
            float3 xHairPosition = new float3(translation.Value.x, translation.Value.y, actorWeaponAim.crosshairRaycastTarget.z);
            //float3 start = new float3(translation.Value.x, translation.Value.y, camTranslation.Value.z);
            //float3 start = playerTranslation.Value + new float3(0, 1, 0);
            float3 start = actorWeaponAim.weaponLocation;


            //float3 direction = new float3(0, 0, 1);
            //if (camTranslation.Value.z < 0)
            //{

            //}    
            //float3 direction = math.normalize(xHairPosition - start);
            //if (xHairPosition.z <= 0) xHairPosition.z = camTranslation.Value.z + distance;
            float3 direction = xHairPosition - start;

            //float3 end = start + direction * distance;


            //float3 end = start + direction * distance;
            float3 end = start + direction * distance;
            //end = actorWeaponAim.crosshairRaycastTarget;

            //end.z = camTranslation.Value.z + distance;

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

            //Debug.Log("start " + (int)start.x + " " + (int)start.y + " " + (int)start.z);
            //Debug.Log("end " + end);

            RaycastHit hitForward = new Unity.Physics.RaycastHit();
            Debug.DrawLine(start, end, Color.green, Time.DeltaTime);

            bool hasPointHitForward = collisionWorld.CastRay(inputForward, out hitForward);


            actorWeaponAim.hitPointType = HitPointType.None;

            //bool hasHitPoints = collisionWorld.CastRay(inputForward, ref allHits);


            //if (hasHitPoints)
            if (hasPointHitForward)
            {


                //for (int i = 0; i < allHits.Length; i++)
                //{
                //RaycastHit hitForward = allHits[i];
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitForward.RigidBodyIndex].Entity;

                if (HasComponent<EnemyComponent>(e))
                {
                    actorWeaponAim.crosshairRaycastTarget = hitForward.Position;
                    actorWeaponAim.hitPointType = HitPointType.Enemy;
                    Debug.Log("hit enemy position ");
                }
                else if (HasComponent<BreakableComponent>(e))
                {
                    actorWeaponAim.crosshairRaycastTarget = hitForward.Position;
                    actorWeaponAim.hitPointType = HitPointType.Breakable;
                    Debug.Log("hit breakable position ");
                }
                else
                {
                    actorWeaponAim.crosshairRaycastTarget = hitForward.Position;
                    //Debug.Log("hit something " + hitForward.Position);
                }


                //}



            }

            ecb.SetComponent(actorEntity, actorWeaponAim);





        }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();




    }
}











