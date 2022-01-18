using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BossIKScript : MonoBehaviour
{
    [Tooltip("Reference to the LookAt component (only used for the head in this instance).")]
    public LookAtIK lookAt;
    public float lookAtWeight = 1;
    public Transform lookAtTarget;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        lookAt.enabled = false;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        //if (lookAt != null)
        //{
        //    //lookAt.solver.Update();
        //    Debug.Log("look LU");
        //}
    }
    public void StartStrike()
    {
        Debug.Log("fireball strike");
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var e = animator.GetComponent<BossComponentAuthoring>().bossEntity;
        var bossComponent = manager.GetComponentData<BossWeaponComponent>(e);
        bossComponent.IsFiring = 1;
        manager.SetComponentData<BossWeaponComponent>(e, bossComponent);
    }
    public void LateUpdateSystem()
    {


        if (lookAt != null)
        {
            lookAt.solver.IKPositionWeight = lookAtWeight;
            lookAt.solver.target = lookAtTarget;
            lookAt.solver.IKPosition = lookAtTarget.position; 
            lookAt.solver.Update();
            Debug.Log("look");
        }


    }
}
