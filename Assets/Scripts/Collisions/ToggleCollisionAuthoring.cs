using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public struct ToggleCollisionComponent : IComponentData
{


}
public struct ActorCollisionComponent : IComponentData
{

    public bool isPlayer;
}

public class ToggleCollisionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{


    public bool isPlayer = true;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ActorCollisionComponent
        {
            isPlayer = isPlayer

        });

        dstManager.AddComponent<ToggleCollisionComponent>(entity);

    }


}


