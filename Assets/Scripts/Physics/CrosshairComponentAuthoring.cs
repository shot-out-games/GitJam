using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct CrosshairComponent : IComponentData
{
    public float raycastDistance;
}

public class CrosshairComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float raycastDistance = 140;
    EntityManager manager;
    Entity e;
    void Update()
    {
        if (manager == null || e == Entity.Null) return;
        bool hasComponent = manager.HasComponent<CrosshairComponent>(e);
        if (hasComponent == false) return;

        var crosshairComponent = manager.GetComponentData<CrosshairComponent>(e);
        crosshairComponent.raycastDistance = raycastDistance;
        manager.SetComponentData<CrosshairComponent>(e, crosshairComponent);


    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<CrosshairComponent>(entity, new CrosshairComponent {raycastDistance = raycastDistance });
        manager = dstManager;
        e = entity;
    }



}
