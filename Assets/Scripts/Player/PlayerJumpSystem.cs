using System;
using SandBox.Player;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using ForceMode = UnityEngine.ForceMode;





namespace SandBox.Player
{


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    //[UpdateBefore(typeof(StepPhysicsWorld))]

    public class PlayerJumpSystem2D : SystemBase
    {
        int frames = 0;

        protected override void OnCreate()
        {
            base.OnCreate();
        }


        protected override void OnUpdate()
        {

            Entities.WithoutBurst().WithNone<Pause>().ForEach(
                (
                    (
                        PlayerJump2D playerJump,
                        ref Translation translation,
                        ref PhysicsVelocity pv,
                        ref ApplyImpulseComponent applyImpulseComponent,
                        ref PlayerJumpComponent playerJumpComponent,
                        in InputControllerComponent inputController,
                        in Entity e

                    ) =>
                    {
                        //double timeButtonPressed = Rewired.ReInput.players.GetPlayer(0).GetButtonTimePressed("FireA");
                        double timeButtonPressed = inputController.buttonTimePressed;
                        int jumpPoints = playerJumpComponent.jumpPoints;

                        float leftStickX = inputController.leftStickX;
                        float leftStickY = inputController.leftStickY;
                        bool button_a_held = inputController.buttonA_held;
                        bool button_a_released = inputController.buttonA_Released;
                        bool button_a = inputController.buttonA_Pressed;

                        if (button_a_released == true)
                        {
                            playerJumpComponent.CancelJump = true;
                        }
                        if (button_a_released == true && playerJumpComponent.DoubleJumpStarted == false && playerJumpComponent.doubleJump)
                        {
                            playerJumpComponent.DoubleJumpAllowed = true;
                            playerJumpComponent.CancelJump = false;
                        }
                        float3 velocity = pv.Linear;

                        float originalJumpFrames = playerJumpComponent.JumpStartFrames;
                        float originalJumpPower = playerJumpComponent.startJumpGravityForce;
                        bool height_II_timer = timeButtonPressed > 0 && timeButtonPressed < playerJumpComponent.JumpStartHeightTwoTime && jumpPoints >= 2;
                        bool height_III_timer = timeButtonPressed >= playerJumpComponent.JumpStartHeightTwoTime
                            && timeButtonPressed < playerJumpComponent.heightThreeTime && jumpPoints == 3;


                        if (applyImpulseComponent.InJump == false)//has touched ground
                        {
                            frames = 0;
                            playerJumpComponent.JumpStage = JumpStages.Ground;
                            playerJumpComponent.CancelJump = false;
                            playerJumpComponent.DoubleJumpStarted = false;
                            playerJumpComponent.DoubleJumpAllowed = false;
                            playerJumpComponent.JumpCount = 0;

                        }
                        if (applyImpulseComponent.Falling)
                        {
                            pv.Linear.y += applyImpulseComponent.NegativeForce;
                            return;
                        }
                        if (button_a && frames == 0)
                        {
                            playerJumpComponent.JumpStartFrames = playerJumpComponent.heightOneFrames;
                            playerJumpComponent.JumpStartHeightTwoTime = playerJumpComponent.heightTwoTime;
                            playerJumpComponent.JumpStartHeightThreeTime = playerJumpComponent.heightThreeTime;
                            applyImpulseComponent.InJump = true;
                            applyImpulseComponent.Grounded = false;
                            applyImpulseComponent.Falling = false;

                            frames = 1;
                            playerJump.GetComponent<Animator>().SetTrigger("JumpStage");
                            //playerJump.GetComponent<Animator>().applyRootMotion = false;
                            playerJumpComponent.JumpStage = JumpStages.JumpStart;
                            velocity = new float3(pv.Linear.x, originalJumpPower, pv.Linear.z);
                            Debug.Log("jump 1");
                        }
                        else if (button_a && playerJumpComponent.DoubleJumpAllowed == true)
                        {
                            playerJumpComponent.JumpStartFrames = playerJumpComponent.doubleHeightOneFrames;
                            playerJumpComponent.JumpStartHeightTwoTime = playerJumpComponent.doubleHeightTwoTime;
                            playerJumpComponent.JumpStartHeightThreeTime = playerJumpComponent.doubleHeightThreeTime;
                            playerJumpComponent.DoubleJumpStarted = true;
                            playerJumpComponent.DoubleJumpAllowed = false;
                            applyImpulseComponent.InJump = true;
                            applyImpulseComponent.Grounded = false;
                            applyImpulseComponent.Falling = false;

                            frames = 1;
                            playerJump.GetComponent<Animator>().SetTrigger("JumpStage");
                            playerJump.GetComponent<Animator>().applyRootMotion = false;
                            playerJumpComponent.JumpStage = JumpStages.JumpStart;
                            velocity = new float3(pv.Linear.x, originalJumpPower * 1, pv.Linear.z);//ADD DBL JUMP FACTOR
                            Debug.Log("jump 2");
                        }
                        else if (frames >= 1 && frames <= originalJumpFrames && applyImpulseComponent.InJump == true &&
                                 applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            frames = frames + 1;
                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }
                        else if (frames > originalJumpFrames && height_II_timer && applyImpulseComponent.InJump == true &&
                                playerJumpComponent.CancelJump == false &&
                                applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            frames = frames + 1;
                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }
                        else if (frames > originalJumpFrames && height_III_timer && applyImpulseComponent.InJump == true &&
                                playerJumpComponent.CancelJump == false &&
                                applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            frames = frames + 1;
                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }

                        pv.Linear = new float3(velocity.x, velocity.y, velocity.z);
                        if (playerJumpComponent.JumpStage != JumpStages.Ground)
                        {
                            pv.Linear.y += applyImpulseComponent.NegativeForce;
                        }





                        if (button_a == true && frames == 1)
                        {
                            AudioSource audioSource = playerJump.audioSource;
                            if (playerJump.audioClip && audioSource)
                            {
                                audioSource.PlayOneShot(playerJump.audioClip);
                            }

                            if (playerJump.ps)
                            {
                                playerJump.ps.transform.SetParent(playerJump.transform);
                                playerJump.ps.Play(true);
                            }
                        }

                    }
                )
            ).Run();

        }


    }
}

