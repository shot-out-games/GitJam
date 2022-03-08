using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;


[System.Serializable]



public struct HealthPower : IComponentData
{
    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool enabled;
    public float healthMultiplier;
}


public class PowerHealthItem : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    //public List<PowerItemClass> powerItems;

    [Header("Health")]
    public bool active = true;
    public float healthMultiplier = .75f;

    //public ItemClass PowerItem;

    public bool immediateUse;
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


        //e = entity;
        //manager = dstManager;



        conversionSystem.AddHybridComponent(audioSource);
        conversionSystem.AddHybridComponent(this);
        //entity = conversionSystem.GetPrimaryEntity(powerPrefab);

        Debug.Log("power up " + entity);

        dstManager.AddComponentData<PowerItemComponent>(entity, new PowerItemComponent
        {
            particleSystemEntity = conversionSystem.GetPrimaryEntity(powerEnabledEffectPrefab),
            active = active,
            description = powerItemDescription,
            pickupEntity = entity
            //powerType = (int)powerItems[i].powerType,
            //speedTimeOn = powerItems[i].speedTimeOn,
            //speedTimeMultiplier = powerItems[i].speedMultiplier,
            //healthMultiplier = powerItems[i].healthMultiplier
        });





        dstManager.AddComponentData(entity, new HealthPower
        {
            enabled = false,
            healthMultiplier = 0
        });

        if(immediateUse)
        {
            dstManager.AddComponent<ImmediateUseComponent>(entity);
        }
        
        //dstManager.SetSharedComponentData(entity, new RenderMesh() { mesh = mesh, material = material });

    }






}
