using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public struct BreakableComponent : IComponentData
{
    int value;
    public float damageAmount;
    public bool enemyDamaged;
    public bool playerDamaged;
    public bool instantiated;
    public bool trigger;
    public float currentTime;
    public float spawnTime;
    public bool destroy;
    public float framesToSkip;//timer instead?
    public int frameSkipCounter;
    public int damageEffectsIndex;
    public int deathBlowEffectsIndex;
    public float gravityFactorAfterBreaking;
    public int groupIndex;
    public bool broken;
    public bool playEffect;
    public int effectIndex;
    public Entity breakerEntity;
    public Translation parentTranslation;
    public Entity parentEntity;


}

public class BreakableAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public float damageAmount = 1;
    public float framesToSkip = 30;//timer instead?
    public int damageEffectsIndex;
    public int deathBlowEffectsIndex;
    public float gravityFactorAfterBreaking = 1;
    public int groupIndex = 1;
    public int effectIndex = 0;
    public Transform parent;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //parent needs to be fixed currently self
        // Debug.Log("break " + entity);
        //conversionSystem.DeclareLinkedEntityGroup(this.gameObject);
        //conversionSystem.AddHybridComponent(audioSource);
        //conversionSystem.AddHybridComponent(this);

        BreakableComponent breakable = new BreakableComponent
        {
            damageAmount = damageAmount,
            framesToSkip = framesToSkip,
            damageEffectsIndex = damageEffectsIndex,
            deathBlowEffectsIndex = deathBlowEffectsIndex,
            gravityFactorAfterBreaking = gravityFactorAfterBreaking,
            groupIndex = groupIndex,
            effectIndex = effectIndex,
            parentTranslation = new Translation { Value = new float3(transform.position.x, transform.position.x, transform.position.z) },
            parentEntity = conversionSystem.GetPrimaryEntity(parent.gameObject)


        };

        dstManager.AddComponentData(entity, breakable);
    }


}
