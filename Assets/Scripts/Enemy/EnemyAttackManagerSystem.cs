using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


public class EnemyAttackManagerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);


        Entities.WithoutBurst().ForEach((Entity enemyE, Animator animator, BossWeaponComponent bossWeaponComponent,
            BossMovementComponent bossMovementComponent,
            WeaponManager weaponManager) =>
        {
            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
            if (targetPointBuffer.Length <= 0 || bossMovementComponent.WayPointReached == false)
                return;
            //bossWeaponComponent.IsFiring = 0;
            int weaponIndex = targetPointBuffer[bossMovementComponent.CurrentIndex].weaponListIndex;


            weaponManager.DetachPrimaryWeapon(); //need to add way to set to not picked up  afterwards
            //weaponManager.DetachSecondaryWeapon(); //need to add way to set to not picked up  afterwards
            //weaponManager.DeactivateWeapons();
            weaponManager.primaryWeapon = weaponManager.weaponsList[weaponIndex];
            weaponManager.AttachPrimaryWeapon();
            Debug.Log("MATCH FOUND " + weaponIndex + " " + bossMovementComponent.CurrentIndex);

        }).Run();







    }






}


