using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections.Generic;

public struct ParentComponent : IComponentData
{
    public Translation translation;
    public Entity e;

}

public class ParentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{


    public List<EffectClass> parentEffect;

    [HideInInspector]
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < parentEffect.Count; i++)
        {
            if (parentEffect[i] == null) continue;
            var ps = Instantiate(parentEffect[i].psPrefab);
            ps.transform.parent = transform;
            ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
            parentEffect[i].psInstance = ps;
        }





    }





    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        ParentComponent parent = new ParentComponent
        {
            translation = new Translation { Value = new float3 { x = transform.position.x, y = transform.position.y, z = transform.position.z } },
            e = entity
        };


        dstManager.AddComponentData(entity, parent);

    }

}



