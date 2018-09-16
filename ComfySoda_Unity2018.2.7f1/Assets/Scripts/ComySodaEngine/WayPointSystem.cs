using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

        #region get endpoint(s) function(s)
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
        #endregion

        public List<WaypointBehavior> tracePath(WaypointBehavior startNode, WaypointBehavior targetNode)
        {


            #region clear nodes & initialize system
            List<WaypointBehavior> unvisitedSet = new List<WaypointBehavior>();
            foreach (WaypointBehavior waypointBehavior in waypoints)
            {
                waypointBehavior.nodeVisited = false;
                waypointBehavior.distance = 0;
                waypointBehavior.infiniteDistance = true;
                unvisitedSet.Add(waypointBehavior);
            }

            bool pathFound = false;
            WaypointBehavior currentNode = startNode;
            #endregion


            while (!pathFound && currentNode != null)
            {
                #region visit current node
                currentNode.nodeVisited = true;
                unvisitedSet.Remove(currentNode);
                #endregion

                if (targetNode.nodeVisited == true)
                    pathFound = true;
                else
                {
                    CalculateDistanceAndTentativedistance(unvisitedSet, currentNode);

                    currentNode = GoToClosestAdjacentNode(unvisitedSet);
                }
            }

            #region return path
            if (pathFound)
                return getPath(startNode, currentNode);
            else
                return null;
            #endregion
        }

        private static void CalculateDistanceAndTentativedistance(List<WaypointBehavior> unvisitedSet, WaypointBehavior currentNode)
        {
            foreach (WaypointBehavior waypoint in unvisitedSet)
            {
                float tentativeDistance = Vector2.Distance(currentNode.transform.position, waypoint.transform.position);
                if (tentativeDistance < waypoint.distance || waypoint.infiniteDistance)
                {
                    waypoint.infiniteDistance = false;
                    waypoint.distance = currentNode.distance + tentativeDistance;
                }
            }
        }

        private static WaypointBehavior GoToClosestAdjacentNode(List<WaypointBehavior> unvisitedSet)
        {
            WaypointBehavior nextNode = null;
            for (int j = 0; j < unvisitedSet.Count; j++)
            {
                if ((nextNode != null && unvisitedSet[j].distance < nextNode.distance && unvisitedSet[j].infiniteDistance == false) || (nextNode == null && unvisitedSet[j].infiniteDistance == false))
                {
                    nextNode = unvisitedSet[j];
                }
            }
            return nextNode;
        }

        public List<WaypointBehavior> getPath(WaypointBehavior closestWaypoint, WaypointBehavior currentNode)
        {
            List<WaypointBehavior> pathOfWaypoints = new List<WaypointBehavior>();
            float? shortestDistance = null;
            WaypointBehavior nextNode = null;

            //trace backwards through the network taking the shortest path to the start.
            while (closestWaypoint != currentNode)
            {
                pathOfWaypoints.Add(currentNode); //add node to a path list that can act as a network map
                shortestDistance = null;
                nextNode = null;
                foreach (WaypointBehavior waypointBehavior in currentNode.GetWaypointBehaviors())
                {
                    if ((shortestDistance == null || (shortestDistance > waypointBehavior.distance)) && waypointBehavior.infiniteDistance==false)
                    {
                        shortestDistance = waypointBehavior.distance;
                        nextNode = waypointBehavior;
                    }
                }
                currentNode = nextNode;
            }

            //returns a path throught the network
            return pathOfWaypoints;
        }
    }
}