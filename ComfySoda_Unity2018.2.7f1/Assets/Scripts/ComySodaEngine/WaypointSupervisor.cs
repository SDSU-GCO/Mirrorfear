using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace cs
{

    public class WaypointSupervisor : MonoBehaviour
    {
        #region waypoint list
        List<WaypointObjectController> waypoints = new List<WaypointObjectController>();

        public void Add(WaypointObjectController waypointBehavior)
        {
            waypoints.Add(waypointBehavior);
        }

        public void Remove(WaypointObjectController waypointBehavior)
        {
            waypoints.Remove(waypointBehavior);
        }
        #endregion

        #region get endpoint(s) function(s)
        public WaypointObjectController getClosestWaypointToObject(GameObject originGameObject)
        {
            return getClosestWaypointToObject(originGameObject.transform);
        }

        public WaypointObjectController getClosestWaypointToObject(Transform originGameObject)
        {
            WaypointObjectController closestNode = null;
            if (waypoints != null && waypoints.Count >= 1)
            {
                float? shortestDistanceToWaypoint = null;
                float? distance = null;

                foreach (WaypointObjectController waypoint in waypoints)
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

        public Tuple<WaypointObjectController, WaypointObjectController> getClosestEndpointsToObjects(GameObject originGameObject, GameObject targetGameObject)
        {
            return getClosestEndpointsToObjects(originGameObject.transform, targetGameObject.transform);
        }
        public Tuple<WaypointObjectController, WaypointObjectController> getClosestEndpointsToObjects(Transform originGameObject, Transform targetGameObject)
        {

            WaypointObjectController closestNodeToOrigin = null;
            WaypointObjectController closestNodeToTarget = null;
            if (originGameObject!=null && targetGameObject!= null && waypoints != null && waypoints.Count >= 1)
            {
                float? shortestDistanceToWaypointFromOrigin = null;
                float? shortestDistanceToWaypointFromTarget = null;
                float? distance = null;

                foreach (WaypointObjectController waypoint in waypoints)
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

            return new Tuple<WaypointObjectController, WaypointObjectController>(closestNodeToOrigin, closestNodeToTarget);
        }
        #endregion

        public List<WaypointObjectController> tracePath(WaypointObjectController startNode, WaypointObjectController targetNode)
        {
            #region clear nodes & initialize system
            List<WaypointObjectController> unvisitedSet = new List<WaypointObjectController>();
            foreach (WaypointObjectController waypointBehavior in waypoints)
            {
                waypointBehavior.nodeVisited = false;
                waypointBehavior.distance = null;
                unvisitedSet.Add(waypointBehavior);
            }

            bool pathFound = false;
            WaypointObjectController currentNode = startNode;
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
                        List<WaypointObjectController> adjacentSet = currentNode.GetWaypointBehaviors();
                        CalculateDistanceAndTentativedistanceOfAdjacentNodes(adjacentSet, currentNode);

                        currentNode = GoToClosestUnvisitedNode(unvisitedSet);
                    }
                }
            }
            

            #region return path
            List<WaypointObjectController> pathOfWaypoints = null;
            if (pathFound)
                pathOfWaypoints = getPath(startNode, currentNode);

            return pathOfWaypoints;
            #endregion
        }

        private static void CalculateDistanceAndTentativedistanceOfAdjacentNodes(List<WaypointObjectController> adjacentSet, WaypointObjectController currentNode)
        {
            if(currentNode.distance==null)
                currentNode.distance=0;
            foreach (WaypointObjectController waypoint in adjacentSet)
            {
                float? tentativeDistance = Vector2.Distance(currentNode.transform.position, waypoint.transform.position) + currentNode.distance;
                if (waypoint.distance==null || tentativeDistance < waypoint.distance)
                {
                    waypoint.distance = tentativeDistance;
                }
            }
        }

        private static WaypointObjectController GoToClosestUnvisitedNode(List<WaypointObjectController> unvisitedSet)
        {
            WaypointObjectController nextNode = null;
            foreach (WaypointObjectController unvisitedNode in unvisitedSet)
            {
                if (unvisitedNode.nodeVisited!=true && ((nextNode != null && unvisitedNode.distance!=null && unvisitedNode.distance < nextNode.distance ) || (nextNode == null && unvisitedNode.distance != null)))
                {
                    nextNode = unvisitedNode;
                }
            }



            return nextNode;
        }

        public List<WaypointObjectController> getPath(WaypointObjectController startNode, WaypointObjectController lastNodeTraversed)
        {
            WaypointObjectController currentNode = lastNodeTraversed;
            List<WaypointObjectController> pathOfWaypoints = new List<WaypointObjectController>();
            float? shortestDistance = null;
            WaypointObjectController nextNode = null;

            //trace backwards through the network taking the shortest path to the start.
            while (currentNode != null && startNode != currentNode)
            {
                pathOfWaypoints.Add(currentNode); //add node to a path list that can act as a network map
                shortestDistance = null;
                nextNode = null;
                foreach (WaypointObjectController waypointBehavior in currentNode.GetWaypointBehaviors())
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