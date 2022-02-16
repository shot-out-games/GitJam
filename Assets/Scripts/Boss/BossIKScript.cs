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

    public AimIK aimIk;
    public float aimIkWeight = 1;
    public Transform aimIkTarget;

    //int count = 0;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if(lookAt) lookAt.enabled = false;
        if(aimIk) aimIk.enabled = false;
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
        //count++;
        //Debug.Log("lu " + count);

    }
    public void StartStrike()//any animation 
    {
        //Debug.Log("fireball strike");
        
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var e = animator.GetComponent<BossComponentAuthoring>().bossEntity;
        if (manager == null || e == Entity.Null) return;
        if (manager.HasComponent<BossWeaponComponent>(e) == false) return;
        var bossComponent = manager.GetComponentData<BossWeaponComponent>(e);
        bossComponent.IsFiring = 1;
        manager.SetComponentData<BossWeaponComponent>(e, bossComponent);
    }
    public void LateUpdateSystem()
    {
        if (lookAt != null && lookAtTarget)
        {
            lookAt.solver.IKPositionWeight = lookAtWeight;
            //lookAt.solver.target = lookAtTarget;
            lookAt.solver.IKPosition = lookAtTarget.position;
            lookAt.solver.Update();
            //Debug.Log("look");
        }

        if (aimIk != null && aimIkTarget)
        {
            aimIk.solver.IKPositionWeight = aimIkWeight;
            //aimIk.solver.target = aimIkTarget;
            aimIk.solver.IKPosition = aimIkTarget.position;
            aimIk.solver.Update();
        }

        //count++;
        //Debug.Log("lu system " + count);


        //Debug.Log("look sb");



    }
}
