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
    public class EnemySupervisor : MonoBehaviour
    {

        #region Static referencing
        public static EnemySupervisor enemySupervisor = null;
        private void Awake()
        {
            Debug.Assert(enemySupervisor == null, "Error: There should only be one enemy controller assigned to \"EnemyController.enemyController\" at a time");
            enemySupervisor = this;
        }

        private void OnDestroy()
        {
            Debug.Assert(enemySupervisor == this, "Error: The enemy controller assigned to \"EnemyController.enemyController\" does not equal: " + gameObject);
            enemySupervisor = null;
        }
        #endregion


        #region Enemy distance
        public enum EnemyRange { CLOSE, MID, FAR };


        public float CLOSE = 3;
        public float MID = 8;
        public EnemyRange enemyRange = EnemyRange.FAR;

        [NonSerialized]
        public List<EnemyObjectController> enemyList = EnemyObjectController.enemyList;

        void calculateAndAssignEnemyRange()
        {
            enemyRange = EnemyRange.FAR;
            foreach (EnemyObjectController enemyLogic in enemyList)
            {
                float tempDistance = Vector2.Distance(PlayerObjectController.playerObjectController.transform.position, enemyLogic.transform.position);
                if(tempDistance <= CLOSE)
                {
                    enemyRange = EnemyRange.CLOSE;
                }
                else if(tempDistance <= MID && enemyRange == EnemyRange.FAR)
                {
                    enemyRange = EnemyRange.MID;
                }
            }
        }
        #endregion

        private void Update()
        {
            calculateAndAssignEnemyRange();

            if(GameOverSupervisor.gameIsOver == true)
            {
                MusicManager.musicState = MusicState.main;
            }
            else
            {
                switch (enemyRange)
                {
                    case EnemyRange.CLOSE:
                        MusicManager.musicState = MusicState.boss;
                        break;
                    case EnemyRange.MID:
                        if (GameOverSupervisor.gameIsOver != true)
                            MusicManager.musicState = MusicState.mob;
                        break;
                    case EnemyRange.FAR:
                        MusicManager.musicState = MusicState.main;
                        break;
                }
            }
        }
    }
}
