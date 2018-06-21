using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    
    public class SceneLocalStaticsManager : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(DialogueManager.DialogueManagerOpen)
            {
                EnemyLogic.disableEnimies = true;
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
            if (PlayerLogic.playerObject != null && enemy.gameObject == PlayerLogic.playerObject)
            {
                MusicManager.musicState = MusicState.main;
                EnemyLogic.disableEnimies = true;
                PlayerLogic.disablePlayer = true;
                GameOverController.StartGameOver();
            }

        }

    }

}