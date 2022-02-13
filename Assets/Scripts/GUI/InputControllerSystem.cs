using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;



//[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]

public class InputControllerSystemUpdate : SystemBase
{

    //FixedStepSimulationSystemGroup fixedStepSimulationSystemGroup;

    protected override void OnCreate()
    {
        //fixedStepSimulationSystemGroup = World.GetOrCreateSystem<FixedStepSimulationSystemGroup>();
        //fixedStepSimulationSystemGroup.Timestep = 60;
        //var group = World.GetOrCreateSystem<FixedStepSimulationSystemGroup>();
        //group.Timestep = 1 / 60;
    }

    protected override void OnUpdate()
    {


        Entities.WithoutBurst().WithAll<PlayerComponent, DeadComponent>().ForEach((InputController inputController) =>
        {

                inputController.UpdateSystem();


        }).Run();
    }
}




//[UpdateInGroup(typeof(InitializationSystemGroup))]
//[UpdateAfter(typeof(BuildPhysicsWorld))]
//[UpdateBefore(typeof(BuildPhysicsWorld))]
public class TimeStep : SystemBase
{
    protected override void OnCreate()
    {
        var group = World.GetExistingSystem<FixedStepSimulationSystemGroup>();
        //group.Timestep = 1/120f;
    }


    protected override void OnUpdate()
    {



    }
}



