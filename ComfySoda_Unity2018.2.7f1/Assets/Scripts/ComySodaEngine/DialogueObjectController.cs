using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace cs
{
    public class DialogueObjectController : MonoBehaviour
    {
        public static bool DialogueBoxOpen = false;
        public int? characterPerSecondOverride = null;
        [SerializeField]
        int characterPerSecond = 10;
        [SerializeField]
        Animator animator = null;
        [SerializeField]
        TextMeshProUGUI nameText = null;
        [SerializeField]
        TextMeshProUGUI dialogueText = null;
        [SerializeField]
        Image currentPortrait = null;

        static DialogueObjectController dialogueObjectController = null;
        Dictionary<string, DialogInstructionSet.ActionFunction> actionFunctions = new Dictionary<string, DialogInstructionSet.ActionFunction>();
        Coroutine scrollingTextCoroutune = null;
        List<DialogueSupervisor.Parser.SentenceStructure> Conversation;
        int sentenceCounter = 0;
        int characterPerSecondDefaultForScene;


        private void Awake()
        {
            characterPerSecondDefaultForScene = characterPerSecond;
            DialogueBoxOpen = false;
            if (dialogueObjectController != null)
            {
                Debug.LogWarning("There should only be one \"DialogueObjectController\" in a scene!");
            }
            dialogueObjectController = this;
            if(animator==null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void OnDestroy()
        {
            DialogueBoxOpen = false;
            if (dialogueObjectController == this)
            {
                dialogueObjectController = null;
            }
            else
            {
                Debug.LogWarning("There should only be one \"DialogueObjectController\" in a scene!");
            }
        }

        public static void StartDialogue(string theScene, DialogInstructionSet dialogInstructionSet)
        {
            if (dialogueObjectController != null)
            {
                dialogueObjectController.PrivateStartDialogue(theScene, dialogInstructionSet);
            }
            else
            {
                Debug.LogError("There is no \"DialogueObjectController\" set on a dialog box!\nDid you add one to the scene?");
            }
        }

        public static void StartDialogue(string theScene)
        {
            if(dialogueObjectController!=null)
            {
                dialogueObjectController.PrivateStartDialogue(theScene);
            }
            else
            {
                Debug.LogError("There is no \"DialogueObjectController\" set on a dialog box!\nDid you add one to the scene?");
            }
        }

        void PrivateStartDialogue(string cutscene, DialogInstructionSet dialogInstructionSet)
        {

            if (dialogInstructionSet != null)
            {
                foreach (KeyValuePair<string, DialogInstructionSet.ActionFunction> actionFunction in dialogInstructionSet.actionFunctions)
                {
                    actionFunctions.Add(actionFunction.Key, actionFunction.Value);
                }
            }
            else
            {
                actionFunctions.Clear();
            }
            PrivateStartDialogue(cutscene);
        }

        void PrivateStartDialogue(string cutscene)
        {
            if(characterPerSecondOverride!=null)
            {
                characterPerSecond = (int)characterPerSecondOverride;
            }
            else
            {
                characterPerSecond = characterPerSecondDefaultForScene;
            }


            DialogueBoxOpen = true;
            animator.SetBool("IsOpen", true);

            Conversation = DialogueSupervisor.getSceneData(cutscene);

            sentenceCounter = 0;
            
            DisplayNextSentence();
        }

        public void EndDialogue()
        {
            animator.SetBool("IsOpen", false);
            DialogueBoxOpen = false;
            EnemyObjectController.disableEnimies = false;
            PlayerObjectController.playerObjectController.enableMovement = true;
            actionFunctions.Clear();
        }

        public void DisplayNextSentence()
        {

            if (sentenceCounter >= Conversation.Count)
            {
                EndDialogue();
                return;
            }
            if(scrollingTextCoroutune!=null)
            {
                StopCoroutine(scrollingTextCoroutune);
            }

            bool testKeypair = StoryCharachterGameSupervisor.storyCharacters.ContainsKey(Conversation[sentenceCounter].speaker);
            Debug.Assert(testKeypair == true, "Error: Invalid speaker for conversation: " + Conversation[sentenceCounter].speaker);
            if (testKeypair)
            {
                nameText.text = StoryCharachterGameSupervisor.storyCharacters[Conversation[sentenceCounter].speaker].name;
                currentPortrait.overrideSprite = StoryCharachterGameSupervisor.storyCharacters[Conversation[sentenceCounter].speaker].sprite;
            }

            scrollingTextCoroutune = StartCoroutine(TypeSentence(Conversation[sentenceCounter]));
            sentenceCounter++;

        }

        IEnumerator TypeSentence(DialogueSupervisor.Parser.SentenceStructure sentence)
        {
            int charachtersToPrint = 0;
            float intervalInSeconds = 0;
            float currentTime = Time.time;
            float? lastFrame = null;
            float Overflow = 0;
            dialogueText.text = " ";
            while (sentence.actionCharachterlist.Count>0)
            {
                frameStart(ref currentTime, ref intervalInSeconds, ref charachtersToPrint, ref Overflow, lastFrame);
                if (lastFrame != null)
                {
                    for (int i = 0; i < charachtersToPrint && sentence.actionCharachterlist.Count > 0; i++)
                    {
                        if (sentence.actionCharachterlist[0].character != null)
                        {
                            dialogueText.text += sentence.actionCharachterlist[0].character;
                            sentence.actionCharachterlist.Remove(sentence.actionCharachterlist[0]);
                        }
                        else if (sentence.actionCharachterlist[0].action != "")
                        {
                            if(sentence.actionCharachterlist[0].action.Contains("DYNAMIC_ACTION: "))
                            {
                                sentence.actionCharachterlist[0].action.Remove(0, "DYNAMIC_ACTION: ".Length);

                                if (actionFunctions.ContainsKey(sentence.actionCharachterlist[0].action))
                                {
                                    //trigger the action function
                                    actionFunctions[sentence.actionCharachterlist[0].action].Invoke();
                                }
                                else
                                {
                                    if(actionFunctions.Count==0)
                                    {
                                        Debug.LogError("No Instruction set found!  Please load an instructionset before invoking instructions!");
                                    }
                                    Debug.LogError("ActionFunction in DialogueObjectController not found in instructionset: " + sentence.actionCharachterlist[0].action);
                                }

                            }
                        }

                    }
                }
                frameEnd(ref lastFrame, currentTime);
                yield return null;
            }


        }

        private void frameStart(ref float currentTime, ref float intervalInSeconds, ref int charachtersToPrint, ref float Overflow, float? lastFrame)
        {
            if (lastFrame != null)
            {
                currentTime = Time.time;
                intervalInSeconds = currentTime - ((float)lastFrame);
                charachtersToPrint = (int)((intervalInSeconds * characterPerSecond) + Overflow);
                Overflow += (intervalInSeconds * characterPerSecond) - charachtersToPrint;
            }
        }

        private void frameEnd(ref float? lastFrame, float currentTime)
        {
            lastFrame = currentTime;
        }
    }
    
}
