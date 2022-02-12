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

[InternalBufferCapacity(8)]

public struct BossWaypointBufferElement : IBufferElementData
{
    public float3 wayPointPosition;
    public float wayPointSpeed;
    public bool wayPointChase;
    public int wayPointAction;
    public int wayPointAnimation;
    public int weaponListIndex;
    public int ammoListIndex;
    public float duration;//n/a
}

public struct BossWeaponComponent : IComponentData
{
    public Entity PrimaryAmmo;
    public Entity Weapon;
    public float gameStrength;
    public float gameDamage;
    public float gameRate;
    public float Strength;
    public float Damage;
    public float Rate;
    public float Duration;//rate counter for job
    public bool CanFire;
    public int IsFiring;
    //public LocalToWorld AmmoStartLocalToWorld;
    public Translation AmmoStartPosition;
    public Rotation AmmoStartRotation;
    public bool Disable;
    public float ChangeAmmoStats;
    public float CurrentWaypointIndex; 
}

public struct BossAmmoManagerComponent : IComponentData //used for managed components - read and then call methods from MB
{
    public bool playSound;
    public bool setAnimationLayer;
}



public class BossAmmoManager : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity entity;
    //[SerializeField]
    public AudioSource weaponAudioSource;
    //[HideInInspector]
    //public List<GameObject> AmmoInstances = new List<GameObject>();
    public GameObject PrimaryAmmoPrefab;
    public List<AmmoClass> AmmoPrefabList = new List<AmmoClass>();

    public AudioClip weaponAudioClip;

    [Header("Ammo Ratings")]
    [SerializeField]
    //bool randomize;
    float AmmoTime;
    float Strength;
    float Damage;
    float Rate;

    
    void Start()
    {
        //PrimaryAmmoPrefab = AmmoPrefabList[0].primaryAmmoPrefab;
    }
    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        //gameObjects.Add(PrimaryAmmoPrefab);
        for (int i = 0; i < AmmoPrefabList.Count ; i++)
        {
            gameObjects.Add(AmmoPrefabList[i].primaryAmmoPrefab);
        }
        //gameObjects.Add(WeaponPrefab);
    }

    void Generate(bool randomize)
    {
        PrimaryAmmoPrefab = AmmoPrefabList[0].primaryAmmoPrefab;

        if (randomize)
        {
            float multiplier = .7f;
            Strength = UnityEngine.Random.Range(Strength * multiplier, Strength * (2 - multiplier));
            Damage = UnityEngine.Random.Range(Damage * multiplier, Damage * (2 - multiplier));
            Rate = UnityEngine.Random.Range(Rate * multiplier, Rate * (2 - multiplier));
        }
        else
        {
            Strength = PrimaryAmmoPrefab.GetComponent<AmmoData>().Strength;
            Damage = PrimaryAmmoPrefab.GetComponent<AmmoData>().Damage;
            Rate = PrimaryAmmoPrefab.GetComponent<AmmoData>().Rate;
            //Debug.Log("dam " + Damage);
        }


    }



    void Update()
    {
  
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Generate(false);



        for (int i = 0; i < AmmoPrefabList.Count; i++)
        {
            var ltw = new LocalToWorld
            {
                Value = float4x4.TRS(AmmoPrefabList[i].ammoStartLocation.position, AmmoPrefabList[i].ammoStartLocation.rotation, Vector3.one)
            };


            dstManager.AddBuffer<BossAmmoListBuffer>(entity).Add
                (
                    new BossAmmoListBuffer
                    {
                        e = conversionSystem.GetPrimaryEntity(AmmoPrefabList[i].primaryAmmoPrefab),
                        ammoStartLocalToWorld = ltw,
                        ammoStartPosition = new Translation() { Value = AmmoPrefabList[i].ammoStartLocation.position },
                        ammoStartRotation = new Rotation() { Value = AmmoPrefabList[i].ammoStartLocation.rotation }

                    }
                );

        }

        var localToWorld = new LocalToWorld
        {
            Value = float4x4.TRS(AmmoPrefabList[0].ammoStartLocation.position, AmmoPrefabList[0].ammoStartLocation.rotation, Vector3.one)
        };


        dstManager.AddComponentData<BossWeaponComponent>(
            entity,
            new BossWeaponComponent()
            {
                //AmmoStartLocalToWorld = localToWorld,
                AmmoStartPosition = new Translation() { Value = AmmoPrefabList[0].ammoStartLocation.position },
                AmmoStartRotation = new Rotation() { Value = AmmoPrefabList[0].ammoStartLocation.rotation },
                PrimaryAmmo = conversionSystem.GetPrimaryEntity(PrimaryAmmoPrefab),
                Strength = Strength,
                gameStrength = Strength,
                Damage = Damage,
                Rate = Rate,
                gameDamage = Damage,
                gameRate = Rate,
                CanFire = true,
                IsFiring = 0
                

            });

        dstManager.AddComponent(entity, typeof(BossAmmoManagerComponent));
        manager = dstManager;
        this.entity = entity;

    }


}


