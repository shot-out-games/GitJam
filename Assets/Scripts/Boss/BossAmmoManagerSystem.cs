using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Physics.Extensions;
using UnityEngine.Jobs;

public class BossAmmoManagerSystem : SystemBase
{


    protected override void OnUpdate()
    {



        Entities.WithoutBurst().ForEach(
            (
                 Entity e,
                 BossAmmoManager bulletManager,
                 Animator animator,
                 ref BossAmmoManagerComponent bulletManagerComponent
                 ) =>
            {



                if (bulletManager.weaponAudioClip && bulletManager.weaponAudioSource && bulletManagerComponent.playSound)
                {
                    bulletManager.weaponAudioSource.PlayOneShot(bulletManager.weaponAudioClip, .25f);
                    bulletManagerComponent.playSound = false;
                }

                if (bulletManagerComponent.setAnimationLayer)
                {
                    animator.SetLayerWeight(0, 0);
                    bulletManagerComponent.setAnimationLayer = false;
                }



            }
        ).Run();


    }

}
