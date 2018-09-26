using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    public class DialogInstructionSet : MonoBehaviour
    {

        public delegate void ActionFunction();
        public Dictionary<string, ActionFunction> actionFunctions = new Dictionary<string, ActionFunction>();
        public delegate bool DialogueConditionMet();
        public DialogueConditionMet dialogueConditionMet;
        public string instructionSetName = "default";

        private void Awake()
        {
            dialogueConditionMet = null;
            //actionFunctions.Add(string key, ActionFunction value);
        }
        
    }

}
