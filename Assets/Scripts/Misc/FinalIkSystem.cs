using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

//[UpdateInGroup(typeof(LateSimulationSystemGroup))]
//[UpdateAfter(typeof(BossAmmoHandlerSystem))]
//[UpdateAfter(typeof(GunAmmoHandlerSystem))]
//[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateAfter(typeof(EndFrameLocalToParentSystem))]
//[UpdateBefore(typeof(SynchronizeGameObjectTransformsWithTriggerEntities))]
//[UpdateBefore(typeof(EndFrameTRSToLocalToWorldSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
//[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(HybridRendererSystem))]

public class FinalIkSystem : SystemBase
{
    protected override void OnUpdate()
    {


        Entities.WithoutBurst().ForEach((PlayerCombat playerCombat) =>
        {

            playerCombat.LateUpdateSystem();


        }).Run();

        Entities.WithoutBurst().ForEach((EnemyMelee  enemyMelee) =>
        {

            enemyMelee.LateUpdateSystem();


        }).Run();


        Entities.WithoutBurst().ForEach((BossIKScript bossIK) =>
        {

            bossIK.LateUpdateSystem();


        }).Run();
    }
}
