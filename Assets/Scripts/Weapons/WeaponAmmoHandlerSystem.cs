using SandBox.Player;
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


public class WeaponAmmoHandlerSystem : SystemBase
{


    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;


    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

    }
    protected override void OnUpdate()
    {

        if (LevelManager.instance.endGame == true) return;

        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.TempJob);
        int players = playerEntities.Length;


        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration
        //var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var commandBuffer = new EntityCommandBuffer(Allocator.Persistent);

        var ammoGroup = GetComponentDataFromEntity<AmmoComponent>(false);
        Entities.WithBurst().WithNone<Pause>().ForEach(
            // Entities.WithBurst().WithNone<Pause>().ForEach(
            (
                 Entity entity,
                 ref BulletManagerComponent bulletManagerComponent,
                in ActorWeaponAimComponent actorWeaponAimComponent,
                in DeadComponent dead,
                 in PhysicsVelocity playerVelocity,
                in AttachWeaponComponent attachWeapon
                 ) =>
            {



                Entity playerE = Entity.Null;
                //change to closest
                for (int i = 0; i < players; i++)
                {
                    playerE = playerEntities[i];
                }

                if (!HasComponent<WeaponComponent>(entity)) return;
                var enemyWeapon = GetComponent<WeaponComponent>(entity);


                Entity primaryAmmoEntity = enemyWeapon.PrimaryAmmo;
                var ammoDataComponent = GetComponent<AmmoDataComponent>(primaryAmmoEntity);
                float rate = ammoDataComponent.GameRate;
                float strength = ammoDataComponent.GameStrength;
                //float damage = ammoDataComponent.GameDamage;
                //change based on game

                if (enemyWeapon.ChangeAmmoStats > 0)
                {
                    strength = strength * (100 - enemyWeapon.ChangeAmmoStats * 2) / 100;
                    if (strength <= 0) strength = 0;
                }



                if (enemyWeapon.IsFiring == 1)
                {
                    enemyWeapon.IsFiring = 0;
                    //var playerMove = GetComponent<Translation>(playerE);
                    var bossTranslation = GetComponent<Translation>(entity);
                    var e = commandBuffer.Instantiate(enemyWeapon.PrimaryAmmo);

                    var translation = new Translation() { Value = enemyWeapon.AmmoStartPosition.Value };//use bone mb transform
                    var playerTranslation = GetComponent<Translation>(playerE).Value;
                    var rotation = new Rotation() { Value = enemyWeapon.AmmoStartRotation.Value };
                    var velocity = new PhysicsVelocity();

                    float3 forward = math.forward(rotation.Value);

                    //if (bossStrategyComponent.AimAtPlayer)
                    //{
                    float3 bossXZ = new float3(bossTranslation.Value.x, bossTranslation.Value.y, bossTranslation.Value.z);
                    float3 ammoStartXZ = new float3(playerTranslation.x, playerTranslation.y, playerTranslation.z);
                    float3 direction = math.normalize(ammoStartXZ - bossXZ);
                    quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());//always face player
                    forward = direction;
                    //}

                    velocity.Linear = math.normalize(forward) * strength;

                    bulletManagerComponent.playSound = true;

                    ammoDataComponent.Shooter = entity;
                    commandBuffer.SetComponent(e, ammoDataComponent);

                    commandBuffer.SetComponent(e, new TriggerComponent
                    { Type = (int)TriggerType.Ammo, ParentEntity = entity, Entity = e, Active = true });
                    commandBuffer.SetComponent(e, translation);
                    commandBuffer.SetComponent(e, rotation);
                    commandBuffer.SetComponent(e, velocity);
                }








                //if (attachWeapon.attachedWeaponSlot < 0 ||
                //    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                //    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                //    )
                //{
                //    gun.Duration = 0;
                //    gun.IsFiring = 0;
                //    return;
                //}


                //if (dead.isDead) return;
                //bool isEnemy = HasComponent<EnemyComponent>(entity);

                //if (isEnemy)
                //{
                //    if (HasComponent<EnemyWeaponMovementComponent>(entity) == false)
                //    {
                //        return;
                //    }
                //}

                //Entity primaryAmmoEntity = gun.PrimaryAmmo;
                //var ammoDataComponent = GetComponent<AmmoDataComponent>(primaryAmmoEntity);
                //float rate = ammoDataComponent.GameRate;
                //float strength = ammoDataComponent.GameStrength;
                //float damage = ammoDataComponent.GameDamage;
                ////change based on game
                //if (gun.ChangeAmmoStats > 0)
                //{
                //    strength = strength * (100 - gun.ChangeAmmoStats * 2) / 100;
                //    if (strength <= 0) strength = 0;
                //}



                //if (gun.IsFiring == 1 && gun.Duration == 0)
                //{
                //    //Debug.Log(" gun fired ");
                //    gun.Duration += dt;

                //    var e = commandBuffer.Instantiate(gun.PrimaryAmmo);
                //    //Debug.Log("e " + e);
                //    //var translation = new Translation() { Value = gun.AmmoStartLocalToWorld.Position };//use bone mb transform
                //    //var rotation = new Rotation() { Value = gun.AmmoStartLocalToWorld.Rotation };
                //    //var velocity = new PhysicsVelocity();

                //    var translation = new Translation() { Value = bossWeapon.AmmoStartPosition.Value };//use bone mb transform
                //    var playerTranslation = GetComponent<Translation>(playerE).Value;
                //    var rotation = new Rotation() { Value = bossWeapon.AmmoStartRotation.Value };
                //    var velocity = new PhysicsVelocity();





                //    //float3 forward = actorWeaponAimComponent.aimDirection;

                //    //if (actorWeaponAimComponent.weaponCamera == CameraTypes.TopDown)
                //    //{
                //    //    velocity.Linear = actorWeaponAimComponent.aimDirection * strength;
                //    //    velocity.Angular = math.float3(0, 0, 0);
                //    //}
                //    //else
                //    //{
                //    //    velocity.Linear = actorWeaponAimComponent.aimDirection * strength;
                //    //    velocity.Angular = math.float3(0, 0, 0);
                //    //}

                //    //Debug.Log("vel " + velocity.Linear);

                //    bulletManagerComponent.playSound = true;
                //    bulletManagerComponent.setAnimationLayer = true;

                //    ammoDataComponent.Shooter = entity;
                //    commandBuffer.SetComponent(ammoDataComponent);

                //    commandBuffer.SetComponent(e, new TriggerComponent
                //    { Type = (int)TriggerType.Ammo, ParentEntity = entity, Entity = e, Active = true });
                //    commandBuffer.SetComponent(e, translation);
                //    commandBuffer.SetComponent(e, rotation);
                //    commandBuffer.SetComponent(e, velocity);



                //}
                //else if (gun.IsFiring == 1 && gun.Duration > 0)
                //{
                //    gun.Duration += dt;
                //    if ((gun.Duration > rate) && (gun.IsFiring == 1))
                //    {
                //        gun.Duration = 0;
                //        gun.IsFiring = 0;
                //    }


                //}


                commandBuffer.SetComponent(entity, enemyWeapon);



            }
        ).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
        playerEntities.Dispose();


        //m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);


    }



}
