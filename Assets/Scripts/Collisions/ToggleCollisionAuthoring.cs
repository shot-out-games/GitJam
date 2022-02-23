using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public struct ToggleCollisionComponent : IComponentData
{


}
public struct ActorCollisionBufferElement : IBufferElementData
{

    public bool isPlayer;
    public Entity _parent;
    public Entity _child;
}

public class ToggleCollisionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{


    public bool isPlayer = true;
    public GameObject _parent;

    void Update()
    {


    }
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity parentEntity = conversionSystem.GetPrimaryEntity(_parent.transform.root.gameObject);
        //dstManager.AddComponentData(parentEntity, new ActorCollisionComponent
        //{
        //    isPlayer = isPlayer,
        //    _parent = parentEntity,
        //    _child = entity


        //});

        dstManager.AddBuffer<ActorCollisionBufferElement>(parentEntity).Add
             (
                 new ActorCollisionBufferElement
                 {
                     isPlayer = isPlayer,
                     _parent = parentEntity,
                     _child = entity

                 }
             );


        dstManager.AddComponent<ToggleCollisionComponent>(parentEntity);

    }


}


