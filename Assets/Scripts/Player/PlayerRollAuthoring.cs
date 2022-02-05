using System;
using TMPro;
using Unity.Entities;
using UnityEngine;



[System.Serializable]

public struct PlayerRollComponent : IComponentData
{


}


public class PlayerRollAuthoring : MonoBehaviour, IConvertGameObjectToEntity

{
   

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlayerRollComponent
            {
            }
        );





    }
}
