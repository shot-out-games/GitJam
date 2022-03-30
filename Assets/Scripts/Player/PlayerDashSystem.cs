using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;

namespace SandBox.Player
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerJumpSystem2D))]

    public class PlayerDashSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

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
                    if (playerDash.active == false) return;
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
                        bool rtPressed = inputController.leftTriggerPressed;
                        if (rtPressed)
                        {
                            //t.Value += ltw.Forward * dt * playerDash.power;
                            //pv.Linear += ltw.Forward * playerDash.power;
                            playerDash.DashTimeTicker += dt;
                            if (animator.GetInteger("Dash") == 0)
                            {
                                animator.SetInteger("Dash", 1);
                                playerDash.Collider = GetComponent<PhysicsCollider>(e);
                                playerDash.InDash = true;

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
                        playerDash.InDash = true;

                        //Debug.Log("fw  " + ltw.Forward);
                        //t.Value += ltw.Forward * dt * playerDash.power;
                        pv.Linear = ltw.Forward * playerDash.power;
                        playerDash.DashTimeTicker += dt;
                        SetComponent(e, pv);
                    }
                    else if (playerDash.DashTimeTicker >= playerDash.dashTime)
                    {
                        //playerDash.colliderAdded = true;
                        //playerDash.colliderRemoved = false;
                        //if(HasComponent<PhysicsCollider>(e) == false)
                        //{
                            //var collider = GetComponent<PhysicsCollider>(e);
                            //playerDash.box = collider.Value;
                        //}    
                        playerDash.DashTimeTicker = 0;
                        playerDash.DelayTimeTicker = playerDash.delayTime;
                        animator.SetInteger("Dash", 0);
                        playerDash.InDash = false;
                        if (audioSource != null) audioSource.Stop();
                        if (player.ps != null) player.ps.Stop();

                    }


                }
            ).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();


        }



    }
}


