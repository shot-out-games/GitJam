using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;


[System.Serializable]
public struct PowerItemComponent : IComponentData
{
    public Entity pickedUpActor;
    public bool active;
    public bool enabled;
    public int powerType;
    public float speedTimeOn;
    public float speedTimeMultiplier;
    public float healthMultiplier;
    public Entity particleSystemEntity;

}


[System.Serializable]
public class PowerItemClass
{
    public PowerType powerType;

    public bool alive = true;

    [SerializeField]
    public bool active = true;
    //public Entity e;
    EntityManager manager;

    [Header("Speed")]
    public float speedTimeOn = 3.0f;
    public float speedMultiplier = 3.0f;

    [Header("Health")]
    public float healthMultiplier = .75f;

    [Header("Shared")]
    public GameObject powerPrefab;
    public GameObject powerEnabledEffectPrefab;
    public GameObject powerEnabledEffectInstance;

    public AudioClip powerEnabledAudioClip;
    public AudioClip powerTriggerAudioClip;
    public AudioSource audioSource;

    public Mesh mesh;
    public Material material;
    public MeshRenderer meshRenderer;



}



public enum PowerType
{
    None = 0,
    Speed = 1,
    Health = 2,
    Control = 3,
}



public class PowerItem : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<PowerItemClass> powerItems;
  

    void Start()
    {
        for (int i = 0; i < powerItems.Count; i++)
        {

            if (powerItems[i].audioSource) return;
            powerItems[i].audioSource = GetComponent<AudioSource>();
        }

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        for (int i = 0; i < powerItems.Count; i++)
        {
            referencedPrefabs.Add(powerItems[i].powerPrefab);
        }
    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {


        conversionSystem.DeclareLinkedEntityGroup(this.gameObject);

        dstManager.AddComponent<AudioSourceComponent>(entity);


        //e = entity;
        //manager = dstManager;

        for (int i = 0; i < powerItems.Count; i++)
        {


            conversionSystem.AddHybridComponent(powerItems[i].audioSource);
            conversionSystem.AddHybridComponent(this);
            entity = conversionSystem.GetPrimaryEntity(powerItems[i].powerPrefab);

            Debug.Log("power up " + entity);

            dstManager.AddComponentData<PowerItemComponent>(entity, new PowerItemComponent
            {
                particleSystemEntity = conversionSystem.GetPrimaryEntity(powerItems[i].powerEnabledEffectPrefab),
                active = powerItems[i].active,
                powerType = (int)powerItems[i].powerType,
                speedTimeOn = powerItems[i].speedTimeOn,
                speedTimeMultiplier = powerItems[i].speedMultiplier,
                healthMultiplier = powerItems[i].healthMultiplier
            });


            if (powerItems[i].powerType == PowerType.Speed)
            {
                dstManager.AddComponentData(entity, new Speed
                {
                    enabled = false,
                    timer = 0,
                    timeOn = 0,
                    startTimer = false,
                    originalSpeed = 0,
                    multiplier = 0,
                }
               );

            }

            if (powerItems[i].powerType == PowerType.Health)
            {
                dstManager.AddComponentData(entity, new HealthPower
                {
                    enabled = false,
                    healthMultiplier = 0
                }
                );
            }


            if (powerItems[i].powerType == PowerType.Control)
            {

                dstManager.AddComponentData(entity, new ControlPower
                {
                    enabled = false,
                    controlMultiplier = 0
                }
            );

            }

            dstManager.SetSharedComponentData(entity, new RenderMesh() { mesh = powerItems[i].mesh, material = powerItems[i].material });
        }



    }


}
