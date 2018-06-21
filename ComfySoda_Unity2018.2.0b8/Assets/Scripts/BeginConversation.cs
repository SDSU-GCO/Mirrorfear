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
            if (proximityTrigger!=null)
            {
                proximityTrigger.proximityActionFunction.AddListener(StartConversation);
            }
            else
            {
                Debug.LogError("There must be a \"ProximityTrigger\" on an object to use the \"BeginConversation\" script!");
            }
        }

        private void OnDisable()
        {
            if(proximityTrigger==null)
            {
                Debug.LogError("There must be a \"ProximityTrigger\" on an object to use the \"BeginConversation\" script!");
            }
            else
            {
                proximityTrigger.proximityActionFunction.RemoveListener(StartConversation);
            }
            
        }

        void StartConversation()
        {
           DialogueManager.StartDialogue(sceneToLoad);
        }
    }

}

