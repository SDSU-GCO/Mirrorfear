using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class WaypointBehavior : MonoBehaviour {
        public static List<WaypointBehavior> waypoints = new List<WaypointBehavior>();
        public List<GameObject> waypointObjects = new List<GameObject>();
        public bool nodeVisited=false;
        public float distance;
        public bool infiniteDistance;
        public string WaypointName = "";

        

        private void Awake()
        {
            waypoints.Add(this);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnDestroy()
        {
            waypoints.Remove(this);
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

}




