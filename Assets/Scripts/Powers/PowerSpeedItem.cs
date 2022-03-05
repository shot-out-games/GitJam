using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;






//[System.Serializable]
//public struct PowerHealthItemComponent : IComponentData
//{
//    public float healthMultiplier;

//}


//[System.Serializable]
//public class PowerHealthItemClass
//{
//    public PowerType powerType;

//    public bool alive = true;
//    [SerializeField]


//    [Header("Health")]
//    public float healthMultiplier = .75f;

//    [Header("Shared")]
//    public GameObject powerPrefab;
//    public GameObject powerEnabledEffectPrefab;
//    public GameObject powerEnabledEffectInstance;

//    public AudioClip powerEnabledAudioClip;
//    public AudioClip powerTriggerAudioClip;
//    public AudioSource audioSource;

//    public Mesh mesh;
//    public Material material;
//    public MeshRenderer meshRenderer;



//}



public class PowerSpeedItem : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    //public List<PowerItemClass> powerItems;

    public bool active = true;

    public float speedTimeOn;
    public float speedTimeMultiplier;

    //public GameObject powerPrefab;
    public GameObject powerEnabledEffectPrefab;
    public GameObject powerEnabledEffectInstance;
    public AudioClip powerEnabledAudioClip;
    public AudioClip powerTriggerAudioClip;
    public AudioSource audioSource;


    //public Mesh mesh;
    //public Material material;
    //public MeshRenderer meshRenderer;


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
            //powerType = (int)powerItems[i].powerType,
            //speedTimeOn = powerItems[i].speedTimeOn,
            //speedTimeMultiplier = powerItems[i].speedMultiplier,
            //healthMultiplier = powerItems[i].healthMultiplier
        });



        dstManager.AddComponentData(entity, new Speed
        {
            enabled = false,
            timeOn = speedTimeOn,
            multiplier = speedTimeMultiplier
        });



        //dstManager.SetSharedComponentData(entity, new RenderMesh() { mesh = mesh, material = material });

    }
















}

