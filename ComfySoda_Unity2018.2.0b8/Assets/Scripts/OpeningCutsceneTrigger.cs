using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class OpeningCutsceneTrigger : MonoBehaviour
    {

        private void Awake()
        {
            if (PersistentData.firstLoadSceneOne)
            {
                DialogueManager.StartDialogue("Introduction");
                PersistentData.firstLoadSceneOne = false;
            }
        }
    }

}
