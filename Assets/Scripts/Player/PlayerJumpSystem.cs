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
                        double timeButtonPressed = Rewired.ReInput.players.GetPlayer(0).GetButtonTimePressed("FireA");
                        int jumpPoints = playerJumpComponent.jumpPoints;
                        bool height_II_timer = timeButtonPressed > 0 && timeButtonPressed < playerJumpComponent.heightTwoTime && jumpPoints >= 2;
                        bool height_III_timer = timeButtonPressed >= playerJumpComponent.heightTwoTime
                            && timeButtonPressed < playerJumpComponent.heightThreeTime && jumpPoints == 3;





                        float leftStickX = inputController.leftStickX;
                        float leftStickY = inputController.leftStickY;

                        bool button_a = inputController.buttonA_Pressed;
                        bool button_a_held = inputController.buttonA_held;
                        bool button_a_released = inputController.buttonA_Released;

                        //bool button_a = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("FireA");
                        //bool button_a_released = Rewired.ReInput.players.GetPlayer(0).GetButtonUp("FireA");
                        //bool button_a_held = Rewired.ReInput.players.GetPlayer(0).GetButton("FireA");
                        //bool buttonPrev = Rewired.ReInput.players.GetPlayer(0).GetButtonPrev("FireA");

                        if (button_a_released == true)
                        {
                            playerJumpComponent.CancelJump = true;
                        }
                        if (button_a_released == true && playerJumpComponent.DoubleJumpStarted == false && playerJumpComponent.doubleJump)
                        {
                            Debug.Log("release");
                            playerJumpComponent.DoubleJumpAllowed = true;
                            playerJumpComponent.CancelJump = false;
                        }


                        float3 velocity = pv.Linear;

                        float gameToDefaultJumpForce = playerJumpComponent.gameStartJumpGravityForce /
                                                       playerJumpComponent.startJumpGravityForce;

                    
                        float originalJumpFrames = playerJumpComponent.DoubleJumpStarted  ? playerJumpComponent.doubleHeightOneFrames : playerJumpComponent.heightOneFrames;

                        float jumpPower = playerJumpComponent.gameStartJumpGravityForce;
                        float originalJumpPower = playerJumpComponent.startJumpGravityForce;

                        float standardJumpHeight =
                            originalJumpPower * originalJumpFrames; //total height of jump at peak - ref only

                        if (applyImpulseComponent.InJump == false)//has touched ground
                        {
                            frames = 0;
                            playerJumpComponent.JumpStage = JumpStages.Ground;
                            playerJumpComponent.CancelJump = false;
                            playerJumpComponent.DoubleJumpStarted = false;
                            playerJumpComponent.DoubleJumpAllowed = false;
                        }

                        if (applyImpulseComponent.Falling)
                        {
                            pv.Linear.y += applyImpulseComponent.NegativeForce;
                            return;
                        }


                        if (button_a && frames == 0)
                        {
                            Debug.Log("first");
                            applyImpulseComponent.InJump = true;
                            applyImpulseComponent.Grounded = false;
                            applyImpulseComponent.Falling = false;

                            frames = 1;
                            playerJump.GetComponent<Animator>().SetTrigger("JumpStage");
                            playerJump.GetComponent<Animator>().applyRootMotion = false;
                            playerJumpComponent.JumpStage = JumpStages.JumpStart;
                            velocity = new float3(pv.Linear.x, originalJumpPower, pv.Linear.z);
                        }
                        else if (button_a && playerJumpComponent.DoubleJumpAllowed == true)
                        {
                            Debug.Log("hd");
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
                        }
                        else if (frames >= 1 && frames <= originalJumpFrames && applyImpulseComponent.InJump == true &&
                                 applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            Debug.Log("h1 " + originalJumpFrames);
                            frames = frames + 1;
                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }
                        else if (frames > originalJumpFrames && height_II_timer && applyImpulseComponent.InJump == true &&
                                playerJumpComponent.CancelJump == false &&
                                applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            Debug.Log("h2");
                            frames = frames + 1;
                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }
                        else if (frames > originalJumpFrames && height_III_timer && applyImpulseComponent.InJump == true &&
                                playerJumpComponent.CancelJump == false &&
                                applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            Debug.Log("h3");
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

