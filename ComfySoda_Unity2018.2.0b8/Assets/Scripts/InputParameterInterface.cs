using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class InputParameterInterface : MonoBehaviour {

        Animator animator = null;
        Rigidbody2D playerRigidbody2D = null;
        bool previouslyIdle = false;
        float verticalMemory=1;
        float horizontalMemory=0;
	
	    // Update is called once per frame
	    void Update ()
        {
            if (PlayerLogic.playerObject != null && animator == null)
            {
                animator = PlayerLogic.playerObject.GetComponent<Animator>();
            }
            if (PlayerLogic.playerObject != null && playerRigidbody2D == null)
            {
                playerRigidbody2D = PlayerLogic.playerObject.GetComponent<Rigidbody2D>();
            }

            if (animator!=null && playerRigidbody2D != null)
            {
                animator.SetFloat("Horizontal Axis", playerRigidbody2D.velocity.normalized.x);
                animator.SetFloat("Vertical Axis", playerRigidbody2D.velocity.normalized.y);

                if (Mathf.Abs(playerRigidbody2D.velocity.normalized.x) <= 0.1 && Mathf.Abs(playerRigidbody2D.velocity.normalized.y) <= 0.1)
                {
                    animator.SetBool("idle", true);
                    if(previouslyIdle!=true)
                    {
                        verticalMemory = playerRigidbody2D.velocity.normalized.y;
                        horizontalMemory = playerRigidbody2D.velocity.normalized.x;
                    }
                    else
                    {
                        animator.SetFloat("Horizontal Axis", horizontalMemory);
                        animator.SetFloat("Vertical Axis", verticalMemory);
                    }
                    previouslyIdle = true;
                }
                else
                {
                    animator.SetBool("idle", false);
                    previouslyIdle = false;
                }

            }
        }
    }
}


