using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    public class WaypointObjectController : MonoBehaviour {
        public List<GameObject> waypointObjects = new List<GameObject>();
        public bool nodeVisited=false;
        public float? distance;
        WaypointSupervisor waypointSupervisor = null;


        public List<WaypointObjectController> GetWaypointBehaviors()
        {
            List<WaypointObjectController> temp = new List<WaypointObjectController>();
            foreach(GameObject go in waypointObjects)
            {
                temp.Add(go.GetComponent<WaypointObjectController>());
            }
            return temp;
        }

        private void OnEnable()
        {
            waypointSupervisor = GetComponentInParent<WaypointSupervisor>();
            waypointSupervisor.Add(this);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnDisable()
        {
            waypointSupervisor.Remove(this);
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}




