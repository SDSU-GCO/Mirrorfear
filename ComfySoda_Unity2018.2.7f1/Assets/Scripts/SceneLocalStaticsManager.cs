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


            //music handler
            if (!EnemyLogic.disableEnimies && MusicManager.musicState != MusicState.off)
            {
                if (EnemyLogic.enemyLogicsInBossRange != null && EnemyLogic.enemyLogicsInBossRange.Count > 0)
                {
                    MusicManager.musicState = MusicState.boss;
                }
                else if (EnemyLogic.enemyLogicsInMobRange != null && EnemyLogic.enemyLogicsInMobRange.Count > 0)
                {
                    MusicManager.musicState = MusicState.mob;
                }
                else
                {
                    MusicManager.musicState = MusicState.main;
                }
            }
            else if (MusicManager.musicState != MusicState.off)
            {
                MusicManager.musicState = MusicState.main;
            }
        }

        public static void GameOverCheck(Collision2D enemy)
        {
            if (PlayerLogic.playerLogic != null && enemy.gameObject == PlayerLogic.playerLogic.gameObject)
            {
                MusicManager.musicState = MusicState.main;
                EnemyLogic.disableEnimies = true;
                PlayerLogic.playerLogic.enableMovement = false;
                GameOverController.StartGameOver();
            }

        }

    }

}