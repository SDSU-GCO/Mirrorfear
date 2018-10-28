using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpriteScriptableObject", menuName = "Create/ScriptableObject/CharacterSpriteScriptableObject")]
public class CharacterSpriteScriptableObject : ScriptableObject {

    [System.Serializable]
    public struct CharecterSprite
    {
        public string id;
        public Sprite sprite;
    }

    public string characterId;
    public string characterName;
    public Sprite characterPortrait;
    [SerializeField]
    private CharecterSprite[] standingCharacterSprites;
    
    public Dictionary<string, Sprite> standingSprites = new Dictionary<string, Sprite>();
    
    private void Awake()
    {
        foreach(CharecterSprite charecterSprite in standingCharacterSprites)
        {
            standingSprites.Add(charecterSprite.id, charecterSprite.sprite);
        }
    }

}
