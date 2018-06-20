using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace cs
{
    public delegate void ProximityActionFunction();
    public class ProximityTrigger : MonoBehaviour
    {
        public Text scenePromptText = null;
        public string SceneToLoad = "SampleScene";
        public bool useCoordinates = false;
        public string prompt = "Press 'E' to interact";
        bool isColliding;
        public ProximityActionFunction proximityActionFunction = null;

        private void OnEnable()
        {
            scenePromptText = PersistentData.proximityText;
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
                proximityActionFunction();
            }
        }

        private void hidePrompt()
        {
            if (scenePromptText != null)
                scenePromptText.text = "";
        }

        private void displayPrompt()
        {
            if (scenePromptText != null)
                scenePromptText.text = prompt;
        }
    }

}
