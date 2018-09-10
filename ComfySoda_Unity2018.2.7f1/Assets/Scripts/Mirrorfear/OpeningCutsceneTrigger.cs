using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class OpeningCutsceneTrigger : MonoBehaviour
    {
        [SerializeField]
        string cutScene = "Introduction";
        private void Awake()
        {
            if (PersistentData.firstLoadSceneOne)
            {
                DialogueManager.StartDialogue(cutScene);
                PersistentData.firstLoadSceneOne = false;
            }
        }
    }

}
