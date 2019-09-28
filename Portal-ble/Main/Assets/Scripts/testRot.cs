using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRot : MonoBehaviour {
	public GameObject gm;
	// Use this for initialization
	void Start () {
		gm.transform.Rotate (new Vector3 (0, 90, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
