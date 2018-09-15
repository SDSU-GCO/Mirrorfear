using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{

    public class WayPointSystem : MonoBehaviour
    {
        List<WaypointBehavior> waypoints = WaypointBehavior.waypoints;
        
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public WaypointBehavior getClosestWaypointToObject(GameObject originGameObject)
        {
            WaypointBehavior closestNode = null;
            if (waypoints != null && waypoints.Count >= 1)
            {
                float? shortestDistanceToWaypoint = null;
                float? distance = null;

                foreach (WaypointBehavior waypoint in waypoints)
                {
                    distance = Vector3.Distance(originGameObject.transform.position, waypoint.transform.position);
                    if (shortestDistanceToWaypoint == null || shortestDistanceToWaypoint > distance)
                    {
                        shortestDistanceToWaypoint = distance;
                        closestNode = waypoint;
                    }
                }
            }
            return closestNode;
        }
        
        public WaypointBehavior getClosestEndpointsToObjects(GameObject originGameObject, GameObject targetGameObject)
        {

            WaypointBehavior closestNodeToOrigin = null;
            WaypointBehavior closestNodeToTarget = null;
            if (waypoints != null && waypoints.Count >= 1)
            {
                float? shortestDistanceToWaypointFromOrigin = null;
                float? shortestDistanceToWaypointFromTarget = null;
                float? distance = null;

                foreach (WaypointBehavior waypoint in waypoints)
                {

                    distance = Vector3.Distance(originGameObject.transform.position, waypoint.transform.position);
                    if (shortestDistanceToWaypointFromOrigin == null || shortestDistanceToWaypointFromOrigin > distance)
                    {
                        shortestDistanceToWaypointFromOrigin = distance;
                        closestNodeToOrigin = waypoint;
                    }

                    distance = Vector3.Distance(targetGameObject.transform.position, waypoint.transform.position);
                    if (shortestDistanceToWaypointFromTarget == null || shortestDistanceToWaypointFromTarget > distance)
                    {
                        shortestDistanceToWaypointFromTarget = distance;
                        closestNodeToTarget = waypoint;
                    }

                }
            }
            return closestNodeToOrigin;
        }

        public void findPath(ref bool pathFound, ref WaypointBehavior currentNode, ref List<WaypointBehavior> unvisitedSet, WaypointBehavior targetNode)
        {
            while (!pathFound && currentNode != null)
            {
                currentNode.nodeVisited = true;
                unvisitedSet.Remove(currentNode);
                if (targetNode.nodeVisited)
                    pathFound = true;
                else
                {
                    for (int i = 0; i < currentNode.waypointObjects.Count && !pathFound; i++)
                    {
                        GameObject waypoint = currentNode.waypointObjects[i];
                        WaypointBehavior tempWaypointBehavior = waypoint.GetComponent<WaypointBehavior>();
                        if (!tempWaypointBehavior.nodeVisited)
                        {
                            float tentativeDistance = Vector3.Distance(currentNode.transform.position, waypoint.transform.position);
                            if (tentativeDistance < tempWaypointBehavior.distance || tempWaypointBehavior.infiniteDistance)
                            {
                                tempWaypointBehavior.infiniteDistance = false;
                                tempWaypointBehavior.distance = currentNode.distance + tentativeDistance;
                            }
                        }
                    }

                    WaypointBehavior nextNode = null;
                    for (int j = 0; j < unvisitedSet.Count && !pathFound; j++)
                    {
                        if ((nextNode != null && unvisitedSet[j].distance < nextNode.distance && !unvisitedSet[j].infiniteDistance) || (nextNode == null && !unvisitedSet[j].infiniteDistance))
                        {
                            nextNode = unvisitedSet[j];
                        }
                    }
                    currentNode = nextNode;
                }
            }
        }

        public List<WaypointBehavior> tracePath(WaypointBehavior closestWaypoint, WaypointBehavior currentNode)
        {
            List<WaypointBehavior> pathOfWaypoints = new List<WaypointBehavior>();
            while (closestWaypoint != currentNode)
            {
                pathOfWaypoints.Add(currentNode);
                float? shortestDistance = null;
                WaypointBehavior nextNode = null;
                foreach (GameObject tempWayPointObject in currentNode.waypointObjects)
                {
                    WaypointBehavior tempWaypointBehavior = tempWayPointObject.GetComponent<WaypointBehavior>();
                    if ((shortestDistance == null || (shortestDistance > tempWaypointBehavior.distance)) && !tempWaypointBehavior.infiniteDistance)
                    {
                        shortestDistance = tempWaypointBehavior.distance;
                        nextNode = tempWaypointBehavior;
                    }
                }
                currentNode = nextNode;
            }
            return pathOfWaypoints;
        }
    }
}