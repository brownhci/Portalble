using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapDarts : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.name.Contains ("PD")) {
			other.GetComponent<Rigidbody> ().useGravity = false;
			other.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			other.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}
	}
}
