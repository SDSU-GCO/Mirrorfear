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
        public class SceneParameters
        {
            public bool useTarget;
            public Vector2 target;
            public SceneParameters()
            {
                useTarget = false;
                target.x = 0;
                target.y = 0;
            }
        }

        //private but visible in editor
        [SerializeField]
        Text scenePromptText = null;
        [SerializeField]
        string prompt = "Press 'E' to interact";
        [SerializeField]
        SceneParameters sceneParameters;

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
            DialogueManager.StartDialogue(cutscene);
        }
        
        public void LoadScene(string SceneToLoad)
        {
            if (sceneParameters.useTarget)
            {
                PersistentData.xPlayerCoordinate = sceneParameters.target.x;
                PersistentData.yPlayerCoordinate = sceneParameters.target.y;
            }
            else
            {
                PersistentData.xPlayerCoordinate = null;
                PersistentData.yPlayerCoordinate = null;
            }

            SceneManager.LoadScene(SceneToLoad);
        }
    }

}
