using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace cs
{
    public class SceneChanger : MonoBehaviour
    {
        public Text scenePromptText;
        public string SceneToLoad = "SampleScene";
        public bool useCoordinates = false;
        public float targetXCoordinate;
        public float targetYCoordinate;
        bool isColliding;

        private void OnEnable()
        {
            if(scenePromptText==null && PersistentData.proximityText!=null)
            {
                scenePromptText = PersistentData.proximityText;
            }
        }
        private void OnDisable()
        {

            scenePromptText = null;
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
                if (useCoordinates)
                {
                    PersistentData.xPlayerCoordinate = targetXCoordinate;
                    PersistentData.yPlayerCoordinate = targetYCoordinate;
                }
                else
                {
                    PersistentData.xPlayerCoordinate = null;
                    PersistentData.yPlayerCoordinate = null;
                }
                SceneManager.LoadScene(SceneToLoad);
            }
        }

        private void hidePrompt()
        {
            if (scenePromptText == null && PersistentData.proximityText != null)
            {
                scenePromptText = PersistentData.proximityText;
            }
            if(scenePromptText!=null)
                scenePromptText.text = "";
        }

        private void displayPrompt()
        {
            if (scenePromptText == null && PersistentData.proximityText != null)
            {
                scenePromptText = PersistentData.proximityText;
            }
            if (scenePromptText != null)
                scenePromptText.text = "Press 'E' to change floors.";
        }
    }
}
