using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cs
{
    public class GameOverController : MonoBehaviour
    {
        public static bool gameIsOver = false;
        [SerializeField]
        Image GameOverScreen = null;
        [SerializeField]
        float fadeSpeed = 0.05f;

        delegate void Function();
        static Function function;

        private void Awake()
        {
            if (function != null)
            {
                Debug.LogWarning("There should only be one \"GameOverController\" per scene!");
            }
            function = StartDisplayingGameOver;
        }

        private void OnDestroy()
        {
            if (function != StartDisplayingGameOver)
            {
                Debug.LogWarning("There should only be one \"GameOverController\" per scene!");
            }
            else
            {
                function = null;
            }
        }
        
        public static void StartGameOver()
        {
            function();
            gameIsOver = true;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            SceneLocalStaticsManager.GameOverCheck(collision);
        }

        void StartDisplayingGameOver()
        {
            StartCoroutine(GameOverDisplay());
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
