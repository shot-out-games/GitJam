using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[InternalBufferCapacity(8)]

public struct BossAmmoListBuffer : IBufferElementData
{
    public Entity e;
    public LocalToWorld ammoStartLocalToWorld;
    public Translation ammoStartPosition;
    public Rotation ammoStartRotation;
}
//public struct BossWaypointDurationBufferElement : IBufferElementData
//{
//  public float duration;
//}

public struct BossMovementComponent : IComponentData
{
    public int CurrentIndex;
    public float CurrentWayPointTimer;
    public bool CurrentAnimationStarted;
    public float Speed;
    public bool Repeat;
    public int StartStrike;
    public float RotateSpeed;
    public bool WayPointReached;
}

public struct BossStrategyComponent : IComponentData
{
    public bool AimAtPlayer;
}

public struct BossComponent : IComponentData
{

}




public class BossComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    [SerializeField]
    float BossSpeed = 1;
    [SerializeField]
    float RotateSpeed = 90;
    [SerializeField]
    bool Repeat = true;
    [SerializeField]
    bool AimAtPlayer = true;
    public Entity bossEntity;

    public List<WayPoint> wayPoints = new List<WayPoint>();
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<BossMovementComponent>(entity, new BossMovementComponent { WayPointReached = false, Speed = BossSpeed, Repeat = Repeat, RotateSpeed = RotateSpeed });
        dstManager.AddComponentData<BossStrategyComponent>(entity, new BossStrategyComponent { AimAtPlayer = AimAtPlayer });
        dstManager.AddComponent<DeadComponent>(entity);
        dstManager.AddComponent<BossComponent>(entity);
        dstManager.AddComponent<EnemyComponent>(entity);//keep?
        dstManager.AddComponentData(entity, new CheckedComponent());
        bossEntity = entity;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            dstManager.AddBuffer<BossWaypointBufferElement>(entity).Add
                (
                    new BossWaypointBufferElement
                    {
                        wayPointPosition = wayPoints[i].targetPosition,
                        wayPointSpeed = wayPoints[i].speed,
                        wayPointChase = wayPoints[i].chase,
                        duration = wayPoints[i].duration,
                        wayPointStrike = (int)wayPoints[i].wayPointWeaponType,
                        wayPointAnimation = (int)wayPoints[i].animation,
                        weaponListIndex = wayPoints[i].weaponListIndex,
                        ammoListIndex = wayPoints[i].ammoListIndex

                    }

                );


        }



    }
}
