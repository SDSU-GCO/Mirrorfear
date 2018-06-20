using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cs
{
    public class GameOverController : MonoBehaviour
    {
        public Image GameOverScreen = null;
        public float fadeSpeed = 0.05f;
        public MusicManager musicManager = null;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == PersistentData.playerObject)
            {
                musicManager.musicState = MusicState.main;
                EnemyLogic.disableEnimies = true;
                PersistentData.playerObject.GetComponent<PlayerMovement>().disablePlayer = true;
                StartCoroutine(GameOverDisplay());
            }
        }

        IEnumerator GameOverDisplay()
        {
            while (GameOverScreen.color.a < 1)
            {
                GameOverScreen.color = new Color(GameOverScreen.color.r, GameOverScreen.color.g, GameOverScreen.color.b, Mathf.Min(1.0f, GameOverScreen.color.a + (fadeSpeed * Time.deltaTime)));
                yield return null;
            }
        }

    }
}
