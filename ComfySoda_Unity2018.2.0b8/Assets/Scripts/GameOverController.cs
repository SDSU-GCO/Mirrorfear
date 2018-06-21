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

        delegate void Function();
        static Function function;

        void OnEnable()
        {
            if(function != null)
            {
                Debug.LogWarning("There should only be one \"GameOverController\" per scene!");
            }
            function = StartDisplayingGameOver;
        }

        void OnDisable()
        {
            if(function != StartDisplayingGameOver)
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
