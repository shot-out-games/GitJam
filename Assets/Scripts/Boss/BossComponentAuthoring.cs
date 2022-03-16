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
    public float StopDistance;
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
    [SerializeField]
    float StopDistance = 5;
    public Entity bossEntity;
    [Header("Misc")]
    [SerializeField]
    bool checkWinCondition = true;
    [SerializeField]
    bool paused = true;


    public List<WayPoint> wayPoints = new List<WayPoint>();
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<BossMovementComponent>(entity, new BossMovementComponent { WayPointReached = false, Speed = BossSpeed, Repeat = Repeat, RotateSpeed = RotateSpeed });
        dstManager.AddComponentData<BossStrategyComponent>(entity, new BossStrategyComponent { AimAtPlayer = AimAtPlayer, StopDistance = StopDistance });
        dstManager.AddComponent<DeadComponent>(entity);
        dstManager.AddComponent<BossComponent>(entity);
        dstManager.AddComponent<EnemyComponent>(entity);//keep?
        dstManager.AddComponentData(entity, new CheckedComponent());


        if (paused == true)
        {
            dstManager.AddComponent<Pause>(entity);
        }

        dstManager.AddComponentData(entity, new StatsComponent()
        {
            shotsFired = 0,
            shotsLanded = 0
        }
       );



        dstManager.AddComponentData(entity, new SkillTreeComponent()
        {
            e = entity,
            availablePoints = 0,
            SpeedPts = 0,
            PowerPts = 0,
            ChinPts = 0,
            baseSpeed = 0,
            CurrentLevel = 1,
            CurrentLevelXp = 0,
            PointsNextLevel = 10

        }

        );




        dstManager.AddComponentData(entity, new WinnerComponent
        {
            active = true,
            goalCounter = 0,
            goalCounterTarget = 0,//ie how many players you have to save - usually zero
            targetReached = false,
            endGameReached = false,
            checkWinCondition = checkWinCondition
        }
    );

        dstManager.AddComponentData(entity,
            new LevelCompleteComponent
            {
                active = true,
                targetReached = false,
                checkWinCondition = checkWinCondition
            }
        );






        bossEntity = entity;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            dstManager.AddBuffer<BossWaypointBufferElement>(entity).Add
                (
                    new BossWaypointBufferElement
                    {
                        wayPointPosition = wayPoints[i].targetPosition + transform.position,
                        wayPointSpeed = wayPoints[i].speed,
                        wayPointChase = wayPoints[i].chase,
                        duration = wayPoints[i].duration,
                        wayPointAction = (int)wayPoints[i].action,
                        //wayPointAnimation = (int)wayPoints[i].animation,
                        weaponListIndex = wayPoints[i].weaponListIndex,
                        ammoListIndex = wayPoints[i].ammoListIndex

                    }

                );


        }



    }
}
