using Unity.Entities;
//using Unity.Burst;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;
using UnityEngine.AI;
using Unity.Collections;

public enum EffectType
{
    None,
    Dead,
    Damaged,
    TwoClose,


}
[System.Serializable]
public class EffectClass
{
    public EffectType effectType;
    public ParticleSystem psPrefab;
    public ParticleSystem psInstance;
    public AudioClip clip;
}

public struct PlayAndDestroyEffectComponent : IComponentData
{
    public int effectIndex;
    public bool play;


}


//[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(HealthSystem))]
//[UpdateAfter(typeof(AttackerSystem))]


public class CharacterImpulseEffectsSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //timer += Time.DeltaTime;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();


        //attempt effect hit pause
        Entities.WithoutBurst().ForEach(
            (
                Entity e,
                Animator anim,
                ref ImpulseComponent impulseComponent,
                ref PhysicsVelocity physicsVelocity,
                in Impulse impulse

                ) =>
            {
                //anim null check later
                if (anim != null)
                {
                    float damage = 0;
                    if (HasComponent<DamageComponent>(e))
                    {
                        damage = GetComponent<DamageComponent>(e).DamageLanded;
                    }

                    bool hasNavMesh = anim.gameObject.GetComponent<NavMeshAgent>();

                    if (damage > 0)
                    {
                        if (HasComponent<PlayerComponent>(e)) impulse.impulseSourceHitLanded.GenerateImpulse();
                        impulseComponent.activate = true;
                        ecb.AddComponent<Pause>(e);
                        anim.speed = impulseComponent.animSpeedRatio;
                        if (hasNavMesh) anim.gameObject.GetComponent<NavMeshAgent>().speed = anim.speed;
                        physicsVelocity.Linear = physicsVelocity.Linear * math.float3(anim.speed, anim.speed, anim.speed);

                    }
                    else if (impulseComponent.activate == true && impulseComponent.timer <= impulseComponent.maxTime)
                    {
                        impulseComponent.timer += Time.DeltaTime;
                        //Debug.Log("timer " + impulseComponent.timer);
                        if (impulseComponent.timer >= impulseComponent.maxTime)
                        {
                            impulseComponent.timer = 0;
                            impulseComponent.activate = false;
                            anim.speed = 1;
                            if (hasNavMesh) anim.gameObject.GetComponent<NavMeshAgent>().speed = anim.speed;
                            ecb.RemoveComponent<Pause>(e);
                        }
                    }
                }
                //Debug.Log("shake");


            }
        ).Run();



        //attempt effect hit pause
        Entities.WithoutBurst().ForEach(
            (
                Entity e,
                Animator anim,
                ref ImpulseComponent impulseComponent,
                ref PhysicsVelocity physicsVelocity,
                in Impulse impulse

                ) =>
            {
                //anim null check later
                if (anim != null)
                {
                    float damage = 0;
                    if (HasComponent<DamageComponent>(e))
                    {
                        damage = GetComponent<DamageComponent>(e).DamageReceived;
                    }

                    bool hasNavMesh = anim.gameObject.GetComponent<NavMeshAgent>();

                    if (damage > 0)
                    {
                        impulseComponent.activateOnReceived = true;
                        ecb.AddComponent<Pause>(e);
                        anim.speed = impulseComponent.animSpeedRatioOnReceived;
                        if (hasNavMesh) anim.gameObject.GetComponent<NavMeshAgent>().speed = anim.speed;
                        physicsVelocity.Linear = physicsVelocity.Linear * math.float3(anim.speed, anim.speed, anim.speed);

                    }
                    else if (impulseComponent.activateOnReceived == true && impulseComponent.timerOnReceived <= impulseComponent.maxTimeOnReceived)
                    {
                        impulseComponent.timerOnReceived += Time.DeltaTime;
                        //Debug.Log("timer " + impulseComponent.timer);
                        if (impulseComponent.timerOnReceived >= impulseComponent.maxTimeOnReceived)
                        {
                            impulseComponent.timerOnReceived = 0;
                            impulseComponent.activateOnReceived = false;
                            anim.speed = 1;
                            if (hasNavMesh) anim.gameObject.GetComponent<NavMeshAgent>().speed = anim.speed;
                            ecb.RemoveComponent<Pause>(e);
                        }
                    }
                }
                //Debug.Log("shake");
                //impulse.impulseSourceHitReceived.GenerateImpulse();


            }
        ).Run();





        //Entities.WithoutBurst().WithAll<AudioSourceComponent>().ForEach(
        //    (
        //        PowerItem powerItem, Entity e, AudioSource audioSource, PowerItemComponent powerItemComponent) =>
        //    {
        //        //impulse.impulseSourceHitReceived.GenerateImpulse();
        //        if (audioSource.isPlaying == false && powerItemComponent.enabled)
        //        {
        //            Debug.Log("play ");
        //            audioSource.Play();
        //        }

        //    }
        //).Run();



        Entities.WithoutBurst().ForEach(
            (
                in InputController input, in ControlBarComponent controlBar,
                in Impulse impulse) =>
            {
                //if (input.rightTriggerDown == true && controlBar.value < 25f) 
                //if (pause.value == 1) return;

                //if (input.leftTriggerDown == true || input.rightTriggerDown == true)
                //{
                //impulse.impulseSource.GenerateImpulse();
                //}

            }
        ).Run();





        Entities.WithoutBurst().WithNone<Pause>().ForEach(
            (
                Entity e,
                ref DeadComponent deadComponent,
                ref EffectsComponent effectsComponent,
                in Animator animator,
                in EffectsManager effects) =>
            {


                AudioSource audioSource = effects.audioSource;

                if (effectsComponent.playEffectType == EffectType.TwoClose)
                {
                    //Debug.Log("play effect two close");
                    //effectsComponent.playEffectType = EffectType.None;
                    int effectsIndex = effectsComponent.effectIndex;
                    if (effects.actorEffect != null)
                    {

                        if (effects.actorEffect[effectsComponent.effectIndex].psInstance)//tryinmg to match index to effect type - 3 is 2 close
                        {
                            if (effects.actorEffect[effectsComponent.effectIndex].psInstance.isPlaying == false && effectsComponent.playEffectAllowed)
                            {
                                effects.actorEffect[effectsComponent.effectIndex].psInstance.Play(true);
                                if (effects.actorEffect[effectsComponent.effectIndex].clip)
                                {
                                    //effectsComponent.startEffectSound = false;
                                    audioSource.clip = effects.actorEffect[effectsComponent.effectIndex].clip;
                                    if (!audioSource.isPlaying)
                                        audioSource.PlayOneShot(audioSource.clip, .5f);
                                    //Debug.Log("play audio " + audioSource.clip);
                                }

                            }
                            else if (effectsComponent.playEffectAllowed == false)
                            {
                                effects.actorEffect[effectsComponent.effectIndex].psInstance.Stop(true);


                            }
                        }
                    }
                }


            }
        ).Run();


        Entities.WithoutBurst().WithNone<Pause>().ForEach(
            (
                in Entity e,
                in SlashComponent slashComponent,
                in SlashComponentAuthoring slashComponentAuthoring) =>
            {
                AudioSource audioSource = slashComponentAuthoring.audioSource;
                if (!slashComponentAuthoring.audioClip || !slashComponentAuthoring.audioSource) return;
                if (slashComponent.slashState == (int)SlashStates.Started)
                {
                    audioSource.clip = slashComponentAuthoring.audioClip;
                    audioSource.PlayOneShot(audioSource.clip);
                }
            }
        ).Run();






        Entities.WithoutBurst().ForEach
        (
            (ref EffectsComponent effectsComponent,
                in LevelCompleteComponent goal, in Entity entity, in EffectsManager effects, in AudioSource audioSource) =>
            {
                if (goal.active == true || effectsComponent.soundPlaying == true) return;

            }
        ).Run();

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);




    }




}



//[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(HealthSystem))]
[UpdateAfter(typeof(AttackerSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


public class CharacterDamageEffectsSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    //private float timer;

    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //timer += Time.DeltaTime;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();


        //attempt effect hit pause




        Entities.WithoutBurst().WithNone<Pause>().ForEach(
            (
                Entity e,
                ref DeadComponent deadComponent,
                ref EffectsComponent effectsComponent,
                in Animator animator,
                in EffectsManager effects) =>
            {


                AudioSource audioSource = effects.audioSource;




                bool hasDamage = HasComponent<DamageComponent>(e);
                if (hasDamage == true && deadComponent.isDead == false)
                {
                    var damageComponent = GetComponent<DamageComponent>(e);
                    int effectsIndex = damageComponent.effectsIndex;//set in attackersystem by readin visualeffect component index
                                                                    //Debug.Log("effectsIndex " + effectsIndex);

                    if (damageComponent.DamageReceived <= .0001) return;
                    animator.SetInteger("HitReact", 1);// can easily change to effect index (maybe new field in component ammo and visual effect) if we add more hitreact animations
                    if (effects.actorEffect.Count > 0)
                    {

                        if (effects.actorEffect != null)
                        {
                            if (effects.actorEffect[effectsIndex].psInstance)
                            {
                                effects.actorEffect[effectsIndex].psInstance.Play(true);
                                //Debug.Log("ps dam " + effects.actorEffect[effectsIndex].psInstance);
                            }
                            if (effects.actorEffect[effectsIndex].clip)
                            {
                                audioSource.clip = effects.actorEffect[effectsIndex].clip;
                                audioSource.PlayOneShot(audioSource.clip);
                            }
                        }
                    }

                }

            }
        ).Run();







        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);




    }




}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(HealthSystem))]


public class CharacterDeadEffectsSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    //private float timer;

    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //timer += Time.DeltaTime;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();






        Entities.WithoutBurst().WithNone<Pause>().ForEach(
            (
                Entity e,
                ref DeadComponent deadComponent,
                ref EffectsComponent effectsComponent,
                in Animator animator,
                in EffectsManager effects) =>
            {


                AudioSource audioSource = effects.audioSource;


                //int state = animator.GetInteger("Dead");
                if(deadComponent.isDead)
                {
                    Debug.Log("DEATH PRE");
                }

                if (deadComponent.isDead && deadComponent.playDeadEffects)//can probably just use playEffectType in effectsComponent TO DO
                {
                    bool isEnemy = HasComponent<EnemyComponent>(e);
                    bool isPlayer = HasComponent<PlayerComponent>(e);
                    if (isPlayer) animator.SetInteger("Dead", 1);// can easily change to effect index (maybe new field in component ammo and visual effect) if we add more DEAD animations
                    if (isEnemy) animator.SetInteger("Dead", 2);


                    deadComponent.playDeadEffects = false;
                    //animator.SetInteger("HitReact", 0);
                    //deadComponent.playDeadEffects = false;
                    int effectsIndex = deadComponent.effectsIndex;
                    Debug.Log("DEATH ");

                    if (effects.actorEffect != null)
                    {

                        if (effects.actorEffect[effectsIndex].psInstance)//tryinmg to match index to effect type - 1 is dead
                        {
                            if (effects.actorEffect[effectsIndex].psInstance.isPlaying == false)
                            {
                                effects.actorEffect[effectsIndex].psInstance.Play(true);
                                Debug.Log("ps dead " + effects.actorEffect[effectsIndex].psInstance);
                                if (effects.actorEffect[effectsIndex].clip)
                                {
                                    //effectsComponent.startEffectSound = false;
                                    audioSource.clip = effects.actorEffect[effectsIndex].clip;
                                    if (!audioSource.isPlaying)
                                    {
                                        audioSource.PlayOneShot(audioSource.clip, .5f);
                                        Debug.Log("play audio dead " + audioSource.clip);
                                    }
                                }

                            }
                            else if (effectsComponent.playEffectAllowed == false)
                            {
                                effects.actorEffect[effectsIndex].psInstance.Stop(true);

                            }
                        }
                    }


                }

            }
        ).Run();





        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);




    }




}

[UpdateAfter(typeof(BreakableCollisionHandlerSystem))]

public class BreakableEffectsSystem : SystemBase
{

    //EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    //private float timer;

    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        //  m_EndSimulationEcbSystem = World
        //    .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //timer += Time.DeltaTime;

        //var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);





        Entities.WithoutBurst().ForEach(
            (
                Entity e,
                ref PlayAndDestroyEffectComponent playAndDestroyEffect,
                in ParentAuthoring effects
                ) =>
            {


                AudioSource audioSource = effects.audioSource;
                int effectsIndex = playAndDestroyEffect.effectIndex;
                //Debug.Log("e index " + effectsIndex);
                if (effects.parentEffect != null && effects.parentEffect.Count > 0)
                {

                    if (effects.parentEffect[effectsIndex].psInstance)//tryinmg to match index to effect type - 1 is dead
                    {
                        if (effects.parentEffect[effectsIndex].psInstance.isPlaying == false)
                        {
                            effects.parentEffect[effectsIndex].psInstance.Play(true);
                            //Debug.Log("ps breakable " + effects.parentEffect[effectsIndex].psInstance);
                            if (effects.parentEffect[effectsIndex].clip)
                            {
                                //effectsComponent.startEffectSound = false;
                                audioSource.clip = effects.parentEffect[effectsIndex].clip;
                                if (!audioSource.isPlaying)
                                {
                                    audioSource.PlayOneShot(audioSource.clip, 1.0f);
                                    //Debug.Log("play audio breakable " + audioSource.clip);
                                }
                            }

                        }
                        else
                        {
                            effects.parentEffect[effectsIndex].psInstance.Stop(true);
                            ecb.RemoveComponent<PlayAndDestroyEffectComponent>(e);
                        }
                    }
                }




            }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();



        //m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);




    }




}

