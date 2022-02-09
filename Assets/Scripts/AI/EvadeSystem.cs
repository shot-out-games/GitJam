



using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;




//[UpdateAfter(typeof(GunAmmoHandlerSystem))]//gunammohandler makes the  bullet first
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class EvadeSystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);





        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        int players = playerEntities.Length;

        Entities.WithoutBurst().WithAny<EnemyComponent>().ForEach((Entity e, EvadeComponent evade,
                in Translation enemyTranslation, in DefensiveStrategyComponent defensiveStrategy) =>
        {

            for (int i = 0; i < players; i++)
            {
                var playerE = playerEntities[i];
            }

            var attacker = defensiveStrategy.closeBulletEntity;

        }

        ).Run();






        ecb.Playback(EntityManager);
        ecb.Dispose();
        playerEntities.Dispose();
    }



}





