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
    public float duration;//n/a
}
//public struct BossWaypointDurationBufferElement : IBufferElementData
//{
  //  public float duration;
//}

public struct BossMovementComponent : IComponentData
{
    public int CurrentIndex;
    public float Speed;
    public bool Repeat;
}




public class BossComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    [SerializeField]
    float BossSpeed = 1;
    [SerializeField]
    bool Repeat = true;
    //public bool matchupClosest = true;
    //public bool leader = false;

    //public float AngleRadians = 180;
    //public float ViewDistanceSQ = 100;

    //public bool View360 = false;

    public List<WayPoint> wayPoints = new List<WayPoint>();
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<BossMovementComponent>(entity, new BossMovementComponent { Speed = BossSpeed, Repeat = Repeat });

        for (int i = 0; i < wayPoints.Count; i++)
        {
            dstManager.AddBuffer<BossWaypointBufferElement>(entity).Add
                (
                    new BossWaypointBufferElement
                    {
                        wayPointPosition = wayPoints[i].targetPosition,
                        wayPointSpeed = wayPoints[i].speed
                    }

                );

            //dstManager.AddBuffer<BossWaypointDurationBufferElement>(entity).Add
            //    (
            //        new BossWaypointDurationBufferElement
            //        {
            //            duration = wayPoints[i].duration
            //        }

            //    );

        }



    }
}
