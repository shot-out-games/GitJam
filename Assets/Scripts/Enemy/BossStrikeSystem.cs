﻿

using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BossStrikeSystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        int players = playerEntities.Length;
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);

        Entities.WithoutBurst().ForEach((Entity enemyE, Animator animator, ref BossMovementComponent bossMovementComponent) =>
        {

            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
            if (targetPointBuffer.Length <= 0)
                return;
            int strike = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointStrike;
            if (strike > 0)//later set to different numbers for different strikes
            {
                animator.SetInteger("Animation Type", 2);
            }//animation will be set by Boss Strike System

            //Debug.Log("strike " + strike);
            Entity playerE = Entity.Null;
            //change to closest
            for (int i = 0; i < players; i++)
            {
                playerE = playerEntities[i];                
            }




        }

        ).Run();






        ecb.Playback(EntityManager);
        ecb.Dispose();
        playerEntities.Dispose();
    }



}





