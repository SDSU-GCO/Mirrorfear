using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    
    public class SceneLocalStaticsManager : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if(DialogueManager.DialogueManagerOpen)
            {
                EnemyLogic.disableEnimies = true;
                PlayerLogic.playerLogic.enableMovement = false;
            }
            
        }

        public static void GameOverCheck(Collision2D enemy)
        {
            if (PlayerLogic.playerLogic != null && enemy.gameObject == PlayerLogic.playerLogic.gameObject)
            {
                EnemyLogic.disableEnimies = true;
                PlayerLogic.playerLogic.enableMovement = false;
                GameOverController.StartGameOver();
            }

        }

    }

}