using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class PlayerObjectTracker : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        private void OnDisable()
        {
            if (PersistentData.playerObject == gameObject)
            {
                PersistentData.playerObject = null;
            }
        }

        private void OnEnable()
        {
            PersistentData.playerObject = gameObject;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


