using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

namespace cs
{
    public class ProximityTrigger : MonoBehaviour
    {
        public delegate bool ProximityTriggerConditionCheck();
        public UnityEvent proximityActionFunction = new UnityEvent();
        public string triggerName = "";
        public ProximityTriggerConditionCheck proximityTriggerConditionCheck = null;
        
        [Serializable]
        private class DialogueParameters
        {
            public DialogInstructionSet dialogInstructionSet = null;
            public string instructionSetName = "";
        }

        //private but visible in editor
        [SerializeField]
        Text scenePromptText = null;
        [SerializeField]
        string prompt = "Press 'E' to interact";
        [SerializeField]
        DialogueParameters dialogueParameters = null;

        //private
        bool isColliding;
        
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            displayPrompt();
            isColliding = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            hidePrompt();
            isColliding = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && isColliding)
            {
                if(proximityTriggerConditionCheck==null || proximityTriggerConditionCheck())
                {
                    proximityActionFunction.Invoke();
                }
            }
        }

        private void hidePrompt()
        {
            if (scenePromptText == null)
                scenePromptText = MessageBoxTagger.proximityText;
            if (scenePromptText != null)
            {
                if (proximityTriggerConditionCheck == null || proximityTriggerConditionCheck())
                {
                    scenePromptText.text = "";
                }
            }
            else
            {
                Debug.LogError("No MessageBox Text Component has been tagged in this scene or it was cleared!");
            }
        }

        private void displayPrompt()
        {
            if(scenePromptText==null)
                scenePromptText = MessageBoxTagger.proximityText;
            if (scenePromptText != null)
            {
                if (proximityTriggerConditionCheck == null || proximityTriggerConditionCheck())
                {
                    scenePromptText.text = prompt;
                }
            }
            else
            {
                Debug.LogError("No MessageBox Text Component has been tagged in this scene or it was cleared!");
            }
        }

        public void StartConversation(string cutscene)
        {
            if(dialogueParameters!=null)
            {
                if(dialogueParameters.dialogInstructionSet == null)
                {
                    DialogInstructionSet[] dialogInstructionSets = gameObject.GetComponents<DialogInstructionSet>();
                    foreach(DialogInstructionSet dialogInstructionSet in dialogInstructionSets)
                    {
                        if(dialogInstructionSet.instructionSetName == dialogueParameters.instructionSetName)
                        {
                            dialogueParameters.dialogInstructionSet = dialogInstructionSet;
                        }
                    }
                }
                if(dialogueParameters.dialogInstructionSet!=null)
                {
                    DialogueSceneSupervisor.StartDialogue(cutscene, dialogueParameters.dialogInstructionSet);
                }
                else
                {
                    DialogueSceneSupervisor.StartDialogue(cutscene);
                }
            }
            else
            {
                DialogueSceneSupervisor.StartDialogue(cutscene);
            }
        }
        
        public void LoadScene(string SceneToLoad)
        {
            TriggerTeleport triggerTeleport = GetComponent<TriggerTeleport>();
            Debug.Assert(triggerTeleport != null, "Error: No TriggerTeleport component is attatched!");
            triggerTeleport.TeleportPlayer();
        }
    }

}
