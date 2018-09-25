using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class Waypoint : MonoBehaviour {
        public List<GameObject> waypointObjects = new List<GameObject>();
        public bool nodeVisited=false;
        public float? distance;
        public string WaypointName = "";
        WayPointSystem wayPointSystem = null;


        public List<Waypoint> GetWaypointBehaviors()
        {
            List<Waypoint> temp = new List<Waypoint>();
            foreach(GameObject go in waypointObjects)
            {
                temp.Add(go.GetComponent<Waypoint>());
            }
            return temp;
        }

        private void OnEnable()
        {
            wayPointSystem = GetComponentInParent<WayPointSystem>();
            wayPointSystem.Add(this);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnDisable()
        {
            wayPointSystem.Remove(this);
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}




