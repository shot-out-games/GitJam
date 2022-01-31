using SandBox.Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class BreakableCollisionSystem : SystemBase
{
    //EndSimulationEntityCommandBufferSystem m_ecbSystem;
    EndFixedStepSimulationEntityCommandBufferSystem m_ecbSystem;
    BuildPhysicsWorld buildPhysicsWorldSystem;
    StepPhysicsWorld stepPhysicsWorld;
    EndFramePhysicsSystem endFramePhysicsSystem;

    protected override void OnCreate()
    {
        m_ecbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

    }

    protected override void OnUpdate()
    {
        EntityQuery breakableQuery = GetEntityQuery(ComponentType.ReadOnly<BreakableComponent>());
        NativeArray<BreakableComponent> breakableGroup = breakableQuery.ToComponentDataArray<BreakableComponent>(Allocator.Persistent);

        var inputDeps = new JobHandle();

        inputDeps = JobHandle.CombineDependencies(inputDeps, buildPhysicsWorldSystem.GetOutputDependency());
        inputDeps = JobHandle.CombineDependencies(inputDeps, stepPhysicsWorld.GetOutputDependency());
        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;
        var collisionJob = new BreakableCollisionJob
        {
            physicsWorld = physicsWorld,
            ecb = m_ecbSystem.CreateCommandBuffer(),
            breakableGroup = GetComponentDataFromEntity<BreakableComponent>(true),
            ammoGroup = GetComponentDataFromEntity<AmmoDataComponent>(true),
            gravityGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(false)
        };
        JobHandle collisionHandle = collisionJob.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, inputDeps);
        collisionHandle.Complete();

    } 


    struct BreakableCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public ComponentDataFromEntity<AmmoDataComponent> ammoGroup;
        [ReadOnly] public ComponentDataFromEntity<BreakableComponent> breakableGroup;
        public ComponentDataFromEntity<PhysicsGravityFactor> gravityGroup;
        //[DeallocateOnJobCompletion]
        //[ReadOnly] public NativeArray<BreakableComponent> breakableComponents;

        //EntityQuery triggerQuery =  GetEntityQuery(ComponentType.ReadOnly<TriggerComponent>(),

        //EntityQuery breakableQuery = GetEntityQuery(ComponentType.ReadOnly<BreakableComponent>());
        //NativeArray<BreakableComponent> breakableGroup = breakableQuery.ToComponentDataArray<BreakableComponent>(Allocator.Persistent);






        public EntityCommandBuffer ecb;
        public void Execute(CollisionEvent ev) // this is never called
        {
            Entity a = ev.EntityA;
            Entity b = ev.EntityB;

            

            if (breakableGroup.HasComponent(a) == false) return;
            if (ammoGroup.HasComponent(b) == true && gravityGroup.HasComponent(b))
            {
                
                Debug.Log("collide");
                var breakable = breakableGroup[a];
                var gravity = gravityGroup[b];
                gravity.Value = breakable.gravityFactorAfterBreaking;
                ecb.SetComponent<PhysicsGravityFactor>(a, gravity);

            }    







            //if (ammoA)
            //{
            //    CollisionComponent collisionComponent =
            //        new CollisionComponent()
            //        {
            //            Part_entity = triggerComponent_b.Type,
            //            Part_other_entity = triggerComponent_a.Type,
            //            Character_entity = triggerComponent_b.ParentEntity,//actor hit by ammo
            //            Character_other_entity = triggerComponent_a.Entity
            //        };
            //    ecb.AddComponent(triggerComponent_a.ParentEntity, collisionComponent);
            //}
            //else if (ammoB)
            //{

            //    CollisionComponent collisionComponent =
            //        new CollisionComponent()
            //        {
            //            Part_entity = triggerComponent_a.Type,
            //            Part_other_entity = triggerComponent_b.Type,
            //            Character_entity = triggerComponent_a.ParentEntity,
            //            Character_other_entity = triggerComponent_b.Entity

            //        };

            //    ecb.AddComponent(triggerComponent_b.ParentEntity, collisionComponent);
            //}
            //else if (!ammoA && !ammoB)
            //{

            //    CollisionComponent collisionComponent =
            //        new CollisionComponent()
            //        {
            //            Part_entity = triggerComponent_a.Type,
            //            Part_other_entity = triggerComponent_b.Type,
            //            Character_entity = ch_a,
            //            Character_other_entity = ch_b
            //        };
            //    ecb.AddComponent(ch_a, collisionComponent);
            //}
            //else if (!ammoA && !ammoB)
            //{

            //    CollisionComponent collisionComponent =
            //        new CollisionComponent()
            //        {
            //            Part_entity = triggerComponent_b.Type,
            //            Part_other_entity = triggerComponent_a.Type,
            //            Character_entity = ch_b,
            //            Character_other_entity = ch_a

            //        };
            //    ecb.AddComponent(ch_b, collisionComponent);
            //}



        }
    }








} // System


