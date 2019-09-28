using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand_enable : MonoBehaviour {
	private WSManager ws;
	private GameObject hand_l;
	private GameObject hand_r;
    private Sync c;
	// Use this for initialization
	void Start () {
		ws = GameObject.Find ("WebsocketManager").GetComponent<WSManager>();

		hand_l = GameObject.Find ("Hand_l").gameObject;
		hand_r = GameObject.Find ("Hand_r").gameObject;
    

	}
	
	// Update is called once per frame
	void Update () {
    
		string msg = "";
		msg = ws.getHandInfoLeft ();
		if (!msg.Equals ("") ) {
			//if (GameObject.Find ("Hand_l").gameObject.activeInHierarchy)
			hand_l.SetActive (true);
		}
			
		msg = ws.getHandInfoRight ();
		//Debug.Log (msg);
		if (!msg.Equals ("")) {
			//if (GameObject.Find ("Hand_r").gameObject.activeInHierarchy)
			hand_r.SetActive (true);
		}
	}
}
