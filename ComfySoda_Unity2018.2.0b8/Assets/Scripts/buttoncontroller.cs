using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttoncontroller : MonoBehaviour {

    Button button = null;
	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();

    }
    private void OnEnable()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update () {

	}
}
