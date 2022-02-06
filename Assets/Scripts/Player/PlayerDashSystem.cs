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
    [UpdateAfter(typeof(PlayerJumpSystem2D))]

    public class PlayerDashSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            Entities.WithoutBurst().ForEach(
                (
                    Entity e,
                    ref Translation t,
                    ref PlayerDashComponent playerDash,
                    in InputControllerComponent inputController,
                    in ApplyImpulseComponent apply,
                    in LocalToWorld ltw,
                    in Animator animator,
                    in PlayerDashAuthoring player

                ) =>
                {
                    AudioSource audioSource = player.audioSource;
                    if (playerDash.DelayTimeTicker > 0)
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
                            //pv.Linear += ltw.Forward * playerDash.power;
                            playerDash.DashTimeTicker += dt;
                            if (animator.GetInteger("Dash") == 0)
                            {
                                animator.SetInteger("Dash", 1);
                            }

                            if (player.clip && audioSource)
                            {
                                if (audioSource.isPlaying == false)
                                {
                                    audioSource.clip = player.clip;
                                    audioSource.Play();

                                }

                            }

                            if (player.ps)
                            {
                                if (player.ps.isPlaying == false)
                                {
                                    player.ps.transform.SetParent(player.transform);
                                    player.ps.Play(true);
                                }
                            }



                        }
                    }
                    else if (playerDash.DashTimeTicker < playerDash.dashTime)
                    {
                        var pv = new PhysicsVelocity();

                        //Debug.Log("fw  " + ltw.Forward);
                        //t.Value += ltw.Forward * dt * playerDash.power;
                        pv.Linear = ltw.Forward * playerDash.power;
                        playerDash.DashTimeTicker += dt;
                        SetComponent(e, pv);
                    }
                    else if (playerDash.DashTimeTicker >= playerDash.dashTime)
                    {
                        playerDash.DashTimeTicker = 0;
                        playerDash.DelayTimeTicker = playerDash.delayTime;
                        animator.SetInteger("Dash", 0);
                        if (audioSource != null) audioSource.Stop();
                        if (player.ps != null) player.ps.Stop();

                    }


                }
            ).Run();

        }



    }
}


