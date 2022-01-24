using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateAfter(typeof(CollisionSystem))]



public class AttackerSystem : SystemBase
{
    //public int counta;
    //public int countb;

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);



        Entities.WithoutBurst().ForEach(
            (
                ref CheckedComponent checkedComponent

            ) =>
            {
                int countDown = 60;
                if (checkedComponent.collisionChecked == true && checkedComponent.timer < countDown)
                {
                    checkedComponent.timer++;
                }
                else
                {
                    checkedComponent.collisionChecked = false;
                    checkedComponent.timer = 0;
                }



            }
            ).Run();








        Entities.WithoutBurst().ForEach(
        (
            Animator animator,
            HealthBar healthBar,
            in DeadComponent dead,
            in CollisionComponent collisionComponent,
            in Entity entity


        ) =>
        {

            Debug.Log("attacker 0");
            if (dead.isDead == true) return;
            Debug.Log("attacker 1");


            int type_a = collisionComponent.Part_entity;
            int type_b = collisionComponent.Part_other_entity;
            Entity collision_entity_a = collisionComponent.Character_entity;
            Entity collision_entity_b = collisionComponent.Character_other_entity;
            bool isMelee = collisionComponent.isMelee;

            Entity entityA = collision_entity_a;
            Entity entityB = collision_entity_b;



            bool playerA = HasComponent<PlayerComponent>(collision_entity_a);
            bool playerB = HasComponent<PlayerComponent>(collision_entity_b);
            bool enemyA = HasComponent<EnemyComponent>(collision_entity_a);
            bool enemyB = HasComponent<EnemyComponent>(collision_entity_b);

            bool check = false;

            float hw = animator.GetFloat("HitWeight");
            if (playerA && enemyB || playerB && enemyA) check = true;


            if (check == true)
            {

                var trigger_a = GetComponent<CheckedComponent>(entityA);
                if (trigger_a.collisionChecked == false)
                {

                    float hitPower = 10;//need to be able to change eventually
                    if (HasComponent<RatingsComponent>(entityA))
                    {
                        hitPower = GetComponent<RatingsComponent>(entityA).hitPower;
                    }
                    if (HasComponent<HealthComponent>(entityA))
                    {
                        bool alwaysDamage = GetComponent<HealthComponent>(entityA).AlwaysDamage;
                        if (alwaysDamage) hw = 1;//
                    }
                    float damage = hitPower * hw;

                    ecb.AddComponent<DamageComponent>(entityA,
                        new DamageComponent { DamageLanded = damage, DamageReceived = 0, entityCausingDamage = entityA });


                    ecb.AddComponent<DamageComponent>(entityB,
                        new DamageComponent { DamageLanded = 0, DamageReceived = damage, entityCausingDamage = entityA });



                    if (HasComponent<SkillTreeComponent>(entityA))
                    {
                        var skill = GetComponent<SkillTreeComponent>(entityA);
                        skill.CurrentLevelXp += damage;
                        SetComponent<SkillTreeComponent>(entityA, skill);
                    }


                    if (HasComponent<ScoreComponent>(entityA) && damage != 0)
                    {

                        var scoreComponent = GetComponent<ScoreComponent>(entityA);
                        scoreComponent.pointsScored = true;
                        scoreComponent.scoredAgainstEntity = entityB;
                        SetComponent(entityA, scoreComponent);
                    }




                    trigger_a.collisionChecked = true;



                    ecb.SetComponent<CheckedComponent>(entityA, trigger_a);
                }


            }



            Debug.Log("ty b attacker " + type_b + " ty a attacker " + type_a);

            if (type_b == (int)TriggerType.Ammo && HasComponent<TriggerComponent>(collision_entity_a)
                                                && HasComponent<TriggerComponent>(collision_entity_b)) //b is ammo so causes damage to entity
            {
                Entity shooter = Entity.Null;
                shooter = GetComponent<TriggerComponent>(collision_entity_b)
                    .ParentEntity;

                if (shooter != Entity.Null && HasComponent<AmmoComponent>(collision_entity_b))
                {
                    bool isEnemyShooter = HasComponent<EnemyComponent>(shooter);
                    Entity target = GetComponent<TriggerComponent>(collision_entity_a)
                        .ParentEntity;
                    bool isEnemyTarget = HasComponent<EnemyComponent>(target);
                    AmmoComponent ammo =
                        GetComponent<AmmoComponent>(collision_entity_b);
                    AmmoDataComponent ammoData =
                        GetComponent<AmmoDataComponent>(collision_entity_b);

                    float damage = 0;//why using enemy data and not ammo data ?? change this
                    damage = ammoData.GameDamage;//overrides previous
                    ammo.AmmoDead = true;


                    if (ammo.DamageCausedPreviously && ammo.frameSkipCounter > ammo.framesToSkip)//count in ammosystem
                    {
                        ammo.DamageCausedPreviously = false;
                        ammo.frameSkipCounter = 0;
                    }


                    if (ammo.DamageCausedPreviously || ammoData.ChargeRequired == true && ammo.Charged == false ||
                        isEnemyShooter == isEnemyTarget
                        )
                    {

                        damage = 0;
                    }

                    if (HasComponent<DeadComponent>(collision_entity_a) == false ||
                    GetComponent<DeadComponent>(collision_entity_a).isDead)
                    {
                        damage = 0;
                    }


                    ammo.DamageCausedPreviously = true;

                    ecb.AddComponent<DamageComponent>(shooter,
                            new DamageComponent
                            { DamageLanded = damage, DamageReceived = 0, entityCausingDamage = collision_entity_b });


                    ecb.AddComponent<DamageComponent>(collision_entity_a,
                            new DamageComponent
                            {
                                DamageLanded = 0,
                                DamageReceived = damage,
                                StunLanded = damage,
                                effectsIndex = ammo.effectIndex,

                                entityCausingDamage = collision_entity_b
                            });

                    if (HasComponent<SkillTreeComponent>(shooter))
                    {
                        var skill = GetComponent<SkillTreeComponent>(shooter);
                        skill.CurrentLevelXp += damage;
                        SetComponent<SkillTreeComponent>(shooter, skill);
                    }



                    if (HasComponent<ScoreComponent>(shooter) && damage != 0)
                    {

                        var scoreComponent = GetComponent<ScoreComponent>(shooter);
                        scoreComponent.addBonus = 0;
                        //for gmtk bonus for charged (blocked)
                        if (ammo.Charged == true && isEnemyShooter == false && isEnemyTarget == true)
                        {
                            scoreComponent.addBonus = scoreComponent.defaultPointsScored * 1;
                            ammo.Charged = false;

                        }

                        scoreComponent.scoringAmmoEntity = ammo.ammoEntity;
                        scoreComponent.pointsScored = true;
                        scoreComponent.scoredAgainstEntity = collision_entity_a;
                        SetComponent(shooter, scoreComponent);
                    }

                    ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);


                }
            }



            if (type_b == (int)TriggerType.Particle && HasComponent<TriggerComponent>(collision_entity_a)
                                             && HasComponent<TriggerComponent>(collision_entity_b)) //b is damage effect so causes damage to entity
            {
                Entity shooter = Entity.Null;
                shooter = GetComponent<TriggerComponent>(collision_entity_b)
                    .ParentEntity;

                if (shooter != Entity.Null && HasComponent<VisualEffectEntityComponent>(collision_entity_b))
                {
                    bool isEnemyShooter = HasComponent<EnemyComponent>(shooter);
                    Entity target = GetComponent<TriggerComponent>(collision_entity_a)
                        .ParentEntity;
                    bool isEnemyTarget = HasComponent<EnemyComponent>(target);
                    var visualEffectComponent = GetComponent<VisualEffectEntityComponent>(collision_entity_b);

                    float damage = 0;
                    int effectsIndex = 0;
                    bool skip = false;
                    //if (visualEffectComponent.frameSkipCounter < visualEffectComponent.framesToSkip)
                    if (visualEffectComponent.frameSkipCounter == 0)
                    {
                        visualEffectComponent.frameSkipCounter += 1;
                        skip = false;
                    }
                    else if (visualEffectComponent.frameSkipCounter < visualEffectComponent.framesToSkip)

                    {
                        visualEffectComponent.frameSkipCounter += 1;
                        skip = true;
                    }
                    else if (visualEffectComponent.frameSkipCounter >= visualEffectComponent.framesToSkip)

                    {
                        visualEffectComponent.frameSkipCounter = 0;
                        skip = true;
                    }

                    if (skip == false)
                    {
                        damage = visualEffectComponent.damageAmount;
                        //effectsIndex = (int)EffectType.Damaged;
                        effectsIndex = visualEffectComponent.effectsIndex;//???
                    }
                    Debug.Log("effect damage " + damage);

                    if (HasComponent<DeadComponent>(collision_entity_a) == false ||
                        GetComponent<DeadComponent>(collision_entity_a).isDead)
                    {
                        damage = 0;
                    }



                    ecb.AddComponent<DamageComponent>(shooter,
                            new DamageComponent
                            {
                                DamageLanded = damage,
                                DamageReceived = 0,
                                entityCausingDamage = collision_entity_b

                            });


                    ecb.AddComponent<DamageComponent>(collision_entity_a,
                            new DamageComponent
                            { DamageLanded = 0, DamageReceived = damage, StunLanded = damage, entityCausingDamage = collision_entity_b, effectsIndex = effectsIndex });

                    if (HasComponent<SkillTreeComponent>(shooter))
                    {
                        var skill = GetComponent<SkillTreeComponent>(shooter);
                        skill.CurrentLevelXp += damage;
                        SetComponent<SkillTreeComponent>(shooter, skill);
                    }



                    if (HasComponent<ScoreComponent>(shooter) && damage != 0)
                    {

                        var scoreComponent = GetComponent<ScoreComponent>(shooter);
                        scoreComponent.addBonus = 0;
                        scoreComponent.pointsScored = true;
                        scoreComponent.scoredAgainstEntity = collision_entity_a;
                        SetComponent(shooter, scoreComponent);
                    }

                    ecb.SetComponent<VisualEffectEntityComponent>(collision_entity_b, visualEffectComponent);


                }
            }


        }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();

    }

}

