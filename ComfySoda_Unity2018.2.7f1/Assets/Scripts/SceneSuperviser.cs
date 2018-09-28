using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    
    public class SceneSuperviser : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if(DialogueObjectController.DialogueBoxOpen)
            {
                EnemyObjectController.disableEnimies = true;
                PlayerObjectController.playerObjectController.enableMovement = false;
            }
        }

        public static void GameOverCheck(Collision2D enemy)
        {
            if (PlayerObjectController.playerObjectController != null && enemy.gameObject == PlayerObjectController.playerObjectController.gameObject)
            {
                EnemyObjectController.disableEnimies = true;
                PlayerObjectController.playerObjectController.enableMovement = false;
                GameOverSupervisor.StartGameOver();
            }

        }

    }

}