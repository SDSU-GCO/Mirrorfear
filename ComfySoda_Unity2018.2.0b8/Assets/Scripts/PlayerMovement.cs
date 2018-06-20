using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    public class PlayerMovement : MonoBehaviour
    {
        public bool disablePlayer = false;
        public float speed = 1.0f;

	    // Use this for initialization
	    void Start ()
        {
		
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            if (!disablePlayer)
            {
                rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed;
            }
            else
            {
                rigidbody2D.velocity = new Vector2(0, 0);
            }
        }
    }

}

