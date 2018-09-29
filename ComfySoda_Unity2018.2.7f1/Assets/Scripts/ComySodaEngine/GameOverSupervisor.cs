using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace cs
{
    public class GameOverSupervisor : MonoBehaviour
    {
        [NonSerialized]
        public static GameOverSupervisor gameOverSupervisor = null;
        [NonSerialized]
        public static bool gameIsOver = false;


        [SerializeField]
        Image GameOverScreen = null;
        [SerializeField]
        float fadeSpeed = 0.2f;
        

        private void Awake()
        {
            Debug.Assert(gameOverSupervisor==null, "There should only be one \"GameOverController\" per scene!");
            gameOverSupervisor = this;
        }

        private void OnDestroy()
        {
            Debug.Assert(gameOverSupervisor == this, "There should only be one \"GameOverController\" per scene!");
            gameOverSupervisor = null;
        }
        
        public static void StartGameOver()
        {
            gameOverSupervisor.StartCoroutine(gameOverSupervisor.GameOverDisplay());
            gameIsOver = true;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            SceneSuperviser.GameOverCheck(collision);
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
