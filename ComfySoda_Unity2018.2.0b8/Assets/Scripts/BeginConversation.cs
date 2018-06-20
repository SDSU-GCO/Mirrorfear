using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class BeginConversation : MonoBehaviour
    {
        ProximityTrigger proximityTrigger;
        public string sceneToLoad = "";


        private void OnEnable()
        {
            proximityTrigger = GetComponent<ProximityTrigger>();
            proximityTrigger.proximityActionFunction = StartConversation;
        }

        private void OnDisable()
        {
            if(proximityTrigger!=null && proximityTrigger.proximityActionFunction==StartConversation)
            {
                proximityTrigger.proximityActionFunction = null;
            }
            
        }

        void StartConversation()
        {
            DialogueManager.dialogueManager.StartDialogue(sceneToLoad);
        }
    }

}

