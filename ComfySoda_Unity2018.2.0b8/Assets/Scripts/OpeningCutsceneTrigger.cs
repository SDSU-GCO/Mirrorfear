using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class OpeningCutsceneTrigger : MonoBehaviour
    {

        private void Update()
        {
            if (PersistentData.firstLoadSceneOne && DialogueManager.dialogueManager != null)
            {
                DialogueManager.dialogueManager.StartDialogue("Introduction");
                PersistentData.firstLoadSceneOne = false;
            }
        }
    }

}
