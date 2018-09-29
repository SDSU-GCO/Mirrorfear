using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace cs
{

    public class EnemyObjectController : MonoBehaviour
    {
        public WaypointSupervisor targetedWayPointSystem = null;
        public GameObject prey = null;
        public static List<EnemyObjectController> enemyLogicsInBossRange = new List<EnemyObjectController>();
        public static List<EnemyObjectController> enemyLogicsInMobRange = new List<EnemyObjectController>();
        public static bool disableEnimies = false;
        SpriteRenderer spriteRenderer;
        public float fadeSpeed = 0.01f;
        public float initialOpacity = 0.5f;
        public float enemySpeed = 4;
        public float enemyDemotivationRate = 10.0f;
        public float enemysCurrentMotivation = 100.0f;
        float enemyDefaultMotivation;
        enum EnemyState { GO_TO_NEAREST_WAYPOINT, BEE_LINE_FOR_PREY, PHASE_TO_WAYPOINT, FADE_IN, FOLLOW_WAYPOINTS };
        [SerializeField]
        EnemyState currentEnemyState = EnemyState.FADE_IN;
        [SerializeField]
        EnemyState previousEnemyState = EnemyState.FADE_IN;
        [SerializeField]
        WaypointObjectController lastTriggeredWaypoint = null;
        [SerializeField]
        WaypointObjectController nextTargetedWaypoint = null;
        Rigidbody2D myRigidbody2D;
        Animator animator;

        public static List<EnemyObjectController> enemyList = new List<EnemyObjectController>();

        private void OnEnable()
        {
            enemyList.Add(this);
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyDefaultMotivation = enemysCurrentMotivation;
            myRigidbody2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            enemyList.Remove(this);
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            WaypointObjectController tempWaypoint = collision.gameObject.GetComponent<WaypointObjectController>();
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
            bool enemyHasLineOfSightOnPrey = checkLineOfSightToPrey();

            if (enemyHasLineOfSightOnPrey)
            {
                enemysCurrentMotivation = enemyDefaultMotivation;
            }

            if (currentEnemyState != previousEnemyState)
            {
                previousEnemyState = currentEnemyState;
            }

            if (!disableEnimies)
            {
                if (enemyHasLineOfSightOnPrey)
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
                else if(Vector2.Distance(new Vector2(prey.transform.position.x, prey.transform.position.y), new Vector2(transform.position.x, transform.position.y-0.25f))<3.0f)
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

                List<WaypointObjectController> pathOfWaypoints = null;
                WaypointObjectController startNode = null;
                WaypointObjectController targetNode = null;

                if (lastTriggeredWaypoint != null && currentEnemyState != EnemyState.GO_TO_NEAREST_WAYPOINT)
                {
                    startNode = lastTriggeredWaypoint;
                    targetNode = targetedWayPointSystem.getClosestWaypointToObject(prey.transform);
                }
                else
                {
                    targetedWayPointSystem.getClosestEndpointsToObjects(transform, prey.transform).Deconstruct(out startNode, out targetNode);
                    lastTriggeredWaypoint = startNode;
                }


                pathOfWaypoints = targetedWayPointSystem.tracePath(startNode, targetNode);


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
                        if (enemyHasLineOfSightOnPrey)
                        {
                            currentEnemyState = EnemyState.BEE_LINE_FOR_PREY;
                        }
                        nextTargetedWaypoint = startNode;

                        if (Vector2.Distance( new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y), new Vector2(transform.position.x, transform.position.y-0.25f)) < 0.25f)
                        {
                            currentEnemyState = EnemyState.FOLLOW_WAYPOINTS;
                            enemysCurrentMotivation = enemyDefaultMotivation;
                        }

                        if (!enemyHasLineOfSightOnPrey && myRigidbody2D.velocity.magnitude <= 0.2)
                            enemysCurrentMotivation = Mathf.Max(enemysCurrentMotivation - ((enemyDemotivationRate) * Time.deltaTime), float.MinValue);

                        Vector2 directionOfNearestTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y-0.25f);
                        myRigidbody2D.velocity = directionOfNearestTargetWaypoint.normalized * enemySpeed;
                        animator.SetFloat("Horizontal Axis", directionOfNearestTargetWaypoint.normalized.x);
                        animator.SetFloat("Vertical Axis", directionOfNearestTargetWaypoint.normalized.y);
                        break;
                    case EnemyState.BEE_LINE_FOR_PREY:
                        Vector2 directionOfPrey = new Vector2(prey.transform.position.x, prey.transform.position.y -0.25f) - new Vector2(transform.position.x, transform.position.y-0.25f);
                        if (!enemyHasLineOfSightOnPrey && myRigidbody2D.velocity.magnitude <= 0.2)
                            enemysCurrentMotivation = Mathf.Max(enemysCurrentMotivation - ((enemyDemotivationRate) * Time.deltaTime), float.MinValue);
                        myRigidbody2D.velocity = directionOfPrey.normalized * enemySpeed;
                        animator.SetFloat("Horizontal Axis", directionOfPrey.normalized.x);
                        animator.SetFloat("Vertical Axis", directionOfPrey.normalized.y);
                        break;
                    case EnemyState.PHASE_TO_WAYPOINT:
                        if(nextTargetedWaypoint==null)
                        {
                            nextTargetedWaypoint = startNode;
                        }
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, initialOpacity);
                        Vector2 directionOfLastTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y-0.25f);
                        myRigidbody2D.velocity = directionOfLastTargetWaypoint.normalized * enemySpeed;
                        animator.SetFloat("Horizontal Axis", directionOfLastTargetWaypoint.normalized.x);
                        animator.SetFloat("Vertical Axis", directionOfLastTargetWaypoint.normalized.y);
                        if (Vector2.Distance(new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y), new Vector2(transform.position.x, transform.position.y - 0.25f)) < 0.25)
                        {
                            currentEnemyState = EnemyState.FADE_IN;
                            GetComponent<Collider2D>().enabled = true;
                            lastTriggeredWaypoint = nextTargetedWaypoint;
                            enemysCurrentMotivation = enemyDefaultMotivation;
                        }
                        break;
                    case EnemyState.FOLLOW_WAYPOINTS:
                        if (!enemyHasLineOfSightOnPrey && myRigidbody2D.velocity.magnitude <= 0.2)
                            enemysCurrentMotivation = Mathf.Max(enemysCurrentMotivation - ((enemyDemotivationRate) * Time.deltaTime), float.MinValue);

                        Vector2 directionOfPathTargetWaypoint = new Vector2(nextTargetedWaypoint.transform.position.x, nextTargetedWaypoint.transform.position.y) - new Vector2(transform.position.x, transform.position.y-0.25f);
                        myRigidbody2D.velocity = directionOfPathTargetWaypoint.normalized * enemySpeed;
                        animator.SetFloat("Horizontal Axis", directionOfPathTargetWaypoint.normalized.x);
                        animator.SetFloat("Vertical Axis", directionOfPathTargetWaypoint.normalized.y);

                        Debug.Assert(pathOfWaypoints!=null, "Error, No waypoint path found!");

                        if (pathOfWaypoints.Count > 1 && lastTriggeredWaypoint == pathOfWaypoints[pathOfWaypoints.Count - 1])
                        {
                            nextTargetedWaypoint = pathOfWaypoints[pathOfWaypoints.Count - 2];
                        }
                        else if (pathOfWaypoints.Count > 0 && lastTriggeredWaypoint != pathOfWaypoints[pathOfWaypoints.Count - 1])
                        {
                            nextTargetedWaypoint = pathOfWaypoints[pathOfWaypoints.Count - 1];
                        }
                        else if(enemyHasLineOfSightOnPrey)
                        {
                            currentEnemyState = EnemyState.BEE_LINE_FOR_PREY;
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

        bool checkLineOfSightToPrey()
        {
            GameObject enemyObject = gameObject;//the game object enemy logic is atatched to

            bool rayCastHitPreyObjectDirectlyViaLineOfSight = false;
            bool rtnVal = false;

            //raycast from this enemy to the prey object
            //set the raycast boolean
            // Ray enemyToPrey = new Ray ()

            int Enemy = LayerMask.NameToLayer("Enemy");
            int layerMask = (1 << Enemy);

            RaycastHit2D raycast = Physics2D.Raycast(new Vector2(enemyObject.transform.position.x, enemyObject.transform.position.y-0.25f), new Vector2(PlayerObjectController.playerObjectController.transform.position.x, PlayerObjectController.playerObjectController.transform.position.y-0.25f) - new Vector2(transform.position.x, transform.position.y-0.25f), Mathf.Infinity, ~layerMask);

            if(raycast.collider!=null)
            {
                if (raycast.collider.gameObject.tag == prey.tag)
                {
                    rayCastHitPreyObjectDirectlyViaLineOfSight = true;
                }
            }

            if(rayCastHitPreyObjectDirectlyViaLineOfSight)
            {
                rtnVal = true;
            }

            return rtnVal;
        }
    }

}

