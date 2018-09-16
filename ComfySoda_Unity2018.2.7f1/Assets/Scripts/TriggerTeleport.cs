using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cs
{
    public class TriggerTeleport : MonoBehaviour
    {
        public string targetScene = "Default";

        public Vector2 targetPosition;
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                TeleportPlayer();
            }
        }

        private void OnEnable()
        {
            if(GetComponent<ProximityTrigger>() != null)
            {
                enabled = false;
            }
        }

        public void TeleportPlayer()
        {
            if (SceneManager.GetActiveScene().name == targetScene)
            {
                PlayerLogic.playerLogic.transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
            }
            else
            {
                PlayerLogic.playerLogic.targetPosition = targetPosition;
            }
        }
    }
}