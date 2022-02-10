



using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Random = UnityEngine.Random;




//[UpdateAfter(typeof(GunAmmoHandlerSystem))]//gunammohandler makes the  bullet first
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BossStrategySystem))]

public class EvadeSystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        float time = Time.DeltaTime;




        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        int players = playerEntities.Length;

        Entities.WithoutBurst().WithAny<EnemyComponent>().ForEach((Entity e, ref EvadeComponent evade,
                ref Translation enemyTranslation, ref DefensiveStrategyComponent defensiveStrategy) =>
        {

            for (int i = 0; i < players; i++)
            {
                var playerE = playerEntities[i];
            }

            var attacker = defensiveStrategy.closeBulletEntity;
            bool dodge = defensiveStrategy.currentRole == DefensiveRoles.Evade;
            if(dodge == true && evade.EvadeMoveTimer <= evade.evadeMoveTime)
            {
                evade.InEvade = true;
                evade.EvadeMoveTimer += time;
                if(evade.EvadeMoveTimer > evade.evadeMoveTime)
                {
                    evade.evadeMoveTime = evade.randomEvadeMoveTime
                    ? Random.Range(evade.originalEvadeMoveSpeed * .2f, evade.originalEvadeMoveSpeed) : evade.originalEvadeMoveSpeed;
                    int addX = Random.Range(0, 2);
                    float addZ = 0;
                    if (addX == 0)
                    {
                        addX = -1;
                        Debug.Log("addx " + addX);
                    }
                    evade.addX = addX;
                    if (evade.zMovement)
                    {
                        addZ = Random.Range(-1f, 1f);
                        evade.addZ = addZ;
                    }

                    evade.InEvade = false;
                    evade.EvadeMoveTimer = 0;
                    defensiveStrategy.currentRole = DefensiveRoles.None;
                }

            }
            dodge = evade.InEvade;
            if(dodge == true && HasComponent<PhysicsVelocity>(e))
            {
                enemyTranslation.Value.x += evade.addX * time * evade.evadeMoveSpeed;
                enemyTranslation.Value.z += evade.addZ * time * evade.evadeMoveSpeed;

                //Debug.Log("tr val " + enemyTranslation.Value);

            }


        }

        ).Run();






        ecb.Playback(EntityManager);
        ecb.Dispose();
        playerEntities.Dispose();
    }



}





