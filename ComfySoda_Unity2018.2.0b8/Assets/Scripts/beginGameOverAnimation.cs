using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cs
{
    public class beginGameOverAnimation : MonoBehaviour
    {
        Image gameOverScreen = null;
        Animator animator = null;

        // Use this for initialization
        void Start()
        {
            gameOverScreen = GetComponent<Image>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(gameOverScreen!=null)
            {
                if(gameOverScreen.color.a>=1)
                {
                    if(animator!=null)
                    {
                        animator.SetBool("Play Game Over Animation", true);
                    }
                    else
                    {
                        animator = GetComponent<Animator>();
                    }
                }
            }
            else
            {
                gameOverScreen = GetComponent<Image>();
            }
        }
    }

}
