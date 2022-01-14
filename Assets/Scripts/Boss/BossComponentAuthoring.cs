using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[InternalBufferCapacity(8)]

public struct BossWaypointBufferElement : IBufferElementData
{
    public float3 wayPointPosition;
    public float wayPointSpeed;
    public bool wayPointChase;
    public int wayPointStrike;
    public int wayPointAnimation;
    public float duration;//n/a
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
}

public struct BossStrategyComponent : IComponentData
{
    public bool AimAtPlayer;
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
        dstManager.AddComponentData<BossMovementComponent>(entity, new BossMovementComponent { Speed = BossSpeed, Repeat = Repeat, RotateSpeed = RotateSpeed });
        dstManager.AddComponentData<BossStrategyComponent>(entity, new BossStrategyComponent { AimAtPlayer = AimAtPlayer });
        dstManager.AddComponent<DeadComponent>(entity);
        dstManager.AddComponent<EnemyComponent>(entity);//keep?
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
                        wayPointStrike = (int) wayPoints[i].wayPointWeaponType,
                        wayPointAnimation = (int)wayPoints[i].animation
                        
                    }

                );


        }



    }
}
