using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cs
{
    public class MessageBoxTagger : MonoBehaviour
    {
        public static Text proximityText = null;
        // Use this for initialization
        void Start()
        {
            proximityText = GetComponent<Text>(); //gameObject;
        }

        private void OnEnable()
        {
            if(proximityText!=null)
            {
                Debug.LogWarning("There should only be once Text item tagged with the \"ProximityTextTagger\" per scene");
            }
            proximityText = GetComponent<Text>(); //gameObject;
        }

        private void OnDisable()
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

