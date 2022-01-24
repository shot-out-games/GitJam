using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct DeadComponent : IComponentData
{
    public bool isDead;
    //public bool isDying;
    public bool playDeadEffects;
    //public bool justDead;
    //public int dieLevel;
    public int tag;
    public bool checkLossCondition;
    public int effectsIndex;
}


[UpdateBefore(typeof(BasicWinnerSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

public class DeadSystem : SystemBase //really game over system currently
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        int currentLevel = LevelManager.instance.currentLevelCompleted;
        //Debug.Log("cur levl " + currentLevel);
        //bool levelComplete = LevelManager.instance.levelSettings[currentLevel].completed;

        Entities.WithoutBurst().WithAll<PlayerComponent>().ForEach
        (
            (ref DeadComponent deadComponent, in Entity entity, in Animator animator) =>
            {

                if (deadComponent.isDead)//player
                {
                    Debug.Log("Player Dead");

                    LevelManager.instance.levelSettings[currentLevel].playersDead += 1;
                    ecb.RemoveComponent<DeadComponent>(entity);
                }
            }
        ).Run();






        bool enemyJustDead = false;

        Entities.WithoutBurst().WithAll<EnemyComponent>().ForEach
        (
            (Animator animator, ref DeadComponent deadComponent,
            in Entity entity) =>
            {
                if (deadComponent.isDead == true)
                {
                    enemyJustDead = true;
                    LevelManager.instance.levelSettings[currentLevel].enemiesDead += 1;
                    Debug.Log("set dead");
                    ecb.RemoveComponent<DeadComponent>(entity);
                }


                if (HasComponent<WinnerComponent>(entity))
                {
                    var winnerComponent = GetComponent<WinnerComponent>(entity);
                    if (winnerComponent.checkWinCondition == true)//this  (and all with this true) enemy must be defeated to win the game
                    {
                        winnerComponent.endGameReached = true;
                        SetComponent<WinnerComponent>(entity, winnerComponent);
                    }

                }

                // animator.SetInteger("Dead", 5);

            }
        ).Run();




        if (enemyJustDead == true)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in ShowMessageMenuComponent messageMenuComponent, in ShowMessageMenuGroup messageMenu) =>
                {
                    messageMenu.messageString = "... Destroyed ... ";
                    messageMenu.ShowMenu();
                }
            ).Run();


            //add bonuses for defeating enemies here

        }


        ecb.Playback(EntityManager);
        ecb.Dispose();

        //return default;
    }

}



