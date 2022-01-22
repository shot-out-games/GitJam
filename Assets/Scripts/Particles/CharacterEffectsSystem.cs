using Unity.Entities;
//using Unity.Burst;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;
using UnityEngine.AI;

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


[UpdateAfter(typeof(DeadSystem))]

public class CharacterEffectsSystem : SystemBase
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


                if (deadComponent.playDeadEffects)//can probably just use playEffectType in effectsComponent TO DO
                {
                    deadComponent.playDeadEffects = false;
                    int effectsIndex = deadComponent.effectsIndex;
                    Debug.Log("eff ind play " + effectsIndex);

                    if (effects.actorEffect != null)
                    {

                        if (effects.actorEffect[effectsIndex].psInstance)//tryinmg to match index to effect type - 1 is dead
                        {
                            if (effects.actorEffect[effectsIndex].psInstance.isPlaying == false && effectsComponent.playEffectAllowed)
                            {
                                effects.actorEffect[effectsIndex].psInstance.Play(true);
                                if (effects.actorEffect[effectsIndex].clip)
                                {
                                    //effectsComponent.startEffectSound = false;
                                    audioSource.clip = effects.actorEffect[effectsIndex].clip;
                                    if (!audioSource.isPlaying)
                                        audioSource.PlayOneShot(audioSource.clip, .5f);
                                    //Debug.Log("play audio " + audioSource.clip);
                                }

                            }
                            else if (effectsComponent.playEffectAllowed == false)
                            {
                                effects.actorEffect[effectsIndex].psInstance.Stop(true);

                            }
                        }
                    }


                }
                else
                {
                    bool hasDamage = HasComponent<DamageComponent>(e);
                    if (hasDamage == true)
                    {
                        var damageComponent = GetComponent<DamageComponent>(e);
                        int effectsIndex = damageComponent.effectsIndex;//set in attackersystem by readin visualeffect component index
                        Debug.Log("effectsIndex " + effectsIndex);

                        if (damageComponent.DamageReceived <= .0001) return;
                        animator.SetInteger("HitReact", 1);

                        if (effects.actorEffect != null)
                        {
                            if (effects.actorEffect[effectsIndex].psInstance)
                            {
                                effects.actorEffect[effectsIndex].psInstance.Play(true);
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

