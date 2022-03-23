using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[System.Serializable]



public struct DashPower : IComponentData
{
    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool enabled;
    public int count;
}


public class PowerDashItem : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    //public List<PowerItemClass> powerItems;

    [Header("Dash")]
    public bool active = true;
    public PickupType pickupType = PickupType.Dash;


    public bool immediateUse;
    public GameObject powerEnabledEffectPrefab;
    public GameObject powerEnabledEffectInstance;

    public GameObject powerEnabledPrefab;
    public GameObject powerEnabledInstance;

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
            index = entity.Index,
            pickupType = pickupType
        });


        dstManager.AddComponentData<DashPower>(entity, new DashPower { enabled = false });

        if (immediateUse)
        {
            dstManager.AddComponent<ImmediateUseComponent>(entity);
        }
    }
}
        








