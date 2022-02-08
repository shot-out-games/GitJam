using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;



namespace SandBox.Player
{


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerDashSystem))]
    public class ToggleColliderSystem : SystemBase
    {

        private enum CollisionLayer
        {
            Player = 1 << 0,
            Ground = 1 << 1,
            Enemy = 1 << 2,
            WeaponItem = 1 << 3,
            Obstacle = 1 << 4,
            NPC = 1 << 5,
            PowerUp = 1 << 6,
            Stairs = 1 << 7,
            Particle = 1 << 8
        }

        protected override void OnCreate()
        {

        }


    

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities.WithoutBurst().ForEach((Entity e, ref PlayerDashComponent playerDashComponent, in ActorCollisionComponent actorCollisionComponent) =>
            {
                bool hasCollider = HasComponent<PhysicsCollider>(e);
                if (hasCollider && playerDashComponent.box == BlobAssetReference<Unity.Physics.Collider>.Null)
                {
                    var collider = GetComponent<PhysicsCollider>(e);
                    playerDashComponent.box = collider.Value;
                    unsafe
                    {
                        var header = (ConvexCollider*)collider.ColliderPtr;
                        var filter = header->Filter;

                        filter.BelongsTo = (uint) CollisionLayer.Player;
                        filter.CollidesWith = (uint) CollisionLayer.Ground;

                        header->Filter = filter;
                    }


                }
                playerDashComponent.Invincible = false;
                if (playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleStart && playerDashComponent.DashTimeTicker < playerDashComponent.invincibleEnd)
                {
                    //playerDashComponent.colliderAdded = false;
                    //playerDashComponent.colliderRemoved = true;
                    playerDashComponent.Invincible = true;
                    if (hasCollider)
                    {
                        Debug.Log("remove " + playerDashComponent.box);
                        //ecb.RemoveComponent<PhysicsCollider>(e);
                    }
                }
                else if ((playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleEnd || playerDashComponent.DashTimeTicker == 0) && hasCollider == false)
                {
                    //playerDashComponent.colliderAdded = true;
                    //playerDashComponent.colliderRemoved = false;
                    Debug.Log("add " + playerDashComponent.box);
                    //ecb.AddComponent<PhysicsCollider>(e, new PhysicsCollider { Value = playerDashComponent.box });
                }

            }
            ).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();




        }
    }

}

