using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

namespace SandBox.Player
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(StepPhysicsWorld))]

    public class PlayerDashSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            Entities.WithoutBurst().ForEach(
                (   
                    Entity e,
                    ref Translation t,
                    ref PhysicsVelocity pv,
                    ref PlayerDashComponent playerDash,
                    in InputControllerComponent inputController,
                    in ApplyImpulseComponent apply,
                    in LocalToWorld ltw

                ) =>
                {
                    if(playerDash.DelayTimeTicker > 0)
                    {
                        playerDash.DelayTimeTicker -= dt;
                        return;
                    }
                    else
                    {
                        playerDash.DelayTimeTicker = 0;
                    }

                    if (playerDash.DashTimeTicker == 0 && playerDash.DelayTimeTicker <= 0)
                    {
                        bool rtPressed = inputController.rightTriggerPressed;
                        if (rtPressed)
                        {
                            //t.Value += ltw.Forward * dt * playerDash.power;
                            pv.Linear += math.normalize(ltw.Forward) * playerDash.power;
                            //pv.Linear.y -= apply.NegativeForce;

                            playerDash.DashTimeTicker += dt;
                        }
                    }
                    else if (playerDash.DashTimeTicker < playerDash.dashTime)
                    {
                        //t.Value += ltw.Forward * dt * playerDash.power;
                        pv.Linear += math.normalize(ltw.Forward) * playerDash.power;
                        playerDash.DashTimeTicker += dt;
                    }
                    else if (playerDash.DashTimeTicker >= playerDash.dashTime)
                    {
                        playerDash.DashTimeTicker = 0;
                        playerDash.DelayTimeTicker = playerDash.delayTime;
                    }


                }
            ).Run();

        }



    }
}


