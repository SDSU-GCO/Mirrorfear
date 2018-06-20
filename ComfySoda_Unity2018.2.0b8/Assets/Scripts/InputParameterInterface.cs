using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class InputParameterInterface : MonoBehaviour {

        Animator animator = null;
        bool previouslyIdle = false;
        float verticalMemory=1;
        float horizontalMemory=0;

	    // Use this for initialization
	    void Start () {
		    if(PersistentData.playerObject!=null)
            {
                animator = PersistentData.playerObject.GetComponent<Animator>();
            }
	    }
	
	    // Update is called once per frame
	    void Update () {
            if(animator!=null)
            {
                animator.SetFloat("Horizontal Axis", Input.GetAxis("Horizontal"));
                animator.SetFloat("Vertical Axis", Input.GetAxis("Vertical"));

                if (Mathf.Abs(Input.GetAxis("Horizontal")) <= 0.1 && Mathf.Abs(Input.GetAxis("Vertical")) <= 0.1)
                {
                    animator.SetBool("idle", true);
                    if(previouslyIdle!=true)
                    {
                        verticalMemory = Input.GetAxis("Vertical");
                        horizontalMemory = Input.GetAxis("Horizontal");
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


