

using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;



[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
[UpdateAfter(typeof(SandBox.Player.PlayerMoveSystem))]


public class BossStrategySystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        //EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        //NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        //int players = playerEntities.Length;
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);
        var playerRotationGroup = GetComponentDataFromEntity<Rotation>(true);

        Entities.WithoutBurst().WithAll<EnemyComponent>().WithNone<Pause>().ForEach((Entity enemyE, Animator animator, ref BossMovementComponent bossMovementComponent, ref Rotation rotation, in BossStrategyComponent bossStrategyComponent, 
            in DefensiveStrategyComponent defensiveStrategyComponent) =>
        {

            if(HasComponent<EvadeComponent>(enemyE))
            {
                if(GetComponent<EvadeComponent>(enemyE).InEvade == true)
                {
                    return;
                }    
            }

            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];

            int action = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointAction;//for show weapon only - the anim is what  triggers whatever ammo may be used
            var animType = 0;
           

            if (action == (int)WayPointAction.Move)
            {
                animType = 0;
            }
            if (action == (int)WayPointAction.Attack)
            {
                animType = 1;
            }
            if (action == (int)WayPointAction.Fire)
            {
                animType = 2;
            }
         
            animator.SetInteger("Strike", animType);


            bool chase = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointChase;
            //if (targetPointBuffer.Length <= 0 || chase == false)
            if (targetPointBuffer.Length <= 0)
                return;

        


            Entity playerE = defensiveStrategyComponent.closestPlayerEntity;


            if (playerE == Entity.Null) return;


            var playerMove = GetComponent<Translation>(playerE);
            //var playerRotation = GetComponent<Rotation>(playerE);
            var playerRotation = playerRotationGroup[playerE];
            var playerForward = GetComponent<LocalToWorld>(playerE).Forward;

            //var playerMove = GetComponent<Translation>(playerE);
            var bossTranslation = GetComponent<Translation>(enemyE);
            //float3 targetPositon = move.Value;
            float3 targetPosition = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointPosition;
            float3 actionStopDistance = new(0, 1, 5);//may need to add to waypoints


            if (action == (int)WayPointAction.Attack && chase == true)
            {
                targetPosition = new float3(playerMove.Value.x, playerMove.Value.y + actionStopDistance.y, playerMove.Value.z + actionStopDistance.z);
            }
            else if (chase)
            {
                targetPosition = new float3(playerMove.Value.x, targetPosition.y, playerMove.Value.z + bossStrategyComponent.StopDistance);//keep the Y of the waypoint!
            }

            //math.normalize(targetPosition);
            playerForward.y = 0;

            float3 bossXZ = new float3(bossTranslation.Value.x, 0, bossTranslation.Value.z);
            float3 playerXZ = new float3(playerMove.Value.x, 0, playerMove.Value.z);

            float3 direction = math.normalize(playerXZ - bossXZ);

            float dist = math.distance(bossXZ, playerXZ);
            if (dist < 1) direction = -math.forward();//????????????????? 1
            quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());//always face player
            float slerpDampTime = bossMovementComponent.RotateSpeed;
            rotation.Value = math.slerp(rotation.Value, targetRotation.value, slerpDampTime * Time.DeltaTime);

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
                bossMovementComponent.WayPointReached = true;
            }
            else
            {
                //if (dist > bossStrategyComponent.StopDistance)
                //{
                bossTranslation.Value = bossTranslation.Value + math.normalize(targetPosition - bossTranslation.Value) * Time.DeltaTime * bossMovementComponent.Speed * targetSpeed;
                //}
                SetComponent<Translation>(enemyE, bossTranslation);
                bossMovementComponent.WayPointReached = false;
            }


        }

        ).Run();






        ecb.Playback(EntityManager);
        ecb.Dispose();
        //playerEntities.Dispose();
    }



}





