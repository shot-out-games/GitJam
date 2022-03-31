using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class NdeMechanicSystem : SystemBase
{

    

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {

        Entities.ForEach(
            (
                Entity e,
                ref ControlBarComponent controlBar,
                ref NdeMechanicComponent ndeMechanic,
                ref RatingsComponent ratings,
                ref PlayerJumpComponent playerJump,
                in HealthComponent health

            ) =>
            {

                float pct = health.TotalDamageReceived / ratings.maxHealth;
                ndeMechanic.multiplier = .5f;
                controlBar.value = pct;
                float f1 = pct * .5f;
                float f2 = pct * .66f;
                ratings.gameSpeed = ratings.speed * (.666f + f2);
                playerJump.gameStartJumpGravityForce = playerJump.startJumpGravityForce * (f1 + .50f);


            }


        ).ScheduleParallel();

        Entities.WithAll<ControlBarComponent, NdeMechanicComponent>().ForEach
        (
            (
                ref WeaponComponent gun,
                in HealthComponent health,
                in RatingsComponent ratings
                ) =>
            {

                float pct = health.TotalDamageReceived / ratings.maxHealth;
                float f1 = pct * .5f;
                //float f2 = pct * .66f;
                gun.gameStrength = gun.Strength * (.50f + f1);
                gun.gameRate =  gun.Rate + gun.Rate * (1 - pct);
                gun.gameDamage = gun.Damage * (.50f + f1);

            }

        ).ScheduleParallel();





    }
}



//using Unity.Entities;

//namespace EconSim
//{
//    public class UpdateDesiredStateSystem : SystemBase
//    {
//        EndSimulationEntityCommandBufferSystem ecbSystem;

//        protected override void OnCreate()
//        {
//            base.OnCreate();
//            ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//        }

//        protected override void OnUpdate()
//        {
//            var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();

//            Entities
//                .ForEach((Entity entity, int entityInQueryIndex, in DesiredStateData desiredState, in TimeRemainingData timeRemaining) =>
//                {
//                    // Check if entity has the IsActiveFlag component
//                    bool isActive = false; //???

//                    // Entity wants to be active
//                    if (desiredState.value)
//                    {
//                        if (!isActive)
//                        {
//                            // add an entry to the ECB to add the IsActiveFlag component
//                            ecb.AddComponent<IsActiveFlag>(entityInQueryIndex, entity);
//                        }
//                    }
//                    // Entity wants to be disabled
//                    else
//                    
//                        if (isActive && timeRemaining.value <= 0)
//                        {
//                            ecb.RemoveComponent<IsActiveFlag>(entityInQueryIndex, entity);
//                        }
//                    }

//                }).ScheduleParallel();

//            ecbSystem.AddJobHandleForProducer(Dependency);
//        }
//    }
//}