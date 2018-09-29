using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoryCharachterGameSupervisor : MonoBehaviour {

    [Serializable]
    public struct StoryCharacterLoader
    {
        public string ID;
        public string name;
        public Sprite sprite;
    }
    
    public struct StoryCharacter
    {
        public string name;
        public Sprite sprite;
    }

    [SerializeField]
    List<StoryCharacterLoader> StoryCharachtersList = new List<StoryCharacterLoader>();

    public static Dictionary<string, StoryCharacter> storyCharacters = new Dictionary<string, StoryCharacter>();

    /// <summary>
    /// Use this to do psuedo serialization because unity can't properly support direct fucking dictionary serialization, and I can't fix unities classes because I can't edit the base classes and don't want to build my own engine to get around this fucking mess.
    /// </summary>
    private void listToDictionary()
    {
        foreach (StoryCharacterLoader storyCharacterLoader in StoryCharachtersList)
        {
            StoryCharacter storyCharacter = new StoryCharacter();
            storyCharacter.name = storyCharacterLoader.name;
            storyCharacter.sprite = storyCharacterLoader.sprite;
            storyCharacters.Add(storyCharacterLoader.ID, storyCharacter);
        }
    }

    private void Awake()
    {
        listToDictionary();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
