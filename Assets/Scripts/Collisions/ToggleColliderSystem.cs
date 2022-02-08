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
            Particle = 1 << 8,
            Camera = 1 << 9,
            Crosshair = 1 << 10,
            Breakable = 1 << 11
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
                //bool hasToggleCollision = HasComponent<ToggleCollisionComponent>(e);
                if (hasCollider && playerDashComponent.box == BlobAssetReference<Unity.Physics.Collider>.Null)
                {
                    var collider = GetComponent<PhysicsCollider>(e);
                    playerDashComponent.box = collider.Value.Value.Clone();
                    unsafe
                    {
                        var header = (ConvexCollider*)collider.ColliderPtr;
                        var filter = header->Filter;

                        filter.BelongsTo = (uint)CollisionLayer.Player;
                        filter.CollidesWith = (uint)CollisionLayer.Ground | (uint)CollisionLayer.Obstacle | (uint)CollisionLayer.Breakable | (uint)CollisionLayer.Enemy;

                        header->Filter = filter;
                    }


                }

                bool hasToggleCollision = HasComponent<ToggleCollisionComponent>(e);
                playerDashComponent.Invincible = false;
                if (playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleStart && playerDashComponent.DashTimeTicker < playerDashComponent.invincibleEnd)
                {

                    playerDashComponent.Invincible = true;
                    if (hasToggleCollision)
                    {
                        Debug.Log("remove " + playerDashComponent.box);
                        ecb.RemoveComponent<ToggleCollisionComponent>(e);
                        var collider = GetComponent<PhysicsCollider>(e);
                        unsafe
                        {
                            var header = (ConvexCollider*)collider.ColliderPtr;
                            var filter = header->Filter;

                            filter.BelongsTo = (uint)CollisionLayer.Player;
                            filter.CollidesWith = (uint)CollisionLayer.Ground;

                            header->Filter = filter;
                        }
                    }
                }
                else if ((playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleEnd || playerDashComponent.DashTimeTicker == 0) && hasToggleCollision == false)
                {

                    Debug.Log("add " + playerDashComponent.box);
                    ecb.AddComponent<ToggleCollisionComponent>(e, new ToggleCollisionComponent { });
                    var collider = GetComponent<PhysicsCollider>(e);
                    unsafe
                    {
                        var header = (ConvexCollider*)collider.ColliderPtr;
                        var filter = header->Filter;

                        filter.BelongsTo = (uint)CollisionLayer.Player;
                        filter.CollidesWith = (uint)CollisionLayer.Ground | (uint)CollisionLayer.Obstacle | (uint)CollisionLayer.Breakable | (uint)CollisionLayer.Enemy; 

                        header->Filter = filter;
                    }
                    //ecb.SetComponent<PhysicsCollider>(e, new PhysicsCollider { Value = playerDashComponent.box });
                }

               


            }
            ).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();




        }
    }

}

