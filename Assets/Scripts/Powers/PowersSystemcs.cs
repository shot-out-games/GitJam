using System.Collections;
using System.Collections.Generic;
using SandBox.Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;



//[UpdateAfter(typeof(PlayerMoveSystem))]

public class PowersSystem : SystemBase
{

    //EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        //m_EndSimulationEcbSystem = World
        //    .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }



    protected override void OnUpdate()
    {

        //var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);


        Entities.WithoutBurst().ForEach((in ParticleSystemComponent ps, in Entity e) =>
            {
                if (!HasComponent<Translation>(ps.pickedUpActor)) return;
                float3 value = GetComponent<Translation>(ps.pickedUpActor).Value;


                if (ps.followActor == false)
                {
                    ecb.DestroyEntity(e);
                    return;
                }


                SetComponent(e, new Translation { Value = value });


            }
        ).Run();





        Entities.WithoutBurst().ForEach(
            (
                    ref Speed speed, ref RatingsComponent ratings,
                        in Entity e


                ) =>
            {

                if (speed.startTimer == false && speed.enabled == true)
                {
                    speed.triggered = true;
                    speed.startTimer = true;
                    speed.timer = 0;
                    //ecb.AddComponent(speed.itemEntity, new DestroyComponent());
                    ratings.gameSpeed = ratings.gameSpeed * speed.multiplier;
                    Debug.Log("game speed " + ratings.gameSpeed);
                }
                else if (speed.enabled && speed.timer < speed.timeOn)
                {
                    speed.triggered = false;
                    speed.timer += Time.DeltaTime;
                }
                else if (speed.enabled)
                {
                    speed.startTimer = false;
                    speed.timer = 0;
                    speed.enabled = false;
                    ratings.gameSpeed = ratings.speed;
                    ecb.DestroyEntity(speed.psAttached);
                    ecb.RemoveComponent<Speed>(e);

                }

            }
        ).Run();






        Entities.ForEach(
            (
                ref HealthPower healthPower, ref HealthComponent healthComponent, in RatingsComponent ratings, in Entity e

            ) =>
            {
                if (healthPower.enabled == true)
                {
                    //healthPower.enabled = false;
                    healthComponent.TotalDamageReceived = healthComponent.TotalDamageReceived * healthPower.healthMultiplier;
                    //Rare used if multiplier is > 1 meaning health damage increased
                    if (healthComponent.TotalDamageReceived > ratings.maxHealth)
                    {
                        healthComponent.TotalDamageReceived = ratings.maxHealth;
                    }
                    ecb.RemoveComponent<HealthPower>(e);
                    ecb.AddComponent(healthPower.itemEntity, new DestroyComponent());
                    ecb.DestroyEntity(healthPower.psAttached);

                }

            }
        ).Schedule();


        Entities.WithoutBurst().ForEach(
            (
                HealthBar healthBar, ref HealthPower healthPower) =>
            {
                if (healthPower.enabled == true)
                {
                    healthPower.enabled = false;
                    healthBar.HealthChange();
                    Debug.Log("Health Changed");
                }
            }
        ).Run();




        //Entities.WithoutBurst().ForEach(
        //    (
        //        ref ControlPower healthPower, ref ControlBarComponent healthComponent

        //    ) =>
        //    {
        //        if (healthPower.enabled == true)
        //        {
        //            healthPower.enabled = false;
        //            healthComponent.value = healthComponent.value * healthPower.controlMultiplier;
        //            //Rare used if multiplier is > 1 meaning health damage increased
        //            if (healthComponent.value > healthComponent.maxHealth)
        //            {
        //                healthComponent.value = healthComponent.maxHealth;
        //            }
        //        }

        //    }
        //).Run();



        Entities.WithoutBurst().WithAll<AudioSourceComponent>().ForEach(
            (
                AudioSource audioSource, PowerSpeedItem powerItem, ref PowerItemComponent powerItemComponent, in Entity e) =>
            {
                if (audioSource.isPlaying == false
                    && powerItemComponent.enabled == true
                )
                {
                    powerItemComponent.enabled = false;
                    //audioSource.PlayOneShot(powerItem.powerEnabledAudioClip);
                }
            }
        ).Run();



        ecb.Playback(EntityManager);
        ecb.Dispose();


        // Make sure that the ECB system knows about our job
        //m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);



    }




}

