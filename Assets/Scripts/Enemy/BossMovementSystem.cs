



using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;


public class BossMovementSystem : SystemBase
{

    //[ReadOnly]
    //public BufferFromEntity<BossWaypointBufferElement> positionBuffer;
    //public BufferFromEntity<BossWaypointDurationBufferElement> durationBuffer;

    protected override void OnUpdate()
    {

        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);


        Entities.WithBurst().WithoutBurst().ForEach((Entity e,  ref BossMovementComponent bossMovementComponent,
            ref Translation bossTranslation) =>


        {
            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[e];
            bool chase = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointChase;
            if (targetPointBuffer.Length <= 0 || chase == true)
                return;


            float3 targetPositon = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointPosition;
            float targetSpeed = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointSpeed;
            if (math.distance(bossTranslation.Value, targetPositon) < .1f)
            {
                if (targetPointBuffer.Length > bossMovementComponent.CurrentIndex + 1)
                {
                    bossMovementComponent.CurrentIndex++;
                }
                else 
                {
                    bossMovementComponent.CurrentIndex = bossMovementComponent.Repeat ? 0 : bossMovementComponent.CurrentIndex;
                }
            }
            else
            {
                bossTranslation.Value =  bossTranslation.Value +  (targetPositon - bossTranslation.Value) * Time.DeltaTime * bossMovementComponent.Speed * targetSpeed;
                //bossTranslation.Value +=  Time.DeltaTime * bossMovementComponent.Speed * targetSpeed;
            }




        }

        ).Run();






        //ecb.Playback(EntityManager);
        //ecb.Dispose();

        //playerEntities.Dispose();

    }

    public float3 GetHeading(float3 begin, float3 destination)
    {
        return math.normalize(destination - begin);
    }



}





