using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;



[UpdateInGroup(typeof(TransformSystemGroup))]

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


[UpdateAfter(typeof(CrosshairRaycastSystem))]
[UpdateInGroup(typeof(TransformSystemGroup))]

public class PlayerWeaponAimSystemLateUpdate : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().WithNone<Pause>().ForEach((PlayerWeaponAim mb, ref ActorWeaponAimComponent playerWeaponAimComponent) =>
        {
            mb.LateUpdateSystem(playerWeaponAimComponent.weaponRaised);
            playerWeaponAimComponent.aimDirection = mb.aimDir;
            //Debug.Log("v " + playerWeaponAimComponent.aimDirection);
        }).Run();
    }
}



