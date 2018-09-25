using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEditor;
using UnityEngine;


namespace cs
{
    public class EnemyController : MonoBehaviour
    {

        #region Static referencing
        public static EnemyController enemyController = null;
        private void Awake()
        {
            Debug.Assert(enemyController == null, "Error: There should only be one enemy controller assigned to \"EnemyController.enemyController\" at a time");
            enemyController = this;
        }

        private void OnDestroy()
        {
            Debug.Assert(enemyController == this, "Error: The enemy controller assigned to \"EnemyController.enemyController\" does not equal: " + gameObject);
            enemyController = null;
        }
        #endregion


        #region Enemy distance
        [SerializeField]
        public float CLOSE = 7;
        [SerializeField]
        public float MID = 12;

        public enum EnemyRange { CLOSE, MID, FAR};
        public EnemyRange enemyRange = EnemyRange.FAR;
        public List<EnemyLogic> enemyList = EnemyLogic.enemyList;

        void calculateAndAssignEnemyRange()
        {
            foreach(EnemyLogic enemyLogic in enemyList)
            {
                enemyRange = EnemyRange.FAR;
                float tempDistance = Vector2.Distance(PlayerLogic.playerLogic.transform.position, enemyLogic.transform.position);
                {
                    if(tempDistance <= CLOSE || enemyRange==EnemyRange.CLOSE)
                    {
                        enemyRange = EnemyRange.CLOSE;
                    }
                    else if(tempDistance <= MID || enemyRange == EnemyRange.MID)
                    {
                        enemyRange = EnemyRange.MID;
                    }
                }
            }
        }
        #endregion
        private void Update()
        {
            calculateAndAssignEnemyRange();
            switch(enemyRange)
            {
                case EnemyRange.CLOSE:
                    if(GameOverController.gameIsOver!=true)
                    {
                        MusicManager.musicState = MusicState.boss;
                    }
                    else
                    {
                        MusicManager.musicState = MusicState.main;
                    }
                    break;
                case EnemyRange.MID:
                    if (GameOverController.gameIsOver != true)
                    {
                        MusicManager.musicState = MusicState.mob;
                    }
                    else
                    {
                        MusicManager.musicState = MusicState.main;
                    }
                    break;
                case EnemyRange.FAR:
                    MusicManager.musicState = MusicState.main;
                    break;
            }
        }
    }
}
