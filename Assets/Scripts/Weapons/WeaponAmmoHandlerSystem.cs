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

        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var ammoGroup = GetComponentDataFromEntity<AmmoComponent>(false);
        Entities.WithBurst(FloatMode.Default, FloatPrecision.Standard, true).WithNone<Pause>().ForEach(
            // Entities.WithBurst().WithNone<Pause>().ForEach(
            (
                 Entity entity,
                 int entityInQueryIndex,
                 ref BulletManagerComponent bulletManagerComponent,
                in ActorWeaponAimComponent actorWeaponAimComponent,
                in DeadComponent dead,
                 in PhysicsVelocity playerVelocity,
                in AttachWeaponComponent attachWeapon
                 ) =>
            {


                if (!HasComponent<WeaponComponent>(entity)) return;
                var gun = GetComponent<WeaponComponent>(entity);


                if (attachWeapon.attachedWeaponSlot < 0 ||
                    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                    )
                {
                    gun.Duration = 0;
                    gun.IsFiring = 0;
                    return;
                }


                if (dead.isDead) return;
                bool isEnemy = HasComponent<EnemyComponent>(entity);

                if (isEnemy)
                {
                    if (HasComponent<EnemyWeaponMovementComponent>(entity) == false)
                    {
                        return;
                    }
                }

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



                if (gun.IsFiring == 1 && gun.Duration == 0)
                {
                    //Debug.Log(" gun fired ");
                    gun.Duration += dt;
                   
                    var e = commandBuffer.Instantiate(entityInQueryIndex, gun.PrimaryAmmo);
                    //Debug.Log("e " + e);
                    var translation = new Translation() { Value = gun.AmmoStartLocalToWorld.Position };//use bone mb transform
                    var rotation = new Rotation() { Value = gun.AmmoStartLocalToWorld.Rotation };
                    var velocity = new PhysicsVelocity();

                    float3 forward = actorWeaponAimComponent.aimDirection;

                    if (actorWeaponAimComponent.weaponCamera == CameraTypes.TopDown)
                    {
                        velocity.Linear = actorWeaponAimComponent.aimDirection * strength;
                        velocity.Angular = math.float3(0, 0, 0);
                    }
                    else
                    {
                        velocity.Linear = actorWeaponAimComponent.aimDirection * strength;
                        velocity.Angular = math.float3(0, 0, 0);
                    }

                    Debug.Log("vel " + velocity.Linear);

                    bulletManagerComponent.playSound = true;
                    bulletManagerComponent.setAnimationLayer = true;

                    ammoDataComponent.Shooter = entity;
                    commandBuffer.SetComponent(entityInQueryIndex, e, ammoDataComponent);

                    commandBuffer.SetComponent(entityInQueryIndex, e, new TriggerComponent
                    { Type = (int)TriggerType.Ammo, ParentEntity = entity, Entity = e, Active = true });
                    commandBuffer.SetComponent(entityInQueryIndex, e, translation);
                    commandBuffer.SetComponent(entityInQueryIndex, e, rotation);
                    commandBuffer.SetComponent(entityInQueryIndex, e, velocity);

             

                }
                else if (gun.IsFiring == 1 && gun.Duration > 0)
                {
                    gun.Duration += dt;
                    if ((gun.Duration > rate) && (gun.IsFiring == 1))
                    {
                        gun.Duration = 0;
                        gun.IsFiring = 0;
                    }


                }


                commandBuffer.SetComponent(entityInQueryIndex, entity, gun);



            }
        ).ScheduleParallel();
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);


    }



}
