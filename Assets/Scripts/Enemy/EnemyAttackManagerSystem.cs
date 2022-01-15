using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


public class EnemyAttackManagerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);
        BufferFromEntity<BossAmmoListBuffer> ammoList = GetBufferFromEntity<BossAmmoListBuffer>(true);


        Entities.WithoutBurst().ForEach((Entity enemyE, Animator animator,
            BossMovementComponent bossMovementComponent,
            WeaponManager weaponManager, ref BossWeaponComponent bossWeaponComponent) =>
        {
            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
            DynamicBuffer<BossAmmoListBuffer> ammoListBuffer = ammoList[enemyE];
            if (targetPointBuffer.Length <= 0 || bossMovementComponent.WayPointReached == false)
                return;
            int weaponIndex = targetPointBuffer[bossMovementComponent.CurrentIndex].weaponListIndex;
            if (bossMovementComponent.CurrentIndex < 1) return;

            weaponManager.DetachPrimaryWeapon(); //need to add way to set to not picked up  afterwards
            weaponManager.primaryWeapon = weaponManager.weaponsList[weaponIndex];
            bossWeaponComponent.PrimaryAmmo = ammoListBuffer[weaponIndex].e;
            weaponManager.AttachPrimaryWeapon();
            Debug.Log("MATCH FOUND " + weaponIndex + " " + bossWeaponComponent.PrimaryAmmo);

        }).Run();







    }






}


