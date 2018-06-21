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
        
        public UnityEvent proximityActionFunction;
        public Text scenePromptText = null;
        public string SceneToLoad = "SampleScene";
        public bool useCoordinates = false;
        public string prompt = "Press 'E' to interact";
        bool isColliding;
        void Awake()
        {
            if (proximityActionFunction == null)
                proximityActionFunction = new UnityEvent();
        }

        private void OnEnable()
        {
            scenePromptText = MessageBoxTagger.proximityText;
        }
        

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
                proximityActionFunction.Invoke();
            }
        }

        private void hidePrompt()
        {
            if (scenePromptText != null)
            {
                scenePromptText.text = "";
            }
            else
            {
                Debug.LogError("No MessageBox Text Component has been tagged in this scene or it was cleared!");
            }
        }

        private void displayPrompt()
        {
            if (scenePromptText != null)
            {
                scenePromptText.text = prompt;
            }
            else
            {
                Debug.LogError("No MessageBox Text Component has been tagged in this scene or it was cleared!");
            }
        }
    }

}
