using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cs
{
    public class MessageBoxTagger : MonoBehaviour
    {
        public static Text proximityText = null;

        private void Awake()
        {
            if (proximityText != null)
            {
                Debug.LogWarning("There should only be once Text item tagged with the \"ProximityTextTagger\" per scene");
            }
            proximityText = GetComponent<Text>(); //gameObject;
        }

        private void OnDestroy()
        {

            if (proximityText == GetComponent<Text>())
            {
                proximityText = null;
            }
            else
            {
                Debug.LogWarning("There should only be once Text item tagged with the \"ProximityTextTagger\" per scene");
            }
        }
    }

}

