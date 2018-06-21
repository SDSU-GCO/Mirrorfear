using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    public class PlayerLogic : MonoBehaviour
    {
        public static GameObject playerObject = null;
        public static bool disablePlayer = false;
        public float speed = 1.0f;

	    // Use this for initialization
	    void Start ()
        {
            if (playerObject != null)
            {
                playerObject = gameObject;
            }
            else
            {
                Debug.LogWarning("There is more than one player in the scene, is this intentional?");
            }
        }


        private void OnDisable()
        {
            if (playerObject == gameObject)
            {
                playerObject = null;
            }
            else
            {
                Debug.LogWarning("There is more than one player in the scene, is this intentional?");
            }
        }

        private void OnEnable()
        {
            if (playerObject != gameObject)
            {
                playerObject = gameObject;
            }
            else
            {
                Debug.LogWarning("There is more than one player in the scene, is this intentional?");
            }
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

