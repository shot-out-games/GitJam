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
    public LocalToWorld AmmoStartLocalToWorld;
    public Translation AmmoStartPosition;
    public Rotation AmmoStartRotation;
    public bool Disable;
    public float ChangeAmmoStats;
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
    [HideInInspector]
    public List<GameObject> AmmoInstances = new List<GameObject>();
    public Transform AmmoStartLocation;
    public GameObject PrimaryAmmoPrefab;
    //public List<GameObject> AmmoPrefabList = new List<GameObject>();

    public AudioClip weaponAudioClip;

    [Header("Ammo Ratings")]
    [SerializeField]
    //bool randomize;
    float AmmoTime;
    float Strength;
    float Damage;
    float Rate;

    

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(PrimaryAmmoPrefab);
        //gameObjects.Add(WeaponPrefab);
    }

    void Generate(bool randomize)
    {
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


        var localToWorld = new LocalToWorld
        {
            Value = float4x4.TRS(AmmoStartLocation.position, AmmoStartLocation.rotation, Vector3.one)
        };


        dstManager.AddComponentData<BossWeaponComponent>(
            entity,
            new BossWeaponComponent()
            {
                AmmoStartLocalToWorld = localToWorld,
                AmmoStartPosition = new Translation() { Value = AmmoStartLocation.position },//not used because cant track bone 
                AmmoStartRotation = new Rotation() { Value = AmmoStartLocation.rotation },
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



[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(EndFrameLocalToParentSystem))]


class SynchronizeGameObjectTransformsBossAmmoWeaponEntities : SystemBase
{

    protected override void OnCreate()
    {
        base.OnCreate();

    }

    protected override void OnUpdate()
    {

        Entities.WithoutBurst().ForEach(
            (BossAmmoManager bulletManager, ref BossWeaponComponent gunComponent) =>
            {
                var localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(bulletManager.AmmoStartLocation.position, bulletManager.AmmoStartLocation.rotation, Vector3.one)
                };


                gunComponent.AmmoStartLocalToWorld = localToWorld;
                gunComponent.AmmoStartPosition.Value = bulletManager.AmmoStartLocation.position;
                gunComponent.AmmoStartRotation.Value = bulletManager.AmmoStartLocation.rotation;
            }
        ).Run();

    }
}



public class BossAmmoManagerSystem : SystemBase
{


    protected override void OnUpdate()
    {



        Entities.WithoutBurst().ForEach(
            (
                 Entity e,
                 BossAmmoManager bulletManager,
                 Animator animator,
                 ref BossAmmoManagerComponent bulletManagerComponent
                 ) =>
            {



                if (bulletManager.weaponAudioClip && bulletManager.weaponAudioSource && bulletManagerComponent.playSound)
                {
                    bulletManager.weaponAudioSource.PlayOneShot(bulletManager.weaponAudioClip, .25f);
                    bulletManagerComponent.playSound = false;
                }

                if (bulletManagerComponent.setAnimationLayer)
                {
                    animator.SetLayerWeight(0, 0);
                    bulletManagerComponent.setAnimationLayer = false;
                }



            }
        ).Run();


    }

}
