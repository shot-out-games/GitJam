using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[Serializable]
public struct EvadeComponent : IComponentData
{

}



public class EvadeComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
         dstManager.AddComponentData(entity, 
             new EvadeComponent
             {
             });
        
        
    }
}
