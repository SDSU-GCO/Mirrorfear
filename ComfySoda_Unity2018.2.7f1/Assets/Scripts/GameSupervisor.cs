using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cs
{
    public class GameSupervisor : MonoBehaviour
    {
        static public GameSupervisor gameSupervisor;

        public delegate void sceneChangingDelegate();
        public delegate void gameSaveDelegate();

        public static event sceneChangingDelegate sceneChangingEvent;
        public static event gameSaveDelegate saveGameEvent;

        public string saveGamePath { get; private set; } = "";

        private void Awake()
        {
            Debug.Assert(gameSupervisor == null, "Error: There should only be one Game Manager in the scene!");
            gameSupervisor = this;
        }

        void saveGame()
        {
            saveGameEvent?.Invoke();
        }

        void ChangeScenes(string scene)
        {
            sceneChangingEvent?.Invoke();
            SceneManager.LoadScene(scene);
        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}