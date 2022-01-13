﻿

using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BossStrategySystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        int players = playerEntities.Length;
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);
        var playerRotationGroup = GetComponentDataFromEntity<Rotation>(true);

        Entities.WithoutBurst().ForEach((Entity enemyE, Animator animator, ref BossMovementComponent bossMovementComponent, ref Rotation rotation) =>
        {

            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
            int animation = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointAnimation;
            int strike = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointStrike;
            //if(strike == 0) animator.SetInteger("Animation Type", 0);//animation will be set by Boss Strike System
            int animType = 0;
            if (animation == (int)WayPointAnimation.Attack)
            {
                animType = 1;
            }
            if (strike == 0) animator.SetInteger("Animation Type", animType);


            bool chase = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointChase;
            //if (targetPointBuffer.Length <= 0 || chase == false)
            if (targetPointBuffer.Length <= 0)
                return;

            //if (HasComponent<EffectsComponent>(playerE))
            //{
            //    var effect = GetComponent<EffectsComponent>(playerE);
            //    effect.playEffectType = EffectType.TwoClose;
            //    effect.playEffectAllowed = true;
            //    SetComponent<EffectsComponent>(playerE, effect);
            //}


            Entity playerE = Entity.Null;
            //change to closest
            for (int i = 0; i < players; i++)
            {
                playerE = playerEntities[i];
            }



            var playerMove = GetComponent<Translation>(playerE);
            //var playerRotation = GetComponent<Rotation>(playerE);
            var playerRotation = playerRotationGroup[playerE];
            var playerForward = GetComponent<LocalToWorld>(playerE).Forward;

            var bossTranslation = GetComponent<Translation>(enemyE);
            //float3 targetPositon = move.Value;
            float3 targetPosition = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointPosition;
            if (chase) targetPosition = new float3(playerMove.Value.x, targetPosition.y, playerMove.Value.z);//keep the Y of the waypoint!

            //math.normalize(targetPosition);
            playerForward.y = 0;
            float3 direction = math.normalize(bossTranslation.Value - targetPosition);
            float dist = math.distance(bossTranslation.Value, targetPosition);
            //Debug.Log("bv tp " + bossTranslation.Value + " " + targetPosition);

            //direction.x = 0;
            //direction.y = 0;
            if (dist >= 5)
            {
                quaternion targetRotation = quaternion.LookRotation(direction, math.up());//always face player
                //quaternion targetRotation = rotation.Value * math.right();
                //Debug.Log("dist " + dist);
                float slerpDampTime = bossMovementComponent.RotateSpeed;
                //rotation.Value = targetRotation;
                rotation.Value = math.slerp(rotation.Value, targetRotation, slerpDampTime * Time.DeltaTime);
                //rotation.Value = math.slerp(rotation.Value, playerForward, slerpDampTime * Time.DeltaTime);
            }

            float targetSpeed = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointSpeed;
            float duration = targetPointBuffer[bossMovementComponent.CurrentIndex].duration;
            bossMovementComponent.CurrentWayPointTimer += Time.DeltaTime;
            if (bossMovementComponent.CurrentWayPointTimer >= duration)
            {
                bossMovementComponent.CurrentWayPointTimer = 0;
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
                bossTranslation.Value = bossTranslation.Value + math.normalize(targetPosition - bossTranslation.Value) * Time.DeltaTime * bossMovementComponent.Speed * targetSpeed;
                SetComponent<Translation>(enemyE, bossTranslation);
            }


        }

        ).Run();






        ecb.Playback(EntityManager);
        ecb.Dispose();
        playerEntities.Dispose();
    }



}





