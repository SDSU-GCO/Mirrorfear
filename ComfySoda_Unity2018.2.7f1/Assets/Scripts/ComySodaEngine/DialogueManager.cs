using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace cs
{
    public class DialogueSceneSupervisor : MonoBehaviour
    {
        Dictionary<string, CharacterSpriteScriptableObject> characterSpriteData = new Dictionary<string, CharacterSpriteScriptableObject>();
        public CharacterSpriteScriptableObject[] characterSpriteScriptableObjects;

        public static bool DialogueManagerOpen = false;
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

        static DialogueSceneSupervisor dialogueManager = null;
        Dictionary<string, DialogInstructionSet.ActionFunction> actionFunctions = new Dictionary<string, DialogInstructionSet.ActionFunction>();
        Coroutine scrollingTextCoroutune = null;
        List<Parser.SentenceStructure> Conversation;
        int sentenceCounter = 0;
        int characterPerSecondDefaultForScene;



        private void Reset()
        {
            //doesn't work
            characterSpriteScriptableObjects = ComfySodaFunctions.GetAllInstances<CharacterSpriteScriptableObject>();
            animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            foreach(CharacterSpriteScriptableObject characterSpriteScriptableObject in characterSpriteScriptableObjects)
            {
                characterSpriteData.Add(characterSpriteScriptableObject.characterId, characterSpriteScriptableObject);
            }

            characterPerSecondDefaultForScene = characterPerSecond;
            DialogueManagerOpen = false;
            if (dialogueManager != null)
            {
                Debug.LogWarning("There should only be one \"DialogManager\" in a scene!");
            }
            dialogueManager = this;
        }

        private void OnDestroy()
        {
            DialogueManagerOpen = false;
            if (dialogueManager == this)
            {
                dialogueManager = null;
            }
            else
            {
                Debug.LogWarning("There should only be one \"DialogManager\" in a scene!");
            }
        }

        public static void StartDialogue(string theScene, DialogInstructionSet dialogInstructionSet)
        {
            if (dialogueManager != null)
            {
                dialogueManager.PrivateStartDialogue(theScene, dialogInstructionSet);
            }
            else
            {
                Debug.LogError("There is no \"DialogManager\" set on a dialog box!\nDid you add one to the scene?");
            }
        }

        public static void StartDialogue(string theScene)
        {
            if(dialogueManager!=null)
            {
                dialogueManager.PrivateStartDialogue(theScene);
            }
            else
            {
                Debug.LogError("There is no \"DialogManager\" set on a dialog box!\nDid you add one to the scene?");
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


            DialogueManagerOpen = true;
            animator.SetBool("IsOpen", true);

            Conversation = Dialoguehandler.getSceneData(cutscene);

            sentenceCounter = 0;
            
            DisplayNextSentence();
        }

        public void EndDialogue()
        {
            animator.SetBool("IsOpen", false);
            DialogueManagerOpen = false;
            EnemyLogic.disableEnimies = false;
            PlayerLogic.playerLogic.enableMovement = true;
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

            CharacterSpriteScriptableObject tempCharacter = null;
            if(characterSpriteData.TryGetValue(Conversation[sentenceCounter].speaker, out tempCharacter))
            {
                nameText.text = tempCharacter.characterName;
                currentPortrait.overrideSprite = tempCharacter.characterPortrait;
            }

            scrollingTextCoroutune = StartCoroutine(SaySentence(Conversation[sentenceCounter]));
            sentenceCounter++;

        }

        IEnumerator SaySentence(Parser.SentenceStructure sentence)
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
                                    Debug.LogError("ActionFunction in DialogueManager not found in instructionset: " + sentence.actionCharachterlist[0].action);
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
