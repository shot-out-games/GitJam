using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;





public class PowerSpeedItem : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{


    public bool active = true;
    public bool immediateUse;

    public float speedTimeOn;
    public float speedTimeMultiplier;

    //public GameObject powerPrefab;
    public GameObject powerEnabledEffectPrefab;
    public GameObject powerEnabledEffectInstance;
    public AudioClip powerEnabledAudioClip;
    public AudioClip powerTriggerAudioClip;
    public AudioSource audioSource;
    public string powerItemDescription;


    void Start()
    {
        if (audioSource) return;
        audioSource = GetComponent<AudioSource>();

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(powerEnabledEffectPrefab);
    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {


        conversionSystem.DeclareLinkedEntityGroup(this.gameObject);

        dstManager.AddComponent<AudioSourceComponent>(entity);



        conversionSystem.AddHybridComponent(audioSource);
        conversionSystem.AddHybridComponent(this);


        Debug.Log("power up " + entity);

        dstManager.AddComponentData<PowerItemComponent>(entity, new PowerItemComponent
        {
            particleSystemEntity = conversionSystem.GetPrimaryEntity(powerEnabledEffectPrefab),
            active = active,
            description = powerItemDescription,
            pickupEntity = entity,
            index = entity.Index


        });



        dstManager.AddComponentData(entity, new Speed
        {
            enabled = false,
            timeOn = speedTimeOn,
            multiplier = speedTimeMultiplier
        });

        if (immediateUse)
        {
            dstManager.AddComponent<ImmediateUseComponent>(entity);
        }



    }
















}

