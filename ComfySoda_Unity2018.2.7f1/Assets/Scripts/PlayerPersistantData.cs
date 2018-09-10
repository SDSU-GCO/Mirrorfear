using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{
    [CreateAssetMenu(fileName = "PlayerPersistantDataScriptableObject", menuName = "Create/ScriptableObject/PersistantDataContainers/Player")]
    public class PlayerPersistantData : ScriptableObject
    {
        public Vector2? playerPosition = null;
    }
}
