using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace cs
{

    public class WayPointSystem : MonoBehaviour
    {
        #region waypoint list
        List<WaypointBehavior> waypoints = new List<WaypointBehavior>();

        public void Add(WaypointBehavior waypointBehavior)
        {
            waypoints.Add(waypointBehavior);
        }

        public void Remove(WaypointBehavior waypointBehavior)
        {
            waypoints.Remove(waypointBehavior);
        }
        #endregion

        public WaypointBehavior getClosestWaypointToObject(GameObject originGameObject)
        {
            return getClosestWaypointToObject(originGameObject.transform);
        }

        public WaypointBehavior getClosestWaypointToObject(Transform originGameObject)
        {
            WaypointBehavior closestNode = null;
            if (waypoints != null && waypoints.Count >= 1)
            {
                float? shortestDistanceToWaypoint = null;
                float? distance = null;

                foreach (WaypointBehavior waypoint in waypoints)
                {
                    distance = Vector2.Distance(originGameObject.transform.position, waypoint.transform.position);
                    if (shortestDistanceToWaypoint == null || shortestDistanceToWaypoint > distance)
                    {
                        shortestDistanceToWaypoint = distance;
                        closestNode = waypoint;
                    }
                }
            }
            return closestNode;
        }

        public Tuple<WaypointBehavior, WaypointBehavior> getClosestEndpointsToObjects(GameObject originGameObject, GameObject targetGameObject)
        {
            return getClosestEndpointsToObjects(originGameObject.transform, targetGameObject.transform);
        }
        public Tuple<WaypointBehavior, WaypointBehavior> getClosestEndpointsToObjects(Transform originGameObject, Transform targetGameObject)
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

                    distance = Vector2.Distance(originGameObject.position, waypoint.transform.position);
                    if (shortestDistanceToWaypointFromOrigin == null || shortestDistanceToWaypointFromOrigin > distance)
                    {
                        shortestDistanceToWaypointFromOrigin = distance;
                        closestNodeToOrigin = waypoint;
                    }

                    distance = Vector2.Distance(targetGameObject.position, waypoint.transform.position);
                    if (shortestDistanceToWaypointFromTarget == null || shortestDistanceToWaypointFromTarget > distance)
                    {
                        shortestDistanceToWaypointFromTarget = distance;
                        closestNodeToTarget = waypoint;
                    }

                }
            }

            return new Tuple<WaypointBehavior, WaypointBehavior>(closestNodeToOrigin, closestNodeToTarget);
        }

        public List<WaypointBehavior> findPath(WaypointBehavior startNode, WaypointBehavior targetNode)
        {

            List<WaypointBehavior> unvisitedSet = new List<WaypointBehavior>();

            foreach (WaypointBehavior waypointBehavior in waypoints)
            {
                waypointBehavior.nodeVisited = false;
                waypointBehavior.distance = 0;
                waypointBehavior.infiniteDistance = true;
                unvisitedSet.Add(waypointBehavior);
            }
            WaypointBehavior currentNode = startNode;

            bool pathFound = false;
            while (!pathFound && currentNode != null)
            {
                currentNode.nodeVisited = true;
                unvisitedSet.Remove(currentNode);
                if (targetNode.nodeVisited)
                    pathFound = true;
                else
                {
                    for (int i = 0; i < waypoints.Count && !pathFound; i++)
                    {
                        WaypointBehavior waypoint = waypoints[i];
                        WaypointBehavior tempWaypointBehavior = waypoint;
                        if (tempWaypointBehavior.nodeVisited!=true)
                        {
                            float tentativeDistance = Vector2.Distance(currentNode.transform.position, waypoint.transform.position);
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

            if (pathFound)
                return tracePath(startNode, currentNode);
            else
                return null;
        }

        public List<WaypointBehavior> tracePath(WaypointBehavior closestWaypoint, WaypointBehavior currentNode)
        {
            List<WaypointBehavior> pathOfWaypoints = new List<WaypointBehavior>();
            while (closestWaypoint != currentNode)
            {
                pathOfWaypoints.Add(currentNode);
                float? shortestDistance = null;
                WaypointBehavior nextNode = null;
                foreach (WaypointBehavior waypointBehavior in waypoints)
                {
                    if ((shortestDistance == null || (shortestDistance > waypointBehavior.distance)) && waypointBehavior.infiniteDistance==false)
                    {
                        shortestDistance = waypointBehavior.distance;
                        nextNode = waypointBehavior;
                    }
                }
                currentNode = nextNode;
            }
            return pathOfWaypoints;
        }
    }
}