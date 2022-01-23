using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Audio;


public struct EffectsComponent : IComponentData
{
    public bool pauseEffect;
    public bool soundPlaying;
    public bool playEffectAllowed;
    public EffectType playEffectType;
    public int effectIndex;
}



public class EffectsManager : MonoBehaviour, IConvertGameObjectToEntity
{

    [SerializeField] private bool pauseEffect;


    public List<EffectClass> actorEffect;




    public AudioSource audioSource;
    
    void Start()
    {

        for (int i = 0; i < actorEffect.Count; i++)
        {
            if (actorEffect[i] == null) continue;
            var ps = Instantiate(actorEffect[i].psPrefab);
            //Debug.Log("ps " + ps);
            ps.transform.parent = transform;
            ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
            actorEffect[i].psInstance = ps;
        }





    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new EffectsComponent { pauseEffect = pauseEffect});
    }
}
