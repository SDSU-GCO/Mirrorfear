using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class WaypointBehavior : MonoBehaviour {
        public List<GameObject> waypointObjects = new List<GameObject>();
        public bool nodeVisited=false;
        public float distance;
        public bool infiniteDistance;
        public string WaypointName = "";
        
        private void Awake()
        {
            GetComponentInParent<WayPointSystem>().Add(this);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnDestroy()
        {
            GetComponentInParent<WayPointSystem>().Remove(this);
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

}




