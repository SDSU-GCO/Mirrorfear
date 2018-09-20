using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class WaypointBehavior : MonoBehaviour {
        public List<GameObject> waypointObjects = new List<GameObject>();
        public bool nodeVisited=false;
        public float? distance;
        public string WaypointName = "";
        
        public List<WaypointBehavior> GetWaypointBehaviors()
        {
            List<WaypointBehavior> temp = new List<WaypointBehavior>();
            foreach(GameObject go in waypointObjects)
            {
                temp.Add(go.GetComponent<WaypointBehavior>());
            }
            return temp;
        }

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




