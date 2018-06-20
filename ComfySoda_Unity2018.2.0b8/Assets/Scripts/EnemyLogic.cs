using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{

    public class EnemyLogic : MonoBehaviour
    {
        public static bool disableEnimies = false;
        public MusicManager musicManager = MusicManager.musicManager;
        public enum EnemyState { GO_TO_NEAREST_WAYPOINT, BEE_LINE_FOR_PLAYER, PHASE_TO_WAYPOINT, FADE_IN, FOLLOW_WAYPOINTS};
        public static List<WaypointBehavior> waypoints = WaypointBehavior.waypoints;
        public EnemyState currentEnemyState = EnemyState.FADE_IN;
        Rigidbody2D myRigidbode2D;
        public GameObject displayObjectsInInspector = null;
        public float fadeSpeed = 1;
        public float initialOpacity = 0.5f;
        public float enemySpeed = 5;
        public float enemyDemotivationRate = 2.0f;
        float enemyMotivation = 10.0f;
        public WaypointBehavior lastTriggeredWaypoint = null;
        public WaypointBehavior nextTargetedWaypoint = null;
        Animator tempAnimator;

        // Use this for initialization
        void Start()
        {
            myRigidbode2D = GetComponent<Rigidbody2D>(); 
            tempAnimator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            WaypointBehavior tempWaypoint = collision.gameObject.GetComponent<WaypointBehavior>();
            if(tempWaypoint!=null)
            {
                lastTriggeredWaypoint = tempWaypoint;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!disableEnimies)
            {
                if (Vector2.Distance(myRigidbode2D.position, PersistentData.playerObject.GetComponent<Rigidbody2D>().position) < 3)
                {
                    musicManager.musicState = MusicState.boss;
                }
                else if (Vector2.Distance(myRigidbode2D.position, PersistentData.playerObject.GetComponent<Rigidbody2D>().position) < 6)
                {
                    musicManager.musicState = MusicState.mob;
                }
                else
                {
                    musicManager.musicState = MusicState.main;
                }
            }


            if (DialogueManager.dialogueManager.animator.GetBool("IsOpen") || disableEnimies)
            {
                myRigidbode2D.velocity = new Vector2(0, 0);
            }
            if (!DialogueManager.dialogueManager.animator.GetBool("IsOpen") && !disableEnimies)
            {

                List<WaypointBehavior> pathOfWaypoints = null;
                WaypointBehavior startNode = null;
                if (waypoints != null && waypoints.Count >= 2)
                {
                    float? shortestDistanceToWaypoint = null;

                    List<WaypointBehavior> unvisitedSet = new List<WaypointBehavior>();

                    GameObject playerObject = PersistentData.playerObject;
                    float? shortestDistanceToPlayer = null;
                    WaypointBehavior currentNode = null;
                    WaypointBehavior targetNode = null;
                    foreach (WaypointBehavior waypoint in waypoints)
                    {

                        WaypointBehavior waypointBehavior = waypoint;
                        waypointBehavior.nodeVisited = false;
                        waypointBehavior.distance = 0;
                        waypointBehavior.infiniteDistance = true;
                        unvisitedSet.Add(waypointBehavior);

                        float distance = Vector3.Distance(playerObject.transform.position, waypoint.transform.position);
                        if (shortestDistanceToPlayer == null || shortestDistanceToPlayer > distance)
                        {
                            shortestDistanceToPlayer = distance;
                            targetNode = waypointBehavior;
                        }

                        distance = Vector3.Distance(transform.position, waypoint.transform.position);
                        if (shortestDistanceToWaypoint == null || shortestDistanceToWaypoint > distance)
                        {
                            shortestDistanceToWaypoint = distance;
                            startNode = waypointBehavior;
                        }
                    }

                    startNode.infiniteDistance = false;
                    bool pathFound = false;

                    currentNode = startNode;
                    findPath(ref pathFound, ref currentNode, ref unvisitedSet, targetNode);


                    if (pathFound)
                        pathOfWaypoints = tracePath(startNode, currentNode);
                }

                switch (currentEnemyState)
                {
                    case EnemyState.GO_TO_NEAREST_WAYPOINT:
                        if (nextTargetedWaypoint != null)
                        {
                            Vector2 directionOfNearestTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
                            myRigidbode2D.velocity = directionOfNearestTargetWaypoint.normalized * enemySpeed;
                            tempAnimator.SetFloat("Horizontal Axis", directionOfNearestTargetWaypoint.normalized.x);
                            tempAnimator.SetFloat("Vertical Axis", directionOfNearestTargetWaypoint.normalized.y);
                        }
                        else
                        {
                            currentEnemyState = EnemyState.BEE_LINE_FOR_PLAYER;
                        }
                        if (lastTriggeredWaypoint != null)
                        {
                            currentEnemyState = EnemyState.FOLLOW_WAYPOINTS;
                        }
                        break;
                    case EnemyState.BEE_LINE_FOR_PLAYER:
                        Vector2 directionOfPlayer = new Vector2(PersistentData.playerObject.transform.position.x, PersistentData.playerObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
                        if (!checkLineOfSightToPlayer())
                            enemyMotivation = Mathf.Max(enemyMotivation - ((enemyDemotivationRate) * Time.deltaTime), 0);
                        myRigidbode2D.velocity = directionOfPlayer.normalized * enemySpeed;

                        if (enemyMotivation == 0)
                        {
                            enemyMotivation = 10.0f;
                            currentEnemyState = EnemyState.PHASE_TO_WAYPOINT;
                            if (lastTriggeredWaypoint != null)
                            {
                                nextTargetedWaypoint = lastTriggeredWaypoint;
                                lastTriggeredWaypoint = null;
                            }
                            else
                            {
                                nextTargetedWaypoint = startNode;
                            }
                            GetComponent<Collider2D>().enabled = false;
                        }
                        tempAnimator.SetFloat("Horizontal Axis", directionOfPlayer.normalized.x);
                        tempAnimator.SetFloat("Vertical Axis", directionOfPlayer.normalized.y);
                        break;
                    case EnemyState.PHASE_TO_WAYPOINT:
                        Vector2 directionOfLastTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
                        myRigidbode2D.velocity = directionOfLastTargetWaypoint.normalized * enemySpeed;
                        tempAnimator.SetFloat("Horizontal Axis", directionOfLastTargetWaypoint.normalized.x);
                        tempAnimator.SetFloat("Vertical Axis", directionOfLastTargetWaypoint.normalized.y);
                        if ((Mathf.Abs((nextTargetedWaypoint.transform.position - transform.position).magnitude)) < 0.5)
                        {
                            currentEnemyState = EnemyState.FOLLOW_WAYPOINTS;
                            GetComponent<Collider2D>().enabled = true;
                        }
                        break;
                    case EnemyState.FOLLOW_WAYPOINTS:
                        if (pathOfWaypoints.Count > 1 && lastTriggeredWaypoint == pathOfWaypoints[pathOfWaypoints.Count - 1])
                        {
                            nextTargetedWaypoint = pathOfWaypoints[pathOfWaypoints.Count - 2];
                        }
                        else if (pathOfWaypoints.Count > 0 && lastTriggeredWaypoint != pathOfWaypoints[pathOfWaypoints.Count - 1])
                        {
                            nextTargetedWaypoint = pathOfWaypoints[pathOfWaypoints.Count - 1];
                        }
                        else
                        {
                            currentEnemyState = EnemyState.BEE_LINE_FOR_PLAYER;
                        }
                        Vector2 directionOfPathTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
                        myRigidbode2D.velocity = directionOfPathTargetWaypoint.normalized * enemySpeed;
                        tempAnimator.SetFloat("Horizontal Axis", directionOfPathTargetWaypoint.normalized.x);
                        tempAnimator.SetFloat("Vertical Axis", directionOfPathTargetWaypoint.normalized.y);
                        break;
                    case EnemyState.FADE_IN:
                        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Min(1.0f, (spriteRenderer.color.a + (fadeSpeed * Time.deltaTime))));
                        if (spriteRenderer.color.a >= 1)
                            currentEnemyState = EnemyState.GO_TO_NEAREST_WAYPOINT;
                        nextTargetedWaypoint = startNode;
                        lastTriggeredWaypoint = null;
                        break;
                    default:
                        Debug.LogError("Error in Enemy State Machine");
                        break;
                }

                //Debug.Log(currentEnemyState.ToString());
            }
        }

        bool checkLineOfSightToPlayer()
        {
            GameObject playerObject = PersistentData.playerObject;//the player object
            GameObject enemyObject = gameObject;//the game object enemy logic is atatched to

            bool rayCastHitPlayerObjectDirectlyViaLineOfSight = false;
            bool rtnVal = false;

            //raycast from this enemy to the player object
            //set the raycast boolean
            // Ray enemyToPlayer = new Ray ()

            int Enemy = LayerMask.NameToLayer("Enemy");
            int layerMask = (1 << Enemy);

            RaycastHit2D raycast = Physics2D.Raycast(new Vector2(enemyObject.transform.position.x, enemyObject.transform.position.y-0.5f), new Vector2(PersistentData.playerObject.transform.position.x, PersistentData.playerObject.transform.position.y-0.5f) - new Vector2(transform.position.x, transform.position.y-0.5f), Mathf.Infinity, ~layerMask);

            if(raycast.collider!=null)
            {
                if (raycast.collider.gameObject == playerObject)
                {
                    rayCastHitPlayerObjectDirectlyViaLineOfSight = true;
                }
                Debug.Log("raycast:" + raycast.collider.gameObject);
                displayObjectsInInspector = raycast.collider.gameObject;
            }

            if(rayCastHitPlayerObjectDirectlyViaLineOfSight)
            {
                rtnVal = true;
            }

            return rtnVal;
        }

        void findPath(ref bool pathFound, ref WaypointBehavior currentNode, ref List<WaypointBehavior> unvisitedSet, WaypointBehavior targetNode)
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

        List<WaypointBehavior> tracePath(WaypointBehavior closestWaypoint, WaypointBehavior currentNode)
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
