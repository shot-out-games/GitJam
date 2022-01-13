﻿using SandBox.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;




[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(FinalIkSystem))]


public class BossAmmoHandlerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    //EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {

        if (LevelManager.instance.endGame == true) return;

        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.TempJob);
        int players = playerEntities.Length;

        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration
        BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);

        //var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var ammoGroup = GetComponentDataFromEntity<AmmoComponent>(false);
        Entities.WithoutBurst().WithNone<Pause>().ForEach(
            (
                 Entity entity,
                 ref BossAmmoManagerComponent bulletManagerComponent,
                in AttachWeaponComponent attachWeapon,
                in BossMovementComponent bossMovementComponent
                 //in ActorWeaponAimComponent actorWeaponAimComponent
                 ) =>
            {

                DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[entity];
                if (targetPointBuffer.Length <= 0)
                    return;
                //int strike = targetPointBuffer[bossMovementComponent.CurrentIndex].wayPointStrike;


                Entity playerE = Entity.Null;
                //change to closest
                for (int i = 0; i < players; i++)
                {
                    playerE = playerEntities[i];
                }


                if (!HasComponent<BossWeaponComponent>(entity)) return;
                var gun = GetComponent<BossWeaponComponent>(entity);


                //if (attachWeapon.attachedWeaponSlot < 0 ||
                //    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                //    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                //    )
                //{
                //    gun.Duration = 0;
                //    gun.IsFiring = 0;
                //    return;
                //}


                if (HasComponent<DeadComponent>(entity)) return;//fix add

                Entity primaryAmmoEntity = gun.PrimaryAmmo;
                var ammoDataComponent = GetComponent<AmmoDataComponent>(primaryAmmoEntity);
                float rate = ammoDataComponent.GameRate;
                float strength = ammoDataComponent.GameStrength;
                float damage = ammoDataComponent.GameDamage;
                //change based on game
                if (gun.ChangeAmmoStats > 0)
                {
                    strength = strength * (100 - gun.ChangeAmmoStats * 2) / 100;
                    if (strength <= 0) strength = 0;
                }




                //if (gun.IsFiring == 1) gun.Duration += dt;

                //if ((gun.Duration > rate) && (gun.IsFiring == 1))
                //{
                //    gun.Duration = 0;
                //    gun.IsFiring = 0;
                //    gun.CanFire = true;
                //}

                //if (gun.CanFire == true)
                if (gun.IsFiring == 1)
                    {
                        gun.IsFiring = 0;

                    var move = GetComponent<Translation>(playerE);

                    //    if (actorWeaponAimComponent.weaponRaised == WeaponMotion.Raised || isEnemy)
                    //    {
                    gun.CanFire = false;
                    //gun.Duration = 0;
                    //gun.IsFiring = 0;
                    var e = commandBuffer.Instantiate(gun.PrimaryAmmo);
                    var translation = new Translation() { Value = gun.AmmoStartLocalToWorld.Position };//use bone mb transform
                    var rotation = new Rotation() { Value = gun.AmmoStartLocalToWorld.Rotation };
                    var velocity = new PhysicsVelocity();
                    //float3 forward = gun.AmmoStartLocalToWorld.Forward;
                    float3 forward = math.forward(rotation.Value);
                    //forward.y = -1;
                    //forward = forward * math.normalize(translation.Value - move.Value);
                    velocity.Linear = forward * strength;



                    bulletManagerComponent.playSound = true;
                    //bulletManagerComponent.setAnimationLayer = true;

                    commandBuffer.SetComponent(e, new TriggerComponent
                    { Type = (int)TriggerType.Ammo, ParentEntity = entity, Entity = e, Active = true });
                    commandBuffer.SetComponent(e, translation);
                    commandBuffer.SetComponent(e, rotation);
                    commandBuffer.SetComponent(e, velocity);


                    //    }
                }

                commandBuffer.SetComponent(entity, gun);



            }
        ).Run();


        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
        playerEntities.Dispose();


    }



}
