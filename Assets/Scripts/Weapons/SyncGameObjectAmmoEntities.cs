using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Physics.Extensions;
using UnityEngine.Jobs;

[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(BossAmmoHandlerSystem))]
[UpdateAfter(typeof(EndFrameLocalToParentSystem))]


class SynchronizeGameObjectTransformsBossAmmoWeaponEntities : SystemBase
{

    protected override void OnCreate()
    {
        base.OnCreate();

    }

    protected override void OnUpdate()
    {
        BufferFromEntity<BossAmmoListBuffer> ammoList = GetBufferFromEntity<BossAmmoListBuffer>(true);
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);


        Entities.WithoutBurst().ForEach(
            (Entity enemyE, BossAmmoManager bulletManager, ref BossWeaponComponent bossWeaponComponent, in BossMovementComponent bossMovementComponent) =>
            {
                DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
                DynamicBuffer<BossAmmoListBuffer> ammoListBuffer = ammoList[enemyE];
                if (ammoListBuffer.Length <= 0 || bossMovementComponent.CurrentIndex < 0) return;

                int ammoIndex = targetPointBuffer[bossMovementComponent.CurrentIndex].ammoListIndex;

                if (ammoIndex < 0) return;

                bossWeaponComponent.PrimaryAmmo = ammoListBuffer[ammoIndex].e;
                var localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(bulletManager.AmmoPrefabList[ammoIndex].ammoStartLocation.position, bulletManager.AmmoPrefabList[ammoIndex].ammoStartLocation.rotation, Vector3.one)
                };


                //gunComponent.AmmoStartLocalToWorld = localToWorld;
                bossWeaponComponent.AmmoStartPosition.Value = bulletManager.AmmoPrefabList[ammoIndex].ammoStartLocation.position;
                bossWeaponComponent.AmmoStartRotation.Value = bulletManager.AmmoPrefabList[ammoIndex].ammoStartLocation.rotation;
            }
        ).Run();

    }
}



[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(EndFrameLocalToParentSystem))]
//[UpdateAfter(typeof(FollowTriggerComponent))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


class SynchronizeGameObjectTransformsGunEntities : SystemBase
{
    //[NativeDisableParallelForRestriction] private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    //EntityQuery m_Query;

    protected override void OnCreate()
    {
        base.OnCreate();
        //m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //m_Query = GetEntityQuery(new EntityQueryDesc
        //{
        //    All = new ComponentType[]
        //    {
        //        typeof(WeaponComponent),
        //        typeof(Transform),
        //        typeof(LocalToWorld)
        //    }
        //});



    }

    protected override void OnUpdate()
    {
        //var localToWorlds = m_Query.ToComponentDataArrayAsync<LocalToWorld>(Allocator.TempJob, out var jobHandle);
        //var gunComponents = m_Query.ToComponentDataArray<WeaponComponent>(Allocator.TempJob);
        //var localToWorlds = m_Query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

        //var entities = m_Query.ToEntityArray(Allocator.Temp);

        Entities.WithoutBurst().ForEach(
            (BulletManager bulletManager, ref WeaponComponent gunComponent) =>
            {
                var localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(bulletManager.AmmoStartLocation.position, bulletManager.AmmoStartLocation.rotation, Vector3.one)
                };


                gunComponent.AmmoStartLocalToWorld = localToWorld;
                gunComponent.AmmoStartPosition.Value = bulletManager.AmmoStartLocation.position;
                gunComponent.AmmoStartRotation.Value = bulletManager.AmmoStartLocation.rotation;
            }
        ).Run();



        //Dependency.Complete();

        //m_Query.Dispose();
        //trackerComponents.Dispose();
        //localToWorlds.Dispose();
        //jobHandle.Complete();
    }
}




