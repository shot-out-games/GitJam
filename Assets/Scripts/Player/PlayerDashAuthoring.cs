using System;
using TMPro;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;



[System.Serializable]

public struct PlayerDashComponent : IComponentData
{
    public float power;
    public float dashTime;
    public float DashTimeTicker;
    public float delayTime;
    public float DelayTimeTicker;
    public float invincibleStart;
    public float invincibleEnd;
    public PhysicsCollider Collider;
    public BlobAssetReference<Unity.Physics.Collider> box;
    public bool Invincible;
    public bool InDash;

}

public struct Invincible : IComponentData
{
    public int Value;
}


public class PlayerDashAuthoring : MonoBehaviour, IConvertGameObjectToEntity

{
    public BlobAssetReference<Unity.Physics.Collider> box;
    public float power = 10;
    public float dashTime = 1;
    public float delayTime = .5f;
    public float invincibleStart = .1f;
    public float invincibleEnd = 1f;

    public AudioSource audioSource;
    public AudioClip clip;
    public ParticleSystem ps;



    void Start()
    {
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlayerDashComponent
        {
            power = power,
            dashTime = dashTime,
            delayTime = delayTime,
            invincibleStart = invincibleStart,
            invincibleEnd = invincibleEnd
        }
        ); ; ; ;





    }
}
