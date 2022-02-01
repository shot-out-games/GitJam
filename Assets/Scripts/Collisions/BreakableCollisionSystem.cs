using SandBox.Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public struct BrokenComponent : IComponentData
{
    public bool Value;
}
public struct BrokenEffectComponent : IComponentData
{
    public bool Value;
}

public class BreakableCollisionSystem : SystemBase
{
    //EndSimulationEntityCommandBufferSystem m_ecbSystem;
    EndFixedStepSimulationEntityCommandBufferSystem m_ecbSystem;
    BuildPhysicsWorld buildPhysicsWorldSystem;
    StepPhysicsWorld stepPhysicsWorld;
    EndFramePhysicsSystem endFramePhysicsSystem;
    //public NativeArray<int> BreakableGroupIndex;

    protected override void OnCreate()
    {
        m_ecbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        //BreakableGroupIndex = new NativeArray<int>(1, Allocator.Persistent);

    }

    protected override void OnDestroy()
    {
        //BreakableGroupIndex.Dispose();
    }



    protected override void OnUpdate()
    {

        var inputDeps = new JobHandle();

        inputDeps = JobHandle.CombineDependencies(inputDeps, buildPhysicsWorldSystem.GetOutputDependency());
        inputDeps = JobHandle.CombineDependencies(inputDeps, stepPhysicsWorld.GetOutputDependency());
        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;
        var collisionJob = new BreakableCollisionJob
        {
            physicsWorld = physicsWorld,
            ecb = m_ecbSystem.CreateCommandBuffer(),
            breakableGroup = GetComponentDataFromEntity<BreakableComponent>(false),
            ammoGroup = GetComponentDataFromEntity<AmmoDataComponent>(true),
            gravityGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(false)
            //triggerGroup = GetComponentDataFromEntity<TriggerComponent>(true)
            //breakableGroupIndex = BreakableGroupIndex
        };
        JobHandle collisionHandle = collisionJob.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, inputDeps);
        collisionHandle.Complete();
        

    } 


    struct BreakableCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public ComponentDataFromEntity<AmmoDataComponent> ammoGroup;
        //[ReadOnly] public ComponentDataFromEntity<TriggerComponent> triggerGroup;
        public ComponentDataFromEntity<BreakableComponent> breakableGroup;
        public ComponentDataFromEntity<PhysicsGravityFactor> gravityGroup;
        //public NativeArray<int> breakableGroupIndex;



        public EntityCommandBuffer ecb;
        public void Execute(CollisionEvent ev) // this is never called
        {
            Entity a = ev.EntityA;
            Entity b = ev.EntityB;

            

            if (ammoGroup.HasComponent(b) == true && gravityGroup.HasComponent(a) && breakableGroup.HasComponent(a))
            {

                Debug.Log("collide a ");
                var breakable = breakableGroup[a];
                if (breakable.broken == false)
                {
                    var shooter = ammoGroup[b].Shooter;

                    var gravity = gravityGroup[a];
                    gravity.Value = breakable.gravityFactorAfterBreaking;
                    ecb.SetComponent<PhysicsGravityFactor>(a, gravity);
                    breakable.broken = true;
                    breakable.playEffect = true;
                    //if (triggerGroup.HasComponent(b))
                    //{
                    //  var breakerEntity = triggerGroup[b].ParentEntity;

                    breakable.breakerEntity = shooter;
                    Debug.Log("breaker " + shooter);
                    //}
                    ecb.SetComponent<BreakableComponent>(a, breakable);
                    ecb.AddComponent<BrokenComponent>(a);
                }
            }
            else if (ammoGroup.HasComponent(a) == true && gravityGroup.HasComponent(b) && breakableGroup.HasComponent(b))
            {

                    Debug.Log("collide b ");
            //    var breakable = breakableGroup[b];
            //    var gravity = gravityGroup[b];
            //    gravity.Value = breakable.gravityFactorAfterBreaking;
            //    ecb.SetComponent<PhysicsGravityFactor>(b, gravity);
            //    breakable.broken = true;
            //    ecb.SetComponent<BreakableComponent>(b, breakable);
            //    ecb.AddComponent<BrokenComponent>(b);
            }


        }

        




    }








} // System

[UpdateAfter(typeof(BreakableCollisionSystem))]

public class BreakableCollisionHandlerSystem : SystemBase
{


    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        //var breakableGroup = GetComponentDataFromEntity<BreakableComponent>(false);
        EntityQuery breakablesQuery = GetEntityQuery(ComponentType.ReadOnly<BreakableComponent>());
        NativeArray<Entity> breakableEntities = breakablesQuery.ToEntityArray(Allocator.TempJob);


        JobHandle inputDeps = Entities.ForEach((Entity e, ref BrokenComponent brokenComponent) =>
        {
            if (HasComponent<BreakableComponent>(e))
            {
                int brokenGroupIndex = GetComponent<BreakableComponent>(e).groupIndex;
                for (int i = 0; i < breakableEntities.Length; i++)
                {
                    Entity breakableEntity = breakableEntities[i];
                    var breakableComponent = GetComponent<BreakableComponent>(breakableEntity);
                    if(breakableComponent.groupIndex == brokenGroupIndex && HasComponent<PhysicsGravityFactor>(breakableEntity))
                    {
                        var gravity = GetComponent<PhysicsGravityFactor>(breakableEntity);
                        gravity.Value = breakableComponent.gravityFactorAfterBreaking;
                        ecb.SetComponent(breakableEntity, gravity);
                        breakableComponent.broken = true;
                        ecb.SetComponent(breakableEntity, breakableComponent);
                        //Debug.Log("break " + brokenGroupIndex);
                    }
                }
                ecb.RemoveComponent<BrokenComponent>(e);
                
            }

        }
        ).Schedule(this.Dependency);
        inputDeps.Complete();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        breakableEntities.Dispose();

    }
}