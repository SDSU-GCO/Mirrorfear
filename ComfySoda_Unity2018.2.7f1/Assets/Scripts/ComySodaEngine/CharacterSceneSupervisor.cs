using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSceneSupervisor : MonoBehaviour {

    class CharacterSpriteController
    {
        public string emotion;
        public CharacterSpriteScriptableObject characterSpriteScriptableObject;
        public SpriteRenderer SpriteRenderer;

        public GameObject gameObject = null;
        
        public bool faceLeft
        {
            set
            {
                if(value != SpriteRenderer.flipY)
                {
                    if(value==true)
                    {
                        SpriteRenderer.flipY = true;
                    }
                    else
                    {
                        SpriteRenderer.flipY = false;
                    }
                }
            }
            get
            {
                return SpriteRenderer.flipY;
            }
        }
    }
    
    Dictionary<string, CharacterSpriteController> characterSpriteControllers = new Dictionary<string, CharacterSpriteController>();

    bool AddCharacter(string name, CharacterSpriteController characterSpriteController)
    {
        bool rtnVal = false;
        if(characterSpriteControllers.ContainsKey(name)==false)
        {
            characterSpriteController.SpriteRenderer = characterSpriteController.gameObject.AddComponent<SpriteRenderer>();
            characterSpriteControllers.Add(name, characterSpriteController);
            rtnVal = true;
        }
        return rtnVal;
    }

    bool RemoveCharacter(string name)
    {
        bool rtnVal = false;
        if (characterSpriteControllers.ContainsKey(name) == true)
        {
            characterSpriteControllers.Remove(name);
            rtnVal = true;
        }
        return rtnVal;
    }

    bool MoveCharacter(string name, Vector2 vector2)
    {
        bool rtnVal = false;
        CharacterSpriteController characterSpriteController = null;
        if(characterSpriteControllers.TryGetValue(name, out characterSpriteController))
        {
            rtnVal = true;
            characterSpriteController.gameObject.transform.position = new Vector3(vector2.x, vector2.y, characterSpriteController.gameObject.transform.position.z);
        }
        return rtnVal;
    }
        
}
