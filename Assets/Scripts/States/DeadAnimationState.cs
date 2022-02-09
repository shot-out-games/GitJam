using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadAnimationState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        Debug.Log("animator dead");     
        animator.SetInteger("Dead", -1);

    }

    
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //// Implement code that processes and affects root motion
        //Vector3 velocity = animator.deltaPosition / Time.deltaTime * (float)animator.GetComponent<PlayerMove>().currentSpeed;
        //velocity.y = animator.GetComponent<Rigidbody>().velocity.y;//pass y from rigibody since rigidbody when on controls y force 
        //animator.GetComponent<Rigidbody>().velocity = velocity;

    }

   
}
