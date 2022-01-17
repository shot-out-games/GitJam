using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;



//[UpdateBefore(typeof(GunAmmoHandlerSystem))]
[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]

public class EnemyWeaponAimSystemLateUpdate : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((in EnemyWeaponAim mb, in ActorWeaponAimComponent playerWeaponAimComponent) =>
        {
            mb.LateUpdateSystem();
        }).Run();
    }
}


//[UpdateBefore(typeof(GunAmmoHandlerSystem))]
[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]

public class PlayerWeaponAimSystemLateUpdate : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((PlayerWeaponAim mb, ref ActorWeaponAimComponent playerWeaponAimComponent) =>
        {
            mb.LateUpdateSystem(playerWeaponAimComponent.weaponRaised);
            playerWeaponAimComponent.aimDirection = mb.aimDir;
            //Debug.Log("v " + playerWeaponAimComponent.aimDirection);
        }).Run();
    }
}



