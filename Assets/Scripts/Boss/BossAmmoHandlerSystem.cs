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


        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var ammoGroup = GetComponentDataFromEntity<AmmoComponent>(false);
        Entities.WithBurst(FloatMode.Default, FloatPrecision.Standard, true).WithNone<Pause>().ForEach(
            (
                 Entity entity,
                 int entityInQueryIndex,
                 ref BossAmmoManagerComponent bulletManagerComponent,
                 in PhysicsVelocity playerVelocity,
                in AttachWeaponComponent attachWeapon
                 //in ActorWeaponAimComponent actorWeaponAimComponent
                 ) =>
            {
                Debug.Log("fire");


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


                //gun.IsFiring = 1;


                if(gun.IsFiring == 1) gun.Duration += dt;

                if ((gun.Duration > rate) && (gun.IsFiring == 1))
                {
                    gun.Duration = 0;
                    gun.IsFiring = 0;
                    gun.CanFire = true;
                }

                if (gun.CanFire == true)
                {


                    //    if (actorWeaponAimComponent.weaponRaised == WeaponMotion.Raised || isEnemy)
                    //    {
                    gun.CanFire = false;
                    //gun.Duration = 0;
                    //gun.IsFiring = 0;
                    var e = commandBuffer.Instantiate(entityInQueryIndex, gun.PrimaryAmmo);
                    var translation = new Translation() { Value = gun.AmmoStartLocalToWorld.Position };//use bone mb transform
                    var rotation = new Rotation() { Value = gun.AmmoStartLocalToWorld.Rotation };
                    var velocity = new PhysicsVelocity();
                    float3 forward = gun.AmmoStartLocalToWorld.Forward;
                    //float3 forward = math.forward(rotation.Value);
                    velocity.Linear = forward * strength;



                    bulletManagerComponent.playSound = true;
                    //bulletManagerComponent.setAnimationLayer = true;

                    commandBuffer.SetComponent(entityInQueryIndex, e, new TriggerComponent
                    { Type = (int)TriggerType.Ammo, ParentEntity = entity, Entity = e, Active = true });
                    commandBuffer.SetComponent(entityInQueryIndex, e, translation);
                    commandBuffer.SetComponent(entityInQueryIndex, e, rotation);
                    commandBuffer.SetComponent(entityInQueryIndex, e, velocity);


                    //    }
                }

                commandBuffer.SetComponent(entityInQueryIndex, entity, gun);



            }
        ).ScheduleParallel();



        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);


    }



}
