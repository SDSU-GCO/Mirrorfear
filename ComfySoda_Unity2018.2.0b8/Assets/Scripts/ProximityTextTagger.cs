using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cs
{
    public class ProximityTextTagger : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            PersistentData.proximityText = GetComponent<Text>(); //gameObject;
        }

        private void OnEnable()
        {
            PersistentData.proximityText = GetComponent<Text>(); //gameObject;
        }

        private void OnDisable()
        {
            if (PersistentData.proximityText == GetComponent<Text>())
            {
                PersistentData.proximityText = null;
            }
        }
    }

}

