using Unity.Assertions;
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
    [UpdateBefore(typeof(BuildPhysicsWorld))]

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
        private static void CheckColliderFilterIntegrity(NativeArray<PhysicsCollider> colliders)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
                if (collider.Value.Value.Type == ColliderType.Compound)
                {
                    unsafe
                    {
                        var compoundCollider = (CompoundCollider*)collider.ColliderPtr;

                        var rootFilter = compoundCollider->Filter;
                        var combinedFilter = CollisionFilter.Zero;
                        for (int childIndex = 0; childIndex < compoundCollider->Children.Length; childIndex++)
                        {
                            ref CompoundCollider.Child c = ref compoundCollider->Children[childIndex];
                            combinedFilter = CollisionFilter.CreateUnion(combinedFilter, c.Collider->Filter);
                        }

                        // Check that the combined filter of all children is the same as root filter.
                        // If not, it means user has forgotten to call RefreshCollisionFilter() on the CompoundCollider.
                        if (!rootFilter.Equals(combinedFilter))
                        {
                            Debug.Log("CollisionFilter of a compound collider is not a union of its children. " +
                                "You must call CompoundCollider.RefreshCollisionFilter() to update the root filter after changing child filters.");
                        }
                    }
                }
            }
        }


        static void SetCollisionFilter(PhysicsCollider collider, uint belongsTo, uint collidesWith)
        {
            
            unsafe
            {
                var header = (CompoundCollider*)collider.ColliderPtr;

                
                var filter = header->Filter;

                filter.BelongsTo = belongsTo;
                filter.CollidesWith = collidesWith;

                header->Filter = filter;
                header->RefreshCollisionFilter();


            }

        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            BufferFromEntity<ActorCollisionBufferElement> actorCollisionBufferElement = GetBufferFromEntity<ActorCollisionBufferElement>(true);

            JobHandle inputDeps = Entities.ForEach((Entity e, ref PlayerDashComponent playerDashComponent) =>
            //Entities.WithoutBurst().ForEach((Entity e, ref PlayerDashComponent playerDashComponent) =>
            {
                DynamicBuffer<ActorCollisionBufferElement> actorCollisionElement = actorCollisionBufferElement[e];
                if (actorCollisionElement.Length <= 0 || playerDashComponent.active == false)
                    return;

                bool addColliders = false;
                bool removeColliders = false;

                bool hasToggleCollision = HasComponent<ToggleCollisionComponent>(e);
                //Debug.Log("has toggle " + hasToggleCollision);
                playerDashComponent.Invincible = false;
                if (playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleStart && playerDashComponent.DashTimeTicker < playerDashComponent.invincibleEnd)
                {

                    playerDashComponent.Invincible = true;
                    if (hasToggleCollision)
                    {
                        Debug.Log("remove toggle collision component");
                        ecb.RemoveComponent<ToggleCollisionComponent>(e);
                        removeColliders = true;
                    }
                }
                else if ((playerDashComponent.DashTimeTicker >= playerDashComponent.invincibleEnd || playerDashComponent.DashTimeTicker == 0) && hasToggleCollision == false)
                {
                    Debug.Log("add toggle collision component");
                    ecb.AddComponent<ToggleCollisionComponent>(e, new ToggleCollisionComponent { });
                    addColliders = true;
                }

                for (int i = 0; i < actorCollisionElement.Length; i++)
                {
                    var childEntity = actorCollisionElement[i]._child;
                    if (addColliders)
                    {
                        var collider = GetComponent<PhysicsCollider>(childEntity);
                        SetCollisionFilter(collider, (uint)CollisionLayer.Player, (uint)CollisionLayer.Ground | (uint)CollisionLayer.Obstacle | (uint)CollisionLayer.Breakable | (uint)CollisionLayer.Enemy);
                    }
                    else if (removeColliders)
                    {
                        var collider = GetComponent<PhysicsCollider>(childEntity);
                        SetCollisionFilter(collider, (uint)CollisionLayer.Player, (uint)CollisionLayer.Ground);
                    }

                }



            }
            ).Schedule(this.Dependency);
            //).Run();

            inputDeps.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();





        }
    }

}

