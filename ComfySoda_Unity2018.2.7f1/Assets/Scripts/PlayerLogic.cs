using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    [RequireComponent(typeof(PlayerPersistantDataComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerLogic : MonoBehaviour
    {
        public static PlayerLogic playerLogic = null;
        public float speed = 1.0f;

        private PlayerPersistantDataComponent playerPersistantDataComponent;
        private Rigidbody2D playersRigidBody2D;

        private bool canMove = true;

        public Vector2? targetPosition
        {
            get
            {
                Debug.Assert(playerPersistantDataComponent.playerPersistantData != null, "Error: PlayerPersistentDataComponent does not have a PlayerPersistentData Asset!");
                return playerPersistantDataComponent.playerPersistantData.playerPosition;
            }
            set
            {
                Debug.Assert(playerPersistantDataComponent.playerPersistantData != null, "Error: PlayerPersistentDataComponent does not have a PlayerPersistentData Asset!");
                playerPersistantDataComponent.playerPersistantData.playerPosition = value;
            }
        }

        // Use this for initialization
        private void Awake()
        {
            Debug.Assert(playerLogic == null, "Error: There is more than one player in the scene, is this intentional?");
            playerLogic = this;
            enableMovement = true;
        }

        private void Start()
        {
            playersRigidBody2D = GetComponent<Rigidbody2D>();
            playerPersistantDataComponent = GetComponent<PlayerPersistantDataComponent>();

            if (targetPosition != null)
            {
                transform.position = new Vector3((float)targetPosition.Value.x, (float)targetPosition.Value.y, transform.position.z);
            }
            targetPosition = null;

        }

        private void OnDestroy()
        {
            if (playerLogic == this)
            {
                playerLogic = null;
            }
            else
            {
                Debug.LogWarning("There is more than one player in the scene, is this intentional?");
            }
        }

        delegate void updateFunction();
        updateFunction movementUpdate;

        public bool enableMovement
        {
            get { return canMove; }
            set
            {
                if (!value)
                {
                    playersRigidBody2D.velocity = Vector2.zero;
                    movementUpdate = VoidBasedUpdate;
                }
                else
                {
                    movementUpdate = inputBasedUpdate;
                }
                canMove = value;
            }
        }

        // Update is called once per frame
        void Update()
        {
            movementUpdate();
        }

        void inputBasedUpdate()
        {
            playersRigidBody2D.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed;
        }

        void VoidBasedUpdate()
        {

        }
    }

}

