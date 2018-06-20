using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace cs
{
    public static class PersistentData
    {

        public static float? xPlayerCoordinate = null;
        public static float? yPlayerCoordinate = null;
        public static GameObject playerObject = null;
        public static Text proximityText;
        public static bool firstLoadSceneOne = true;


    }

}

