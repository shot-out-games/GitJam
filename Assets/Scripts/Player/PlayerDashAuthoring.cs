using System;
using TMPro;
using Unity.Entities;
using UnityEngine;



[System.Serializable]

public struct PlayerDashComponent : IComponentData
{
    public float power;
    public float dashTime;
    public float DashTimeTicker;
    public float delayTime;
    public float DelayTimeTicker;

}


public class PlayerDashAuthoring : MonoBehaviour, IConvertGameObjectToEntity

{

    public float power = 10;
    public float dashTime = 1;
    public float delayTime = .5f;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlayerDashComponent
        {
            power = power,
            dashTime = dashTime,
            delayTime = delayTime
        }
        );





    }
}
