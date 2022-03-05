using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Physics.Extensions;
using UnityEngine.Jobs;

[InternalBufferCapacity(24)]
public struct PickupListBuffer : IBufferElementData
{
    public Entity e;
    public Entity _parent;
    public bool active;
    public bool pickedUp;
    public bool special;//for ld
    public bool reset;
    public bool playerPickupAllowed;
    public bool enemyPickupAllowed;
    public int index;
    public int statl;
    public float3 position;
}


public struct PickupComponent : IComponentData
{
    public Entity e;
    public Entity _parent;
    public bool active;
    public bool pickedUp;
    public bool special;//for ld
    public bool reset;
    public bool playerPickupAllowed;
    public bool enemyPickupAllowed;
    public int index;
    public int statl;

}

public struct PickupManagerComponent : IComponentData //used for managed components - read and then call methods from MB
{
    public bool playSound;
    public bool setAnimationLayer;
}

[System.Serializable]
public class PickupClass
{
    public PickupType type;
    public AudioClip audioClip;
    public GameObject pickupPrefab;
    public GameObject effectPrefab;
    public Transform pickupTransform;
    public int stat1;
    public string description;
    public int index;


}


public enum PickupType
{
    None = 0,
    Speed = 1,
    Health = 2,
    Control = 3,
}



public class PickupManager : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity entity;
    //[SerializeField]
    public AudioSource pickupAudioSource;
    [HideInInspector]
    public List<GameObject> pickupInstances = new List<GameObject>();
    public List<PickupClass> PickupPrefabList = new List<PickupClass>();


    [Header("Misc")]

    public GameObject _parent;


    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        for (int i = 0; i < PickupPrefabList.Count; i++)
        {
            PickupPrefabList[i].index = i;
            gameObjects.Add(PickupPrefabList[i].pickupPrefab);
        }
    }

    void Start()
    {
        //Spawner();

    }

    public void Spawner()
    {
        //Spawn instances based on PickupClass members and transform - nested under in scene pickup manager

        for (int i = 0; i < PickupPrefabList.Count; i++)
        {
            var _transform = PickupPrefabList[i].pickupTransform;
            Instantiate(PickupPrefabList[i].pickupPrefab, _transform);
        }
    }


    void Update()
    {
        //if (manager == default) return;

        //if (!manager.HasComponent(entity, typeof(WeaponComponent))) return;

        //var tracker = manager.GetComponentData<TrackerComponent>(entity);
        //tracker.Position = track.transform.position;
        //tracker.Rotation = track.transform.rotation;
        //manager.SetComponentData(entity, tracker);

    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        //dstManager.AddComponentData<PickupComponent>(
        //    entity,
        //    new PickupComponent()
        //    {

        //    });

        dstManager.AddComponent(entity, typeof(PickupManagerComponent));

        Spawner();
        manager = dstManager;
        this.entity = entity;


        Entity parentEntity = conversionSystem.GetPrimaryEntity(_parent.transform.root.gameObject);

        //example but in this case added when item picked up to add to inventory

        for (int i = 0; i < PickupPrefabList.Count; i++)
        {
                dstManager.AddBuffer<PickupListBuffer>(entity).Add
                (
                   new PickupListBuffer
                   {
                       _parent = parentEntity,
                       index = PickupPrefabList[i].index,
                       //so we can check class with audio clip and other non component parts in a MB type system
                       //description = PickupPrefabList[i].description
                       position = PickupPrefabList[i].pickupTransform.position
                   }
               );
            var prefabEntity = conversionSystem.GetPrimaryEntity(PickupPrefabList[i].pickupPrefab);

            //dstManager.AddComponentData(prefabEntity, new PickupComponent { _parent = parentEntity, index = i });

        }


    }


}


