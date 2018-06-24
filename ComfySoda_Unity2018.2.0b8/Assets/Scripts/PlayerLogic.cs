using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    public class PlayerLogic : MonoBehaviour
    {
        public static GameObject playerObject = null;
        public static bool disablePlayer = false;
        public static bool disablePlayerPreviousFrame = false;
        public float speed = 1.0f;

	    // Use this for initialization
	    private void Awake ()
        {
            if (playerObject == null)
            {
                playerObject = gameObject;
            }
            else
            {
                Debug.LogWarning("There is more than one player in the scene, is this intentional?");
            }


            if (PersistentData.xPlayerCoordinate != null && PersistentData.yPlayerCoordinate != null)
            {
                transform.SetPositionAndRotation(new Vector3((float)PersistentData.xPlayerCoordinate, (float)PersistentData.yPlayerCoordinate, transform.position.z), new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w));
            }

            PersistentData.xPlayerCoordinate = null;
            PersistentData.yPlayerCoordinate = null;
        }

        private void OnDestroy()
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

        // Update is called once per frame
        void Update ()
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            if (!disablePlayer)
            {
                rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed;
            }
            else if(disablePlayerPreviousFrame==true)
            {
                rigidbody2D.velocity = new Vector2(0, 0);
            }
            disablePlayerPreviousFrame = disablePlayer;
        }
    }

}

