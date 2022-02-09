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
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            BufferFromEntity<ActorCollisionBufferElement> actorCollisionBufferElement = GetBufferFromEntity<ActorCollisionBufferElement>(true);


            JobHandle inputDeps = Entities.ForEach((Entity e, ref PlayerDashComponent playerDashComponent) =>
            {
                DynamicBuffer<ActorCollisionBufferElement> actorCollisionElement = actorCollisionBufferElement[e];
                if (actorCollisionElement.Length <= 0)
                    return;

                bool addColliders = false;
                bool removeColliders = false;

                bool hasToggleCollision = HasComponent<ToggleCollisionComponent>(e);
                playerDashComponent.Invincible = false;
                if (playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleStart && playerDashComponent.DashTimeTicker < playerDashComponent.invincibleEnd)
                {

                    playerDashComponent.Invincible = true;
                    if (hasToggleCollision)
                    {

                        ecb.RemoveComponent<ToggleCollisionComponent>(e);
                        removeColliders = true;
                    }
                }
                else if ((playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleEnd || playerDashComponent.DashTimeTicker == 0) && hasToggleCollision == false)
                {

                    ecb.AddComponent<ToggleCollisionComponent>(e, new ToggleCollisionComponent { });
                    addColliders = true;
                }

                for (int i = 0; i < actorCollisionElement.Length; i++)
                {
                    var childEntity = actorCollisionElement[i]._child;
                    var collider = GetComponent<PhysicsCollider>(childEntity);
                    if (addColliders)
                    {
                        unsafe
                        {
                            var header = (ConvexCollider*)collider.ColliderPtr;
                            var filter = header->Filter;

                            filter.BelongsTo = (uint)CollisionLayer.Player;
                            filter.CollidesWith = (uint)CollisionLayer.Ground | (uint)CollisionLayer.Obstacle | (uint)CollisionLayer.Breakable | (uint)CollisionLayer.Enemy;

                            header->Filter = filter;
                        }
                    }
                    else if (removeColliders)
                    {
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



            }
            ).Schedule(this.Dependency);

            inputDeps.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();




        }
    }

}

