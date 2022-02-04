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
    //    public int playerId = 0;
    //    private Rewired.Player player { get { return ReInput.isReady ? ReInput.players.GetPlayer(playerId) : null; } }



    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

    public class PlayerJumpSystem2D : SystemBase
    {
        int frames = 0;
        private int airFrames = 0;
        //Rewired.Player player = Rewired.ReInput.players.GetPlayer(0);
        float buttonHeldFrames = 0;
        //private int fallingFramesCounter = 0;
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

                        float3 velocity = pv.Linear;

                        float gameToDefaultJumpForce = playerJumpComponent.gameStartJumpGravityForce /
                                                       playerJumpComponent.startJumpGravityForce;

                        float jumpFrames =
                            gameToDefaultJumpForce *
                            playerJumpComponent.heightOneFrames; //increases if now jumping higher

                        float originalJumpFrames = playerJumpComponent.heightOneFrames;
                        float jumpPower = playerJumpComponent.gameStartJumpGravityForce;
                        float originalJumpPower = playerJumpComponent.startJumpGravityForce;

                        float standardJumpHeight =
                            originalJumpPower * originalJumpFrames; //total height of jump at peak - ref only

                        if (applyImpulseComponent.InJump == false)
                        {
                            frames = 0;
                            airFrames = 0;
                            playerJumpComponent.JumpStage = JumpStages.Ground;
                            applyImpulseComponent.hiJump = false;

                        }

                        if (applyImpulseComponent.Falling)
                        {
                            pv.Linear.y += applyImpulseComponent.NegativeForce;
                            return;
                        }

                        if ((button_a_held == true) && applyImpulseComponent.InJump == false &&
                            frames == 0)
                        {
                            applyImpulseComponent.InJump = true;
                            applyImpulseComponent.Grounded = false;
                            applyImpulseComponent.Falling = false;
                            applyImpulseComponent.hiJump = false;

                            frames = 1;
                            playerJump.GetComponent<Animator>().SetTrigger("JumpStage");
                            playerJump.GetComponent<Animator>().applyRootMotion = false;
                            playerJumpComponent.JumpStage = JumpStages.JumpStart;
                            velocity = new float3(pv.Linear.x, originalJumpPower, pv.Linear.z);
                        }
                        else if (frames >= 1 && frames <= originalJumpFrames && applyImpulseComponent.InJump == true &&
                                 applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            frames = frames + 1;
                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }
                        else if (frames >= 1 && height_II_timer && applyImpulseComponent.InJump == true &&
                                 applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            frames = frames + 1;
                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }
                        else if (frames >= 1 && height_III_timer && applyImpulseComponent.InJump == true &&
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

