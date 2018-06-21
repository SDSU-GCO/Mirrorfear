using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class SceneLoadScript : MonoBehaviour
    {

        GameObject playerObject = null;

        // Use this for initialization
        void Start()
        {
            if (PersistentData.xPlayerCoordinate != null && PersistentData.yPlayerCoordinate != null)
            {
                if (playerObject == null)
                {
                    playerObject = GameObject.Find("Player");
                }
                if (playerObject != null)
                {
                    playerObject.transform.SetPositionAndRotation(new Vector3((float)PersistentData.xPlayerCoordinate, (float)PersistentData.yPlayerCoordinate, playerObject.transform.position.z), new Quaternion(playerObject.transform.rotation.x, playerObject.transform.rotation.y, playerObject.transform.rotation.z, playerObject.transform.rotation.w));
                }
            }

            PersistentData.xPlayerCoordinate = null;
            PersistentData.yPlayerCoordinate = null;
        }
    }

}

