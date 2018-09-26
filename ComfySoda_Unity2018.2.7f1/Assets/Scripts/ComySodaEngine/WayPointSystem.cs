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
        List<Waypoint> waypoints = new List<Waypoint>();

        public void Add(Waypoint waypointBehavior)
        {
            waypoints.Add(waypointBehavior);
        }

        public void Remove(Waypoint waypointBehavior)
        {
            waypoints.Remove(waypointBehavior);
        }
        #endregion

        #region get endpoint(s) function(s)
        public Waypoint getClosestWaypointToObject(GameObject originGameObject)
        {
            return getClosestWaypointToObject(originGameObject.transform);
        }

        public Waypoint getClosestWaypointToObject(Transform originGameObject)
        {
            Waypoint closestNode = null;
            if (waypoints != null && waypoints.Count >= 1)
            {
                float? shortestDistanceToWaypoint = null;
                float? distance = null;

                foreach (Waypoint waypoint in waypoints)
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

        public Tuple<Waypoint, Waypoint> getClosestEndpointsToObjects(GameObject originGameObject, GameObject targetGameObject)
        {
            return getClosestEndpointsToObjects(originGameObject.transform, targetGameObject.transform);
        }
        public Tuple<Waypoint, Waypoint> getClosestEndpointsToObjects(Transform originGameObject, Transform targetGameObject)
        {

            Waypoint closestNodeToOrigin = null;
            Waypoint closestNodeToTarget = null;
            if (originGameObject!=null && targetGameObject!= null && waypoints != null && waypoints.Count >= 1)
            {
                float? shortestDistanceToWaypointFromOrigin = null;
                float? shortestDistanceToWaypointFromTarget = null;
                float? distance = null;

                foreach (Waypoint waypoint in waypoints)
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

            return new Tuple<Waypoint, Waypoint>(closestNodeToOrigin, closestNodeToTarget);
        }
        #endregion

        public List<Waypoint> tracePath(Waypoint startNode, Waypoint targetNode)
        {
            #region clear nodes & initialize system
            List<Waypoint> unvisitedSet = new List<Waypoint>();
            foreach (Waypoint waypointBehavior in waypoints)
            {
                waypointBehavior.nodeVisited = false;
                waypointBehavior.distance = null;
                unvisitedSet.Add(waypointBehavior);
            }

            bool pathFound = false;
            Waypoint currentNode = startNode;
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
                        List<Waypoint> adjacentSet = currentNode.GetWaypointBehaviors();
                        CalculateDistanceAndTentativedistanceOfAdjacentNodes(adjacentSet, currentNode);

                        currentNode = GoToClosestUnvisitedNode(unvisitedSet);
                    }
                }
            }
            

            #region return path
            List<Waypoint> pathOfWaypoints = null;
            if (pathFound)
                pathOfWaypoints = getPath(startNode, currentNode);

            return pathOfWaypoints;
            #endregion
        }

        private static void CalculateDistanceAndTentativedistanceOfAdjacentNodes(List<Waypoint> adjacentSet, Waypoint currentNode)
        {
            if(currentNode.distance==null)
                currentNode.distance=0;
            foreach (Waypoint waypoint in adjacentSet)
            {
                float? tentativeDistance = Vector2.Distance(currentNode.transform.position, waypoint.transform.position) + currentNode.distance;
                if (waypoint.distance==null || tentativeDistance < waypoint.distance)
                {
                    waypoint.distance = tentativeDistance;
                }
            }
        }

        private static Waypoint GoToClosestUnvisitedNode(List<Waypoint> unvisitedSet)
        {
            Waypoint nextNode = null;
            foreach (Waypoint unvisitedNode in unvisitedSet)
            {
                if (unvisitedNode.nodeVisited!=true && ((nextNode != null && unvisitedNode.distance!=null && unvisitedNode.distance < nextNode.distance ) || (nextNode == null && unvisitedNode.distance != null)))
                {
                    nextNode = unvisitedNode;
                }
            }



            return nextNode;
        }

        public List<Waypoint> getPath(Waypoint startNode, Waypoint lastNodeTraversed)
        {
            Waypoint currentNode = lastNodeTraversed;
            List<Waypoint> pathOfWaypoints = new List<Waypoint>();
            float? shortestDistance = null;
            Waypoint nextNode = null;

            //trace backwards through the network taking the shortest path to the start.
            while (startNode != currentNode && nextNode != null)
            {
                pathOfWaypoints.Add(currentNode); //add node to a path list that can act as a network map
                shortestDistance = null;
                nextNode = null;
                foreach (Waypoint waypointBehavior in currentNode.GetWaypointBehaviors())
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