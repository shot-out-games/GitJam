using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dialogue
{
    public class DialogueAction : MonoBehaviour
    {
        Animator animator;
        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            //DialogueManager.ShowAlert("HEY YOU");
        }

        public void OnConversationStart(Transform actor)
        {
            Debug.Log("conversation start");
            animator.SetInteger("DialogState", 1);
        }

        public void OnConversationEnd(Transform actor)
        {
            Debug.Log("conversation end");
            animator.SetInteger("DialogState", 0);
        }
        public void OnUsableStart()
        {
            Debug.Log("usable start");
            animator.SetInteger("DialogState", 1);
        }

        public void OnUsableEnd()
        {
            Debug.Log("usable end");
            animator.SetInteger("DialogState", 0);
        }

      


        // Update is called once per frame
        void Update()
        {
            //Debug.Log("conversation update");
        }
    }
}