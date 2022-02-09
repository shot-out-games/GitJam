using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;




public class LocomotionState : StateMachineBehaviour
{
    public AnimationType animationType;



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (animationType == AnimationType.Aim)
        {
            animator.SetInteger("WeaponRaised", (int)WeaponMotion.Raised);
            //Debug.Log("event aim");
        }

        

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{




    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationType == AnimationType.Lowering)
        {
            animator.SetInteger("WeaponRaised", (int)WeaponMotion.None);
            //Debug.Log("event lowered ");
        }
        if (animationType == AnimationType.JumpStart)
        {
            animator.SetInteger("JumpState", 0);
            //animator.SetInteger("Dash", 0);
            //Debug.Log("event lowered ");
        }




        //Debug.Log("loco exit");
        //if (animationType == AnimationType.BossStrike)
        //{
        //  animator.SetInteger("Strike Type", 0);
        //Debug.Log("event aim");
        //}

        //if (animationType == AnimationType.Aim)
        //{
        //    animator.SetInteger("WeaponRaised", (int)WeaponMotion.Raised);
        //    //Debug.Log("event aim");
        //}


        //if (animationType == AnimationType.BossStrike)
        //{
        //    //Debug.Log("fireball end");
        //    //EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //    //var e = animator.GetComponent<BossComponentAuthoring>().bossEntity;
        //    //var bossComponent = manager.GetComponentData<BossWeaponComponent>(e);
        //    //bossComponent.IsFiring = 1;
        //    //manager.SetComponentData<BossWeaponComponent>(e, bossComponent);

        //}

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{



    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

}