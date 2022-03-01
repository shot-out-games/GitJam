using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Dialogue
{
    public class QuestVariableCheck : MonoBehaviour
    {
        /// <summary>
        /// The variable to increment.
        /// </summary>
        [Tooltip("Increment this Dialogue System variable.")]
        [VariablePopup]
        public string variable = string.Empty;

        /// <summary>
        /// The increment amount. To decrement, use a negative number.
        /// </summary>
        [Tooltip("Increment the variable by this amount. Use a negative value to decrement.")]
        public int increment = 1;

        /// <summary>
        /// The minimum value.
        /// </summary>
        [Tooltip("After incrementing, ensure that the variable is at least this value.")]
        public int min = 0;

        /// <summary>
        /// The maximum value.
        /// </summary>
        [Tooltip("After incrementing, ensure that the variable is no more than this value.")]
        public int max = 100;

        [Tooltip("Optional alert message to show when incrementing.")]
        public string alertMessage = string.Empty;

        [Tooltip("Duration to show alert, or 0 to use default duration.")]
        public float alertDuration = 0;

        //  [Tooltip("If set, only increment if the conditions are true.")]
        //  public Condition condition = new Condition();
        int previousCount;
        //int currentCount;

        public UnityEvent onQuestVariableUpdate = new UnityEvent();
        //Animator animator;
        // Use this for initialization
        void Start()
        {

            //animator = GetComponent<Animator>();
            //DialogueManager.ShowAlert("HEY YOU");
            //      protected virtual void IncrementNow()
            //{
            
            //}
        }

        protected virtual string actualVariableName
        {
            get { return string.IsNullOrEmpty(variable) ? DialogueActor.GetPersistentDataName(transform) : variable; }
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("conversation update");
            int currentLevel = LevelManager.instance.currentLevelCompleted;

            //int oldValue = DialogueLua.GetVariable(actualVariableName).asInt;
            //int newValue = Mathf.Clamp(oldValue + increment, min, max);
            var dead = LevelManager.instance.levelSettings[currentLevel].enemiesDead;

            if (dead == previousCount) return;
            previousCount = 1;

            DialogueLua.SetVariable(actualVariableName, dead);
            Debug.Log("enemies dead" + dead);
            DialogueManager.SendUpdateTracker();
            if (!(string.IsNullOrEmpty(alertMessage) || DialogueManager.instance == null))
            {
                if (Mathf.Approximately(0, alertDuration))
                {
                    DialogueManager.ShowAlert(alertMessage);
                }
                else
                {
                    DialogueManager.ShowAlert(alertMessage, alertDuration);
                }
            }
            onQuestVariableUpdate.Invoke();
        }
    }
}