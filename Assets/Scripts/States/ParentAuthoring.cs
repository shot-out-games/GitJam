using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public struct ParentComponent : IComponentData
{
    public Translation childTranslation;
    Entity e;

}

public class ParentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        ParentComponent parent = new ParentComponent
        {

        };

        dstManager.AddComponentData(entity, parent);
    }


}
