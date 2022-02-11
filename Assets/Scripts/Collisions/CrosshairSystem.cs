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
        NativeList<Unity.Physics.RaycastHit> allHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);



        Entities.WithoutBurst().ForEach((Entity entity, ref CrosshairComponent crosshair) =>
        {


            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var actorEntity = actorWeaponAimEntityList[0];
            var actorWeaponAim = GetComponent<ActorWeaponAimComponent>(actorEntity);

            float distance = crosshair.raycastDistance;
            Translation translation = GetComponent<Translation>(entity);

            actorWeaponAim.closetEnemyWeaponTargetPosition = new float3(0, 0, distance);
            float3 xHairPosition = new float3(translation.Value.x, translation.Value.y + 0f, actorWeaponAim.crosshairRaycastTarget.z);
            float3 start = new float3(translation.Value.x, translation.Value.y + 0f, playerTranslation.Value.z - distance/2);
            if (actorWeaponAim.weaponCamera == CameraTypes.TopDown)
            {
                start = new float3(translation.Value.x, actorWeaponAim.closetEnemyWeaponTargetPosition.y, playerTranslation.Value.z);
                xHairPosition = new float3(translation.Value.x, actorWeaponAim.closetEnemyWeaponTargetPosition.y, playerTranslation.Value.z);
            }
            float3 direction = math.normalize(xHairPosition - start);
            float3 end = start + direction * distance;


            start = actorWeaponAim.rayCastStart;
            end = actorWeaponAim.rayCastEnd;


            RaycastInput inputForward = new RaycastInput()
            {
                Start = start,
                End = end,
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)CollisionLayer.Crosshair,
                    CollidesWith = (uint)CollisionLayer.Enemy | (uint)CollisionLayer.Breakable | (uint)CollisionLayer.Obstacle,
                    GroupIndex = 0
                }
            };
            Debug.DrawLine(start, end, Color.green, Time.DeltaTime);
            bool hasHitPoints = collisionWorld.CastRay(inputForward, ref allHits);
            if (hasHitPoints)
            {

                int closest = 0; ;
                double hi = 1;
                for (int i = 0; i < allHits.Length; i++)
                {
                    RaycastHit hitList = allHits[i];
                    //Debug.Log("index " + i + " f " + hitList.Fraction);

                    if (hitList.Fraction < hi)
                    {
                        closest = i;
                        hi = hitList.Fraction;
                    }
                }
                RaycastHit hitForward = allHits[closest];
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitForward.RigidBodyIndex].Entity;

                if (HasComponent<EnemyComponent>(e))
                {
                    actorWeaponAim.crosshairRaycastTarget.z = hitForward.Position.z;
                    if (actorWeaponAim.weaponCamera == CameraTypes.TopDown)
                    {
                        actorWeaponAim.crosshairRaycastTarget.y = hitForward.Position.y;
                    }

                    Debug.Log("hit enemy position ");
                }
                else if (HasComponent<BreakableComponent>(e))
                {
                    actorWeaponAim.crosshairRaycastTarget.z = hitForward.Position.z;
                    if (actorWeaponAim.weaponCamera == CameraTypes.TopDown)
                    {
                        actorWeaponAim.crosshairRaycastTarget.y = hitForward.Position.y;
                    }
                    Debug.Log("hit breakable position ");
                }
                else if (HasComponent<TriggerComponent>(e))
                {
                    actorWeaponAim.crosshairRaycastTarget.z = hitForward.Position.z;
                    if (actorWeaponAim.weaponCamera == CameraTypes.TopDown)
                    {
                        actorWeaponAim.crosshairRaycastTarget.y = hitForward.Position.y;
                    }
                    Debug.Log("hit something ");
                }

                crosshair.targetDelayCounter = 0;

            }
            else
            {
                crosshair.targetDelayCounter += 1;
                if(crosshair.targetDelayCounter > crosshair.targetDelayFrames)
                {
                    actorWeaponAim.crosshairRaycastTarget.z = playerTranslation.Value.z + 100;
                    crosshair.targetDelayCounter = 0;
                }
            }

            SetComponent(actorEntity, actorWeaponAim);





        }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();




    }
}











