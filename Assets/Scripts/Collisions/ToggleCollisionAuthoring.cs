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
    public Entity _parent;
}

public class ToggleCollisionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{


    public bool isPlayer = true;
    public GameObject _parent;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity parentEntity = conversionSystem.GetPrimaryEntity(_parent.transform.root.gameObject);
        dstManager.AddComponentData(entity, new ActorCollisionComponent
        {
            isPlayer = isPlayer, 
            _parent = parentEntity
            

        });

        dstManager.AddComponent<ToggleCollisionComponent>(entity);

    }


}


