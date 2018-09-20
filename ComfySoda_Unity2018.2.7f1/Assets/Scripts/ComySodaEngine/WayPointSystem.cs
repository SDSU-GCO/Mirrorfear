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
            if (originGameObject!=null && targetGameObject!= null && waypoints != null && waypoints.Count >= 1)
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
                waypointBehavior.distance = null;
                unvisitedSet.Add(waypointBehavior);
            }

            bool pathFound = false;
            WaypointBehavior currentNode = startNode;
            #endregion

            if(startNode!=null && targetNode!=null)
            {
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
                        List<WaypointBehavior> adjacentSet = currentNode.GetWaypointBehaviors();
                        CalculateDistanceAndTentativedistance(adjacentSet, currentNode);

                        currentNode = GoToClosestAdjacentNode(adjacentSet);
                    }
                }
            }
            

            #region return path
            List<WaypointBehavior> pathOfWaypoints = null;
            if (pathFound)
                pathOfWaypoints = getPath(startNode, currentNode);

            return pathOfWaypoints;
            #endregion
        }

        private static void CalculateDistanceAndTentativedistance(List<WaypointBehavior> adjacentSet, WaypointBehavior currentNode)
        {
            if(currentNode.distance==null)
                currentNode.distance=0;
            foreach (WaypointBehavior waypoint in adjacentSet)
            {
                float? tentativeDistance = Vector2.Distance(currentNode.transform.position, waypoint.transform.position) + currentNode.distance;
                if (waypoint.distance==null || tentativeDistance < waypoint.distance)
                {
                    waypoint.distance = tentativeDistance;
                }
            }
        }

        private static WaypointBehavior GoToClosestAdjacentNode(List<WaypointBehavior> adjacentSet)
        {
            WaypointBehavior nextNode = null;
            foreach (WaypointBehavior adjacentNode in adjacentSet)
            {
                if ((nextNode != null && adjacentNode.distance!=null && adjacentNode.distance < nextNode.distance ) || (nextNode == null && adjacentNode.distance != null))
                {
                    nextNode = adjacentNode;
                }
            }
            return nextNode;
        }

        public List<WaypointBehavior> getPath(WaypointBehavior startNode, WaypointBehavior lastNodeTraversed)
        {
            WaypointBehavior currentNode = lastNodeTraversed;
            List<WaypointBehavior> pathOfWaypoints = new List<WaypointBehavior>();
            float? shortestDistance = null;
            WaypointBehavior nextNode = null;

            //trace backwards through the network taking the shortest path to the start.
            while (startNode != currentNode && nextNode != null)
            {
                pathOfWaypoints.Add(currentNode); //add node to a path list that can act as a network map
                shortestDistance = null;
                nextNode = null;
                foreach (WaypointBehavior waypointBehavior in currentNode.GetWaypointBehaviors())
                {
                    if ((shortestDistance == null || (shortestDistance > waypointBehavior.distance)) && waypointBehavior.distance!=null)
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