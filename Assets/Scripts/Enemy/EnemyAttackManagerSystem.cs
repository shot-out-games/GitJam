using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class EnemyAttackManagerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);
        BufferFromEntity<BossAmmoListBuffer> ammoList = GetBufferFromEntity<BossAmmoListBuffer>(true);


        Entities.WithoutBurst().ForEach((Entity enemyE, 
            WeaponManager weaponManager, ref BossWeaponComponent bossWeaponComponent, in BossMovementComponent bossMovementComponent) =>
        {
            DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
            DynamicBuffer<BossAmmoListBuffer> ammoListBuffer = ammoList[enemyE];
            if (targetPointBuffer.Length <= 0 || bossMovementComponent.WayPointReached == false)
                return;

           





            int weaponIndex = targetPointBuffer[bossMovementComponent.CurrentIndex].weaponListIndex;
            int ammoIndex = targetPointBuffer[bossMovementComponent.CurrentIndex].ammoListIndex;


            if (bossMovementComponent.CurrentIndex <= 0) return;






            weaponManager.DetachPrimaryWeapon(); //need to add way to set to not picked up  afterwards
            weaponManager.primaryWeapon = weaponManager.weaponsList[weaponIndex];




            weaponManager.AttachPrimaryWeapon();
            //Debug.Log("MATCH FOUND " + weaponIndex + " " + bossWeaponComponent.PrimaryAmmo);

        }).Run();







    }






}


