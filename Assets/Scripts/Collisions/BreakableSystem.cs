using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateBefore(typeof(CollisionSystem))]
//[UpdateBefore(typeof(AttackerSystem))]


public class BreakableDamageSystem : SystemBase
{

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);





        Entities.WithoutBurst().ForEach(
        (
            //Animator animator,
            //HealthBar healthBar,
            //in DeadComponent dead,
            in CollisionComponent collisionComponent,
            in Entity entity


        ) =>
        {
            //if (dead.isDead == true) return;


            int type_a = collisionComponent.Part_entity;
            int type_b = collisionComponent.Part_other_entity;
            Entity collision_entity_a = collisionComponent.Character_entity;
            Entity collision_entity_b = collisionComponent.Character_other_entity;


            //Debug.Log("breakable a " + collision_entity_a + " type a " + type_a);
           // Debug.Log("breakable b " + collision_entity_b + " type b" + type_b);




            if (type_a == (int)TriggerType.Breakable && HasComponent<TriggerComponent>(collision_entity_a)
                                             && HasComponent<TriggerComponent>(collision_entity_b)) //b is damage effect so causes damage to entity
            {
                //Debug.Log("breakable a " + collision_entity_a + " type a " + type_a);
                //Debug.Log("breakable b " + collision_entity_b + " type b" + type_b);


                var breakableComponent = GetComponent<BreakableComponent>(collision_entity_a);


                float damage = 0;
                int effectsIndex = 0;

                bool skip = false;
                //if (visualEffectComponent.frameSkipCounter < visualEffectComponent.framesToSkip)
                if (breakableComponent.frameSkipCounter == 0)
                {
                    breakableComponent.frameSkipCounter += 1;
                    skip = false;
                }
                else if (breakableComponent.frameSkipCounter < breakableComponent.framesToSkip)

                {
                    breakableComponent.frameSkipCounter += 1;
                    skip = true;
                }
                else if (breakableComponent.frameSkipCounter >= breakableComponent.framesToSkip)

                {
                    breakableComponent.frameSkipCounter = 0;
                    skip = true;
                }

                if (skip == false)
                {
                    damage = breakableComponent.damageAmount;
                    //effectsIndex = (int)EffectType.Damaged;
                    effectsIndex = breakableComponent.damageEffectsIndex;//???
                }
                Debug.Log("breakable damage " + damage);

                if (HasComponent<DeadComponent>(collision_entity_b) == false ||
                    GetComponent<DeadComponent>(collision_entity_b).isDead)
                {
                    damage = 0;
                }





                ecb.AddComponent<DamageComponent>(collision_entity_b,
                        new DamageComponent
                        { DamageLanded = 0, DamageReceived = damage, StunLanded = damage, entityCausingDamage = collision_entity_b, effectsIndex = effectsIndex });




                ecb.SetComponent<BreakableComponent>(collision_entity_a, breakableComponent);


            }








        }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();


        //return default;
    }

}

