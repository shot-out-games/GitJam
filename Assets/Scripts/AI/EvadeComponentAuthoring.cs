using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[Serializable]
public struct EvadeComponent : IComponentData
{
    public bool InEvade;
    public float evadeMoveTime;
    public bool randomEvadeMoveTime;
    public float evadeMoveSpeed;
    public float originalEvadeMoveSpeed;
    public bool zMovement;
    public float EvadeMoveTimer;
    public float addX;
    public float addZ;

}



public class EvadeComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float evadeMoveTime = 1.0f;
    public bool zMovement = true;
    public float evadeMoveSpeed = 5;
    public bool randomEvadeMoveTime = true;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new EvadeComponent
            {
                evadeMoveTime = evadeMoveTime,
                originalEvadeMoveSpeed = evadeMoveTime,
                evadeMoveSpeed = evadeMoveSpeed,
                zMovement = zMovement,
                addX = 1,
                addZ = 0,
                randomEvadeMoveTime = randomEvadeMoveTime

            }); ;
        
        
    }
}
