using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class InputParameterInterface : MonoBehaviour {

        Animator animator = null;
        new Rigidbody2D rigidbody2D = null;
        bool previouslyIdle = false;
        float verticalMemory=-1;
        float horizontalMemory=0;
	
	    // Update is called once per frame
	    void Update ()
        {
            animator = GetComponent<Animator>();
            rigidbody2D = GetComponent<Rigidbody2D>();

            if (animator!=null && rigidbody2D != null)
            {
                animator.SetFloat("Horizontal Axis", rigidbody2D.velocity.normalized.x);
                animator.SetFloat("Vertical Axis", rigidbody2D.velocity.normalized.y);

                if (Mathf.Abs(rigidbody2D.velocity.normalized.x) <= 0.1 && Mathf.Abs(rigidbody2D.velocity.normalized.y) <= 0.1)
                {
                    animator.SetBool("idle", true);
                    if(previouslyIdle!=true)
                    {
                        verticalMemory = rigidbody2D.velocity.normalized.y;
                        horizontalMemory = rigidbody2D.velocity.normalized.x;
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


