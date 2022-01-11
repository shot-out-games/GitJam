

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

        Entities.WithoutBurst().ForEach((Entity enemyE, Animator animator, ref BossMovementComponent bossMovementComponent) =>
        {

            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
            int strike = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointStrike;
            if(strike == 0) animator.SetInteger("Animation Type", 0);//animation will be set by Boss Strike System
            bool chase = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointChase;
            if (targetPointBuffer.Length <= 0 || chase == false)
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


            var move = GetComponent<Translation>(playerE);
            var bossTranslation = GetComponent<Translation>(enemyE);
            float3 targetPositon = move.Value;
            float targetSpeed = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointSpeed;
            float duration = targetPointBuffer[bossMovementComponent.CurrentIndex].duration;
            int animation = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointAnimation;
            int animType = 0;
            if(animation == (int)WayPointAnimation.Attack)
            {
                animType = 1;
            }
            if(strike == 0) animator.SetInteger("Animation Type", animType);
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
                bossTranslation.Value = bossTranslation.Value + (targetPositon - bossTranslation.Value) * Time.DeltaTime * bossMovementComponent.Speed * targetSpeed;
                SetComponent<Translation>(enemyE, bossTranslation);
            }


        }

        ).Run();






        ecb.Playback(EntityManager);
        ecb.Dispose();
        playerEntities.Dispose();
    }



}





