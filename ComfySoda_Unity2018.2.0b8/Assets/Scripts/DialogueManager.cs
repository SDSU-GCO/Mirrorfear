using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace cs
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager dialogueManager = null;
        public TextMeshProUGUI nameText = null;
        public TextMeshProUGUI dialogueText = null;
        public Image portrait = null;
        public Sprite timmy = null;
        public Sprite mirrirChild = null;
        public Sprite Blank = null;
        private Coroutine scrollingTextCoroutune = null;

        public Animator animator = null;
        public int characterPerSecond = 10;

        List<Parser.DialogueStructure> Conversation;

        private int sentenceCounter = 0;

        private void OnEnable()
        {
            dialogueManager = this;
        }
        private void OnDisable()
        {
            if(dialogueManager==this)
            {
                dialogueManager = null;
            }
        }

        public void StartDialogue(string cutscene)
        {
            animator.SetBool("IsOpen", true);

            Conversation = Dialoguehandler.getSceneData(cutscene);

            sentenceCounter = 0;


            DisplayNextSentence();
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
           

            switch (Conversation[sentenceCounter].speaker)
            {
                case Parser.Speaker.PLAYER_NAME:
                    nameText.text = "Timmy";
                    portrait.overrideSprite = timmy;
                    break;
                case Parser.Speaker.ENEMY_NAME:
                    nameText.text = "Mirror Timmy";
                    portrait.overrideSprite = mirrirChild;
                    break;
                case Parser.Speaker.BLANK:
                    nameText.text = " ";
                    portrait.overrideSprite = Blank;
                    break;
                case Parser.Speaker.ERROR:
                    break;
                default:
                    Debug.Log("invalid speaker for conversation");
                    break;
            }

            scrollingTextCoroutune = StartCoroutine(TypeSentence(Conversation[sentenceCounter]));
            sentenceCounter++;

        }

        IEnumerator TypeSentence(Parser.DialogueStructure sentence)
        {
            int charachtersToPrint = 0;
            float intervalInSeconds = 0;
            float currentTime = Time.time;
            float? lastFrame = null;
            float Overflow = 0;
            dialogueText.text = " ";
            while (sentence.dialogue.Count>0)
            {
                frameStart(ref currentTime, ref intervalInSeconds, ref charachtersToPrint, ref Overflow, lastFrame);
                if (lastFrame != null)
                {
                    for (int i = 0; i < charachtersToPrint; i++)
                    {
                        if (sentence.dialogue[0].character != null)
                        {
                            dialogueText.text += sentence.dialogue[0].character;
                            sentence.dialogue.Remove(sentence.dialogue[0]);
                        }
                        else if (sentence.dialogue[0].action != null)
                        {
                            switch (sentence.dialogue[0].action)
                            {
                                case Parser.Action.RING_BELL:
                                    break;
                                default:
                                    Debug.Log("Error in dialog!");
                                    break;
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




        public void EndDialogue()
        {
            animator.SetBool("IsOpen", false);
        }
    }

}
