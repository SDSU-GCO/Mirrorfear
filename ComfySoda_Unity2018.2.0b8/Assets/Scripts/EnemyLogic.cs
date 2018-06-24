using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{

    public class EnemyLogic : MonoBehaviour
    {
        public static List<EnemyLogic> enemyLogicsInBossRange = new List<EnemyLogic>();
        public static List<EnemyLogic> enemyLogicsInMobRange = new List<EnemyLogic>();
        public static bool disableEnimies = false;
        SpriteRenderer spriteRenderer;
        public float fadeSpeed = 0.01f;
        public float initialOpacity = 0.5f;
        public float enemySpeed = 4;
        public float enemyDemotivationRate = 10.0f;
        public float enemysCurrentMotivation = 100.0f;
        private float enemyDefaultMotivation;
        public GameObject raycastTarget = null;
        public enum EnemyState { GO_TO_NEAREST_WAYPOINT, BEE_LINE_FOR_PLAYER, PHASE_TO_WAYPOINT, FADE_IN, FOLLOW_WAYPOINTS };
        public EnemyState currentEnemyState = EnemyState.FADE_IN;
        public EnemyState previousEnemyState = EnemyState.FADE_IN;
        public WaypointBehavior lastTriggeredWaypoint = null;
        public WaypointBehavior nextTargetedWaypoint = null;
        List<WaypointBehavior> waypoints = WaypointBehavior.waypoints;
        Rigidbody2D myRigidbody2D;
        Animator tempAnimator;


        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyDefaultMotivation = enemysCurrentMotivation;
            myRigidbody2D = GetComponent<Rigidbody2D>();
            tempAnimator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            WaypointBehavior tempWaypoint = collision.gameObject.GetComponent<WaypointBehavior>();
            if(tempWaypoint!=null)
            {
                lastTriggeredWaypoint = tempWaypoint;
                if(currentEnemyState==EnemyState.FOLLOW_WAYPOINTS)
                {
                    enemysCurrentMotivation = enemyDefaultMotivation;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            bool enemyHasLineOfSightOnPlayer = checkLineOfSightToPlayer();

            if (enemyHasLineOfSightOnPlayer)
            {
                enemysCurrentMotivation = enemyDefaultMotivation;
            }

            if (currentEnemyState != previousEnemyState)
            {
                previousEnemyState = currentEnemyState;
            }

            if (!disableEnimies)
            {
                if (enemyHasLineOfSightOnPlayer)
                {
                    if (!enemyLogicsInBossRange.Contains(this))
                    {
                        enemyLogicsInBossRange.Add(this);
                    }
                    if (!enemyLogicsInMobRange.Contains(this))
                    {
                        enemyLogicsInMobRange.Add(this);
                    }
                }
                else if(Vector2.Distance(new Vector2(PlayerLogic.playerObject.transform.position.x, PlayerLogic.playerObject.transform.position.y), new Vector2(transform.position.x, transform.position.y-0.5f))<3.0f)
                {
                    if (enemyLogicsInBossRange.Contains(this))
                    {
                        enemyLogicsInBossRange.Remove(this);
                    }
                    if (!enemyLogicsInMobRange.Contains(this))
                    {
                        enemyLogicsInMobRange.Add(this);
                    }
                }
                else
                {
                    if (enemyLogicsInBossRange.Contains(this))
                    {
                        enemyLogicsInBossRange.Remove(this);
                    }
                    if (enemyLogicsInMobRange.Contains(this))
                    {
                        enemyLogicsInMobRange.Remove(this);
                    }
                }
            }


            if (disableEnimies)
            {
                myRigidbody2D.velocity = new Vector2(0, 0);
            }
            else
            {

                List<WaypointBehavior> pathOfWaypoints = null;
                WaypointBehavior startNode = null;
                if (waypoints != null && waypoints.Count >= 2)
                {
                    float? shortestDistanceToWaypoint = null;

                    List<WaypointBehavior> unvisitedSet = new List<WaypointBehavior>();

                    GameObject playerObject = PlayerLogic.playerObject;
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


                        if(lastTriggeredWaypoint!=null && currentEnemyState!=EnemyState.GO_TO_NEAREST_WAYPOINT)
                        {
                            startNode = lastTriggeredWaypoint;
                        }
                        else
                        {
                            distance = Vector3.Distance(transform.position, waypoint.transform.position);
                            if (shortestDistanceToWaypoint == null || shortestDistanceToWaypoint > distance)
                            {
                                shortestDistanceToWaypoint = distance;
                                startNode = waypointBehavior;
                            }
                        }
                    }

                    startNode.infiniteDistance = false;
                    bool pathFound = false;

                    currentNode = startNode;
                    findPath(ref pathFound, ref currentNode, ref unvisitedSet, targetNode);


                    if (pathFound)
                        pathOfWaypoints = tracePath(startNode, currentNode);
                }

                if (enemysCurrentMotivation <= 0)
                {
                    nextTargetedWaypoint = startNode;
                    currentEnemyState = EnemyState.GO_TO_NEAREST_WAYPOINT;

                    if (enemysCurrentMotivation <= -150)
                    {
                        enemysCurrentMotivation = enemyDefaultMotivation;
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
                }
                switch (currentEnemyState)
                {
                    case EnemyState.GO_TO_NEAREST_WAYPOINT:
                        if (enemyHasLineOfSightOnPlayer)
                        {
                            currentEnemyState = EnemyState.BEE_LINE_FOR_PLAYER;
                        }
                        nextTargetedWaypoint = startNode;

                        if (Vector2.Distance( new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y), new Vector2(transform.position.x, transform.position.y-0.5f)) < 0.1f)
                        {
                            currentEnemyState = EnemyState.FOLLOW_WAYPOINTS;
                            enemysCurrentMotivation = enemyDefaultMotivation;
                        }

                        Vector2 directionOfNearestTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y-0.5f);
                        myRigidbody2D.velocity = directionOfNearestTargetWaypoint.normalized * enemySpeed;
                        tempAnimator.SetFloat("Horizontal Axis", directionOfNearestTargetWaypoint.normalized.x);
                        tempAnimator.SetFloat("Vertical Axis", directionOfNearestTargetWaypoint.normalized.y);

                        if (!enemyHasLineOfSightOnPlayer && myRigidbody2D.velocity.magnitude <= 0.2)
                            enemysCurrentMotivation = Mathf.Max(enemysCurrentMotivation - ((enemyDemotivationRate) * Time.deltaTime), float.MinValue);

                        break;
                    case EnemyState.BEE_LINE_FOR_PLAYER:
                        Vector2 directionOfPlayer = new Vector2(PlayerLogic.playerObject.transform.position.x, PlayerLogic.playerObject.transform.position.y -0.5f) - new Vector2(transform.position.x, transform.position.y-0.5f);
                        if (!enemyHasLineOfSightOnPlayer && myRigidbody2D.velocity.magnitude <= 0.2)
                            enemysCurrentMotivation = Mathf.Max(enemysCurrentMotivation - ((enemyDemotivationRate) * Time.deltaTime), float.MinValue);
                        myRigidbody2D.velocity = directionOfPlayer.normalized * enemySpeed;
                        tempAnimator.SetFloat("Horizontal Axis", directionOfPlayer.normalized.x);
                        tempAnimator.SetFloat("Vertical Axis", directionOfPlayer.normalized.y);
                        break;
                    case EnemyState.PHASE_TO_WAYPOINT:
                        if(nextTargetedWaypoint==null)
                        {
                            nextTargetedWaypoint = startNode;
                        }
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, initialOpacity);
                        Vector2 directionOfLastTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y-0.5f);
                        myRigidbody2D.velocity = directionOfLastTargetWaypoint.normalized * enemySpeed;
                        tempAnimator.SetFloat("Horizontal Axis", directionOfLastTargetWaypoint.normalized.x);
                        tempAnimator.SetFloat("Vertical Axis", directionOfLastTargetWaypoint.normalized.y);
                        if (Vector2.Distance(new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y), new Vector2(transform.position.x, transform.position.y - 0.5f)) < 0.1)
                        {
                            currentEnemyState = EnemyState.FADE_IN;
                            GetComponent<Collider2D>().enabled = true;
                            lastTriggeredWaypoint = nextTargetedWaypoint;
                            enemysCurrentMotivation = enemyDefaultMotivation;
                        }
                        break;
                    case EnemyState.FOLLOW_WAYPOINTS:
                        if (!enemyHasLineOfSightOnPlayer && myRigidbody2D.velocity.magnitude <= 0.2)
                            enemysCurrentMotivation = Mathf.Max(enemysCurrentMotivation - ((enemyDemotivationRate) * Time.deltaTime), float.MinValue);

                        Vector2 directionOfPathTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
                        myRigidbody2D.velocity = directionOfPathTargetWaypoint.normalized * enemySpeed;
                        tempAnimator.SetFloat("Horizontal Axis", directionOfPathTargetWaypoint.normalized.x);
                        tempAnimator.SetFloat("Vertical Axis", directionOfPathTargetWaypoint.normalized.y);

                        if (pathOfWaypoints.Count > 1 && lastTriggeredWaypoint == pathOfWaypoints[pathOfWaypoints.Count - 1])
                        {
                            nextTargetedWaypoint = pathOfWaypoints[pathOfWaypoints.Count - 2];
                        }
                        else if (pathOfWaypoints.Count > 0 && lastTriggeredWaypoint != pathOfWaypoints[pathOfWaypoints.Count - 1])
                        {
                            nextTargetedWaypoint = pathOfWaypoints[pathOfWaypoints.Count - 1];
                        }
                        else if(enemyHasLineOfSightOnPlayer)
                        {
                            currentEnemyState = EnemyState.BEE_LINE_FOR_PLAYER;
                        }
                        if(nextTargetedWaypoint == lastTriggeredWaypoint)
                        {
                            myRigidbody2D.velocity = new Vector2(0, 0);
                        }
                        break;
                    case EnemyState.FADE_IN:
                        enemysCurrentMotivation = enemyDefaultMotivation;
                        if (previousEnemyState!= EnemyState.FADE_IN)
                        {
                            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, initialOpacity);
                        }
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
            GameObject playerObject = PlayerLogic.playerObject;//the player object
            GameObject enemyObject = gameObject;//the game object enemy logic is atatched to

            bool rayCastHitPlayerObjectDirectlyViaLineOfSight = false;
            bool rtnVal = false;

            //raycast from this enemy to the player object
            //set the raycast boolean
            // Ray enemyToPlayer = new Ray ()

            int Enemy = LayerMask.NameToLayer("Enemy");
            int layerMask = (1 << Enemy);

            RaycastHit2D raycast = Physics2D.Raycast(new Vector2(enemyObject.transform.position.x, enemyObject.transform.position.y-0.5f), new Vector2(PlayerLogic.playerObject.transform.position.x, PlayerLogic.playerObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y-0.5f), Mathf.Infinity, ~layerMask);

            if(raycast.collider!=null)
            {
                if (raycast.collider.gameObject == playerObject)
                {
                    rayCastHitPlayerObjectDirectlyViaLineOfSight = true;
                }
                raycastTarget = raycast.collider.gameObject;
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
