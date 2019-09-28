using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeTest : MonoBehaviour {
    private Material originMaterial;
    public Material newMaterial;

	// Use this for initialization
	void Start () {
        originMaterial = GetComponent<Renderer>().material;
		if (newMaterial != null) {
            newMaterial.mainTexture = originMaterial.mainTexture;
            newMaterial.color = originMaterial.color;
            newMaterial.SetColor("_OutlineColor", Color.blue);
            GetComponent<Renderer>().material = newMaterial;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
