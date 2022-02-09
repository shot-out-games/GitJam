using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;




[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BasicWinnerSystem))]


public class BasicLoserSystem : SystemBase
{

    protected override void OnUpdate()
    {
        if (LevelManager.instance.gameResult == GameResult.Loser) return;


        bool loser = false;


        Entities.WithAll<PlayerComponent>().WithoutBurst().ForEach
        (
            (in DeadComponent dead) =>
            {
                if (dead.isDead == true)
                {
                    loser = true;
                    Debug.Log("basic loser system");
                }
            }
        ).Run();

   
        if (loser == false) return;

        LevelManager.instance.endGame = true;
        LevelManager.instance.gameResult = GameResult.Loser;

    }
}


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BasicWinnerSystem))]

public class EndGameSystem : SystemBase
{

    StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }


    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        if (LevelManager.instance.gameResult == GameResult.Winner || LevelManager.instance.gameResult == GameResult.Loser)
        {
            bool win = (LevelManager.instance.gameResult == GameResult.Winner);



            Entities.WithoutBurst().WithAny<PlayerComponent, EnemyComponent>().ForEach
            ((in Entity e) =>
                {

                    var velocity = GetComponent<PhysicsVelocity>(e);
                    if (HasComponent<PhysicsVelocity>(e) && win == true && HasComponent<EnemyComponent>(e))
                    {
                        velocity.Linear = new float3(0, 0, 0);
                        ecb.RemoveComponent<EnemyComponent>(e);
                    }
                    else
                    {
                        velocity.Linear = new float3(0, 0, 0);
                    }

                    if (HasComponent<Pause>(e))
                    {
                        ecb.RemoveComponent<Pause>(e);
                    }
                    ecb.SetComponent<PhysicsVelocity>(e, velocity);

                }
            ).Run();


        }

        ecb.Playback(EntityManager);
        ecb.Dispose();


    }
}
