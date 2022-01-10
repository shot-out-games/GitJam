using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[InternalBufferCapacity(8)]

[Serializable]
public struct BossWaypointBufferElement : IBufferElementData
{
    public float3 wayPointPosition;
}




public class BossComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    //public bool matchupClosest = true;
    //public bool leader = false;

    //public float AngleRadians = 180;
    //public float ViewDistanceSQ = 100;

    //public bool View360 = false;

    public List<WayPoint> wayPoints = new List<WayPoint>();
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        dstManager.AddBuffer<BossWaypointBufferElement>(entity).Add
            (
                new BossWaypointBufferElement 
                { 
                    wayPointPosition = wayPoints[0].targetPosition 
                }

            );


    }
}
