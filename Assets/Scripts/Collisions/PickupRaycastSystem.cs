using RootMotion.FinalIK;
using SandBox.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using SphereCollider = Unity.Physics.SphereCollider;




[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]



public class PickupWeaponRaycastSystem : SystemBase
{


    public enum CollisionLayer
    {
        Player = 1 << 0,
        Ground = 1 << 1,
        Enemy = 1 << 2,
        Item = 1 << 3
    }


    protected override void OnUpdate()
    {
        bool pickedUp = false;
        Entity pickedUpEntity = Entity.Null;
        InteractionObject interactionObject = null;
        Entity pickerUpper = Entity.Null;


        var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();



        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, WeaponItem WeaponItem,
                ref Translation translation, ref PhysicsCollider collider, ref Rotation rotation) =>
        {

            if (bufferFromEntity.HasComponent(entity))
            {
                //var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();
                var weaponItemComponent = bufferFromEntity[entity];


                var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
                var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;


                float3 start = translation.Value + new float3(0f, .38f, 0);
                float3 direction = new float3(0, 0, 0);
                float distance = 2f;
                float3 end = start + direction * distance;


                PointDistanceInput pointDistanceInput = new PointDistanceInput
                {
                    Position = start,
                    MaxDistance = distance,
                    //Filter = CollisionFilter.Default
                    Filter = new CollisionFilter()
                    {
                        //BelongsTo = (uint)CollisionLayer.Player,
                        //CollidesWith = (uint)CollisionLayer.Item,
                        BelongsTo = 4u,
                        CollidesWith = 1u,
                        GroupIndex = 0
                    }
                };



                bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);
                if (HasComponent<TriggerComponent>(pointHit.Entity))
                {
                    var parent = EntityManager.GetComponentData<TriggerComponent>(pointHit.Entity).ParentEntity;
                    pickerUpper = parent;
                    Debug.Log(" pt e " + pickerUpper);

                }


                if (hasPointHit && weaponItemComponent[0].pickedUp == false)
                {


                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;

                    if (WeaponItem.e == entity)
                    {
                        //weaponItemComponent.pickedUp = true;

                        var intBufferElement = weaponItemComponent[0];
                        intBufferElement.pickedUp = true;
                        weaponItemComponent[0] = intBufferElement;

                        pickedUp = true;
                        pickedUpEntity = entity;
                        interactionObject = WeaponItem.interactionObject;
                        //Debug.Log(" pt e " + e);
                    }
                }




            }




        }).Run();


        if (pickedUp)
        {



            Entities.WithoutBurst().ForEach((WeaponInteraction weaponInteraction, WeaponManager weaponManager, Entity e) =>
            {
                if (pickerUpper == e)
                {
                    weaponManager.DetachPrimaryWeapon(); //need to add way to set to not picked up  afterwards
                    weaponInteraction.interactionObject = interactionObject;
                    //weaponManager.primaryWeapon.weaponGameObject = interactionObject.gameObject;
                    weaponManager.primaryWeapon.weaponGameObject =
                        interactionObject.GetComponent<WeaponItem>().gameObject; //always the same as go for now
                    weaponManager.AttachPrimaryWeapon();
                    weaponInteraction.UpdateSystem();
                    Debug.Log("MATCH FOUND");
                }
                else
                {
                    pickedUp = false;
                    var weaponItemBufferList = GetBufferFromEntity<WeaponItemComponent>();
                    var weaponItemComponent = bufferFromEntity[pickedUpEntity];
                    var intBufferElement = weaponItemComponent[0];
                    intBufferElement.pickedUp = false;
                    weaponItemComponent[0] = intBufferElement;

                }

            }).Run();

            //EntityManager.DestroyEntity(pickedUpEntity);

        }

        if (pickedUp == true)
        {
            EntityManager.SetComponentData(pickedUpEntity, new Translation { Value = new float3(0, -2500, 0) });
        }


    }




}

[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


public class PickupPowerUpRaycastSystem : SystemBase
{

    //EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        //m_EndSimulationEcbSystem = World
        //  .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //bool pickedUp = false;
        Entity pickedUpActor = Entity.Null;
        //var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        var ecb = new EntityCommandBuffer(Allocator.Persistent);


        Entities.WithoutBurst().WithNone<DestroyComponent>().ForEach((
            ref PowerItemComponent powerItemComponent,
            in Translation translation,
            in Entity entity
        ) =>
        {
            //NativeList<Entity> pickups = new NativeList<Entity>(32, Allocator.TempJob);

            var physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            float3 start = translation.Value + new float3(0f, .38f, 0);
            float3 direction = new float3(0, 0, 0);
            float distance = 2f;
            float3 end = start + direction * distance;




            PointDistanceInput pointDistanceInput = new PointDistanceInput
            {
                Position = start,
                MaxDistance = distance,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
                    BelongsTo = 7u,
                    CollidesWith = 1u,
                    GroupIndex = 0
                }
            };



            bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);


            if (hasPointHit && powerItemComponent.itemPickedUp == false)
            {
                if (HasComponent<TriggerComponent>(pointHit.Entity))
                {
                    var parent = GetComponent<TriggerComponent>(pointHit.Entity).ParentEntity;
                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;
                    pickedUpActor = parent;
                    powerItemComponent.pickedUpActor = pickedUpActor;

                    Debug.Log(" pickup " + pickedUpActor);

                    powerItemComponent.addPickupEntityToInventory = pickedUpActor;
                    powerItemComponent.itemPickedUp = true;
                    ecb.AddComponent(entity, powerItemComponent);

                    //if(HasComponent<HealthPower>(entity))
                    //{
                    //    var item = GetComponent<HealthPower>(entity);
                    //    item.count += 1;
                    //    ecb.AddComponent(entity, item);

                    //}
                    //else if (HasComponent<Speed>(entity))
                    //{
                    //    var item = GetComponent<Speed>(entity);
                    //    item.count += 1;
                    //    ecb.AddComponent(entity, item);
                    //}

                    PickupMenuGroup.UpdateMenu = true;

                }
            }



        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        // Make sure that the ECB system knows about our job
        //m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);


    }





}






public class PickupInputPowerUpUseImmediateSystem : SystemBase//move to new file later 
{

    //EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        // m_EndSimulationEcbSystem = World
        //   .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    //BufferFromEntity<BossWaypointBufferElement> positionBuffer = GetBufferFromEntity<BossWaypointBufferElement>(true);
    //BufferFromEntity<BossAmmoListBuffer> ammoList = GetBufferFromEntity<BossAmmoListBuffer>(true);


    //Entities.WithoutBurst().ForEach((Entity enemyE,
    //    WeaponManager weaponManager, ref BossWeaponComponent bossWeaponComponent, in BossMovementComponent bossMovementComponent) =>
    //    {
    //        DynamicBuffer<BossWaypointBufferElement> targetPointBuffer = positionBuffer[enemyE];
    //DynamicBuffer<BossAmmoListBuffer> ammoListBuffer = ammoList[enemyE];
    //        if (targetPointBuffer.Length <= 0 || bossMovementComponent.WayPointReached == false)
    //            return;
    protected override void OnUpdate()
    {
        //bool pickedUp = false;
        Entity pickedUpActor = Entity.Null;
        bool usedItem = false;
        //var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Persistent);
        //var healthPowerList = GetBufferFromEntity<HealthPower>(false);

        Entities.WithAny<UseItem1, UseItem2>().WithoutBurst().ForEach((
            ref PowerItemComponent powerItemComponent,
            in ImmediateUseComponent immediateUseComponent,
            in Translation translation,
            in Entity entity
        ) =>
        {
            
            //NativeList<Entity> pickups = new NativeList<Entity>(32, Allocator.TempJob);
            //if (HasComponent<UseItem1>(entity) == false && HasComponent<UseItem2>(entity) == false) return;
            pickedUpActor = powerItemComponent.pickedUpActor;
            if (pickedUpActor == Entity.Null) return;


          

            if (HasComponent<HealthPower>(entity) && powerItemComponent.enabled == false)
            {
                if (powerItemComponent.enabled == false)
                {
                    usedItem = true;
                    powerItemComponent.enabled = true;
                    Entity instanceEntity = ecb.Instantiate(powerItemComponent.particleSystemEntity);
                    var ps = new ParticleSystemComponent
                    {
                        followActor = true,
                        pickedUpActor = pickedUpActor
                    };

                    ecb.AddComponent(instanceEntity, ps);

                    var healthPower = GetComponent<HealthPower>(entity);
                    HealthPower healthPowerPlayer = new HealthPower
                    {
                        psAttached = instanceEntity,//attached to player picking up
                        pickedUpActor = pickedUpActor,
                        itemEntity = entity,
                        enabled = true,
                        //healthMultiplier = healthPower[0].healthMultiplier
                        healthMultiplier = healthPower.healthMultiplier
                    };
                    //Debug.Log("health pu");
                    //ecb.RemoveComponent<UseItem1>(entity);
                    //ecb.AddBuffer<HealthPower>(pickedUpActor).Add(healthPowerPlayer);
                    ecb.AddComponent(entity, new DestroyComponent());
                    ecb.AddComponent(pickedUpActor, healthPowerPlayer);



                }

            }





            if (HasComponent<Speed>(entity) && powerItemComponent.enabled == false)
            {

                usedItem = true;
                var speedPower = GetComponent<Speed>(entity);
                Debug.Log("pi " + powerItemComponent.count);
                powerItemComponent.count -= 1;
                powerItemComponent.enabled = true;
                //if (powerItemComponent.useSlot1)
                //{
                //    powerItemComponent.useSlot1 = false;
                //}
                //if (powerItemComponent.useSlot2)
                //{
                //    powerItemComponent.useSlot2 = false;
                //}
                Entity instanceEntity = ecb.Instantiate(powerItemComponent.particleSystemEntity);

                var ps = new ParticleSystemComponent
                {
                    followActor = true,
                    pickedUpActor = pickedUpActor
                };

                ecb.AddComponent(instanceEntity, ps);

                // Debug.Log(" speed " + pickedUpActor);



                Speed speedPowerPlayer = new Speed
                {
                    psAttached = instanceEntity,//attached to player on  speed pick up
                    pickedUpActor = pickedUpActor,
                    itemEntity = entity,
                    enabled = true,
                    timeOn = speedPower.timeOn,
                    multiplier = speedPower.multiplier
                };

                //ecb.RemoveComponent<UseItem1>(entity);
                ecb.RemoveComponent<ImmediateUseComponent>(entity);
                //if (powerItemComponent.count < 0)
                ecb.AddComponent(entity, new DestroyComponent());
                ecb.AddComponent(pickedUpActor, speedPowerPlayer);





            }



        }).Run();



        if(HasSingleton<PickupMenuComponent>())
        {
            var _e = GetSingletonEntity<PickupMenuComponent>();
            var pu = GetComponent<PickupMenuComponent>(_e);
            if(usedItem)
            {
                pu.usedItem = usedItem;
            }

           // Debug.Log("pu " + pu.usedItem);
            SetSingleton<PickupMenuComponent>(pu);

        }

        // Make sure that the ECB system knows about our job
        //m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        ecb.Playback(EntityManager);
        ecb.Dispose();


    }





}



