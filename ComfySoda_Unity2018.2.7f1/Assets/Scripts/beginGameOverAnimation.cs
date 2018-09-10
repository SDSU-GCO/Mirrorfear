using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cs
{
    /// <summary>
    /// This script detects when the image(the game over GIF) is completely opaque, and then it triggers the animation to start playing
    /// The animator should have a boolean parameter "Play Game Over Animation" which is defaulted to false.  The animation state should be
    /// on a blank/static/or fade in animation and should transition wh
    /// </summary>
    public class beginGameOverAnimation : MonoBehaviour
    {
        Image gameOverScreen = null;
        Animator animator = null;

        // Use this for initialization
        private void Awake()
        {
            gameOverScreen = GetComponent<Image>();
            animator = GetComponent<Animator>();

            if (animator == null || gameOverScreen == null)
                Debug.LogError("The \"beginGameOverAnimation\" script should be atatched to the game over screen game object with an image and an animator!");
        }

        // Update is called once per frame
        void Update()
        {
            if(gameOverScreen.color.a>=1)
            {
                animator.SetBool("Play Game Over Animation", true);
            }
        }
    }

}
