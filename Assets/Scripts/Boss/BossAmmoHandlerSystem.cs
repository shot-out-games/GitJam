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
//[UpdateAfter(typeof(FinalIkSystem))]


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
                in BossMovementComponent bossMovementComponent,
                in BossStrategyComponent bossStrategyComponent
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
                var bossWeapon = GetComponent<BossWeaponComponent>(entity);


                //if (attachWeapon.attachedWeaponSlot < 0 ||
                //    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                //    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                //    )
                //{
                //    gun.Duration = 0;
                //    gun.IsFiring = 0;
                //    return;
                //}


                //if (HasComponent<DeadComponent>(entity)) return;//fix add

                Entity primaryAmmoEntity = bossWeapon.PrimaryAmmo;
                var ammoDataComponent = GetComponent<AmmoDataComponent>(primaryAmmoEntity);
                float rate = ammoDataComponent.GameRate;
                float strength = ammoDataComponent.GameStrength;
                float damage = ammoDataComponent.GameDamage;
                //change based on game
                if (bossWeapon.ChangeAmmoStats > 0)
                {
                    strength = strength * (100 - bossWeapon.ChangeAmmoStats * 2) / 100;
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
                if (bossWeapon.IsFiring == 1)
                    {
                        bossWeapon.IsFiring = 0;

                    //var move = GetComponent<Translation>(playerE);
                    var playerMove = GetComponent<Translation>(playerE);
                    var bossTranslation = GetComponent<Translation>(entity);


                    //    if (actorWeaponAimComponent.weaponRaised == WeaponMotion.Raised || isEnemy)
                    //    {
                    //gun.CanFire = false;
                    //gun.Duration = 0;
                    //gun.IsFiring = 0;
                    var e = commandBuffer.Instantiate(bossWeapon.PrimaryAmmo);
                    //var translation = new Translation() { Value = bossWeapon.AmmoStartLocalToWorld.Position };//use bone mb transform
                    var translation = new Translation() { Value = bossWeapon.AmmoStartPosition.Value };//use bone mb transform
                    //Debug.Log("tr " + translation.Value);
                    var playerTranslation = GetComponent<Translation>(playerE).Value;
                    //var rotation = new Rotation() { Value = bossWeapon.AmmoStartLocalToWorld.Rotation };
                    var rotation = new Rotation() { Value = bossWeapon.AmmoStartRotation.Value };
                    var velocity = new PhysicsVelocity();
                    //float3 forward = gun.AmmoStartLocalToWorld.Forward;

                    float3 forward = math.forward(rotation.Value);

                    if (bossStrategyComponent.AimAtPlayer)
                    {
                        float3 bossXZ = new float3(bossTranslation.Value.x, bossTranslation.Value.y, bossTranslation.Value.z);
                        float3 ammoStartXZ = new float3(translation.Value.x, translation.Value.y, translation.Value.z);
                        float3 direction = math.normalize(ammoStartXZ - bossXZ);
                        quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());//always face player
                        //forward = targetRotation.value;                        
                        forward = direction;

                        //forward.y = math.sign(playerTranslation.y - translation.Value.y);
                    }

                    //forward = forward * math.normalize(translation.Value - move.Value);
                    velocity.Linear = math.normalize(forward) * strength;



                    bulletManagerComponent.playSound = true;
                    //bulletManagerComponent.setAnimationLayer = true;

                    commandBuffer.SetComponent(e, new TriggerComponent
                    { Type = (int)TriggerType.Ammo, ParentEntity = entity, Entity = e, Active = true });
                    commandBuffer.SetComponent(e, translation);
                    commandBuffer.SetComponent(e, rotation);
                    commandBuffer.SetComponent(e, velocity);


                    //    }
                }

                commandBuffer.SetComponent(entity, bossWeapon);



            }
        ).Run();


        //m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
        playerEntities.Dispose();


    }



}
