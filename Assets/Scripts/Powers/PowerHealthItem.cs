using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[System.Serializable]



public struct HealthPower : IBufferElementData
{
    public Translation translation;
    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool enabled;
    public float healthMultiplier;
    public int count;
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

    public GameObject powerEnabledPrefab;
    public GameObject powerEnabledInstance;

    public AudioClip powerEnabledAudioClip;
    public AudioClip powerTriggerAudioClip;
    public AudioSource audioSource;
    public string powerItemDescription;

    public List<ItemClass> items = new List<ItemClass>();



    void Start()
    {
        if (audioSource) return;
        audioSource = GetComponent<AudioSource>();

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(powerEnabledEffectPrefab);
        for (int i = 0; i < items.Count; i++)
        {
            referencedPrefabs.Add(items[i].ItemPrefab);

        }
        
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
            pickupEntity = entity,
            index = entity.Index
            //powerType = (int)powerItems[i].powerType,
            //speedTimeOn = powerItems[i].speedTimeOn,
            //speedTimeMultiplier = powerItems[i].speedMultiplier,
            //healthMultiplier = powerItems[i].healthMultiplier
        });

        for (int i = 0; i < items.Count; i++)
        {
           


            dstManager.AddBuffer<HealthPower>(entity).Add
                (
                    new HealthPower
                    {
                       
                        enabled = false,
                        healthMultiplier = healthMultiplier,
                        translation = new Translation { Value = items[i].location.position} 
   
                    }
                );

        }
        

        if (immediateUse)
        {
            dstManager.AddComponent<ImmediateUseComponent>(entity);
        }
    }
}
        


    //    dstManager.AddComponentData(entity, new HealthPower
    //    {
    //        enabled = false,
    //        healthMultiplier = healthMultiplier
    //    });

    //    if(immediateUse)
    //    {
    //        dstManager.AddComponent<ImmediateUseComponent>(entity);
    //    }
        
    //    //dstManager.SetSharedComponentData(entity, new RenderMesh() { mesh = mesh, material = material });

    //}







