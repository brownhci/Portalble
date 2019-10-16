using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectionScript : MonoBehaviour {
	public GameObject thumb;
	public GameObject thumb1;
	public GameObject indexfinger;
	public GameObject middlefinger;
	public GameObject ringfinger;
	public GameObject palm;
	public GameObject thumb_r;
	public GameObject thumb1_r;
	public GameObject indexfinger_r;

	public GameObject idxfinger_r;
	public GameObject thmb_r;
	public GameObject midfinger_r;

	public GameObject middlefinger_r;
	public GameObject ringfinger_r;
	public GameObject palm_r;
	public Material mat;
	public Color color;
	public Queue<Vector3> speedList = new Queue<Vector3>();
	public GameObject Hand_L, Hand_R;
	bool grabbed = false;
	static float restoreColliderTimer;
	//public Camera cam;
	// Use this for initialization
	void Start () {
		float dist_thumb_fore = Vector3.Distance(thumb.transform.position, indexfinger.transform.position);
		Debug.Log (dist_thumb_fore);
		//0.21 finger to palm
		//this.GetComponent<Material> ().SetColor("_SpecColor", Color.green);
		for (int i=0; i < 10; i++)
			speedList.Enqueue(new Vector3(0,0,0));
	}

	//Simplified version of code


	// Update is called once per frame
	void Update () {
		//Grab Detection: 

		//With rigidbody
		Collider c = this.GetComponent<Collider>();
		float dist_palm_index = Vector3.Distance(palm.transform.position, indexfinger.transform.position);
		float dist_palm_middle = Vector3.Distance(palm.transform.position, middlefinger.transform.position);
		float dist_palm_ring = Vector3.Distance(palm.transform.position, ringfinger.transform.position);
		float dist_palm_thumb = Vector3.Distance(palm.transform.position, thumb.transform.position);
		float dist_thumb_index = Vector3.Distance(thumb.transform.position, indexfinger.transform.position);
		float dist_thumb_index_r = Vector3.Distance(thumb_r.transform.position, indexfinger_r.transform.position);
		//Method 2
		/*
		if (c.bounds.Contains (palm.transform.position) && (dist_palm_index < 0.05 || dist_palm_middle < 0.05 || dist_palm_ring < 0.05 || dist_palm_thumb < 0.05)) {
			//Debug.Log (palm.transform.position);
			Debug.Log ("point is inside collider");
			this.transform.parent = palm.transform;
		} else if ((c.bounds.Contains (thumb.transform.position) || c.bounds.Contains (indexfinger.transform.position)) && (dist_thumb_index < 0.08 || dist_palm_index < 0.08 || dist_palm_ring < 0.08 || dist_palm_middle < 0.08)) {
			Debug.Log ("indexfinger or thumb is inside collider");
			this.transform.parent = palm.transform;
		} else if (this.transform.position.y > (-0.09+c.bounds.extents.y)){
			float grav = -0.005f;
			this.transform.Translate (0,grav,0,Space.World);
			//this.transform.rotation = Quaternion.identity;
			this.transform.parent = null;
		} 
		else{
			//float grav = -0.01f;
			//this.transform.Translate (0,grav,0);
			this.transform.rotation = Quaternion.identity;
			this.transform.parent = null;
		}
		*/
		//Method 3: The center of fingers enter objects
		/*
		if (c.bounds.Contains (thumb.transform.position) && (c.bounds.Contains (indexfinger.transform.position) || c.bounds.Contains (middlefinger.transform.position) || c.bounds.Contains (ringfinger.transform.position))) {
			this.transform.parent = palm.transform;
		} else if (c.bounds.Contains (thumb_r.transform.position) && (c.bounds.Contains (indexfinger_r.transform.position) || c.bounds.Contains (middlefinger_r.transform.position) || c.bounds.Contains (ringfinger_r.transform.position))) {
			this.transform.parent = palm_r.transform;
		} 
		else if (this.transform.position.y > (-0.09+c.bounds.extents.y)){
			float grav = -0.005f;
			this.transform.Translate (0,grav,0,Space.World);
			this.transform.parent = null;
		}  
		else {
			this.transform.rotation = Quaternion.identity;
			this.transform.parent = null;
		}
		*/

		//Method 3(w/o rigidBoy of object): The fingers' collider interesect with the object
		float grav = -0.01f;
		float factor = 2f;
		float v_x = 0;
		float v_y = grav;
		float v_z = 0;

		//use vector to help detect grab
		Vector3 index_thumb1 = indexfinger.transform.position - thumb1.transform.position;
		Vector3 thumb_thumb1 = thumb.transform.position - thumb1.transform.position;
		float angle = Vector3.Angle(index_thumb1, thumb_thumb1);
		Vector3 index_thumb1_r = indexfinger_r.transform.position - thumb1_r.transform.position;
		Vector3 thumb_thumb1_r = thumb_r.transform.position - thumb1_r.transform.position;
		float angle_r = Vector3.Angle(index_thumb1_r, thumb_thumb1_r);
		Vector3 Velocity = new Vector3(0,0,0);
		//Debug.Log (angle + ", " + angle_r);
		if ((angle < 20 || (angle > 30 && angle < 45) || dist_thumb_index < 0.085) &&
		     c.bounds.Intersects (thumb.GetComponent<Collider> ().bounds) &&
		     (c.bounds.Intersects (indexfinger.GetComponent<Collider> ().bounds) ||
		     c.bounds.Intersects (middlefinger.GetComponent<Collider> ().bounds) ||
		     c.bounds.Intersects (ringfinger.GetComponent<Collider> ().bounds))) {
			
			Debug.Log (dist_thumb_index);
			this.transform.parent = palm.transform;
			Rigidbody palm_rb = palm.GetComponent<Rigidbody> ();
			Vector3 initial_v = new Vector3 ();
			initial_v = palm_rb.velocity;
			//v_x = factor*initial_v.x;
			//v_z = factor*initial_v.z;
			speedList.Dequeue ();
			speedList.Enqueue (initial_v);
			Velocity = initial_v;
			grabbed = true;
			Debug.Log (Time.time);
			//disableCollider (indexfinger);
			Debug.Log("1");
			restoreColliderTimer = Time.time;

		} else if (dist_thumb_index_r < 0.085 &&
		           (c.bounds.Intersects (thumb_r.GetComponent<Collider> ().bounds) || c.bounds.Contains (thumb_r.transform.position)) &&
		           ((c.bounds.Contains (indexfinger_r.transform.position) ||
		           c.bounds.Contains (middlefinger_r.transform.position) ||
		           c.bounds.Contains (ringfinger_r.transform.position)) ||

		           (c.bounds.Intersects (indexfinger_r.GetComponent<Collider> ().bounds) ||
		           c.bounds.Intersects (middlefinger_r.GetComponent<Collider> ().bounds) ||
		           c.bounds.Intersects (ringfinger_r.GetComponent<Collider> ().bounds)))) {
			this.GetComponent<Rigidbody> ().isKinematic = true;
			this.transform.parent = palm_r.transform;
			Rigidbody palm_rb = palm_r.GetComponent<Rigidbody> ();
			Vector3 initial_v = new Vector3 ();
			initial_v = palm_rb.velocity;
			speedList.Dequeue ();
			speedList.Enqueue (initial_v);
			Velocity = initial_v;
			grabbed = true;
			for (int i = 0; i < 2; i++) {
				idxfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
				//Debug.Log (idxfinger_r.transform.GetChild (i).name);
				thmb_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
				midfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
				palm_r.GetComponent<Collider> ().isTrigger = true;
			}
			Debug.Log("2");
			restoreColliderTimer = Time.time;

		} else if (grabbed == true) {
			grabbed = false;
			this.transform.parent = null;
			this.GetComponent<Rigidbody> ().isKinematic = false;
			int num_speed = speedList.Count;
			Vector3 average = new Vector3 (0, 0, 0);
			for (int i = 0; i < 10; i++) {
				average += speedList.Dequeue ();
				speedList.Enqueue (new Vector3 (0, 0, 0));
			}
			average = 0.8f * (average / num_speed);
			this.GetComponent<Rigidbody> ().velocity = average;
			restoreColliderTimer = Time.time;
			//Debug.Log (this.GetComponent<Rigidbody> ().velocity);
			Debug.Log("3");

		} else {
			Debug.Log("4");
			float diff = Time.time - restoreColliderTimer;
			if (diff > 1.5f) {
				Debug.Log("current restoreTime = " + restoreColliderTimer);
				Debug.Log("Difference : " + diff);
				for (int i = 0; i < 2; i++) {
					idxfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = false;
					thmb_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = false;
					midfinger_r.transform.GetChild (i).GetComponent<Collider> ().isTrigger = false;
					palm_r.GetComponent<Collider> ().isTrigger = false;
				}
			}
		}
		//Debug.Log (grabbed);
		//Method 3(w/ rigidBoy of object): The fingers' collider interesect with the object

		//Hover feature
		float dist_obj_palm = Vector3.Distance(this.transform.position, palm.transform.position);
		float dist_obj_palm_r = Vector3.Distance(this.transform.position, palm_r.transform.position);
		float threshold = 0.25f;
		if (dist_obj_palm < threshold || dist_obj_palm_r < threshold) {
			this.GetComponent<Renderer> ().material.SetFloat ("_Metallic", (threshold - Mathf.Min (dist_obj_palm, dist_obj_palm_r)) / threshold);
		}
		else{
			this.GetComponent<Renderer> ().material.SetFloat ("_Metallic", 0);
		}
	}
	private void disableCollider(GameObject finger){
		for (int i = 0; i < 2; i++) {
			finger.transform.GetChild (i).GetComponent<Collider> ().isTrigger = true;
			Debug.Log (finger.transform.GetChild (i).name);
		}
		return;
	}

	private void enableCollider(GameObject finger){
		for (int i=0; i<2; i++)
			finger.transform.GetChild (i).GetComponent<Collider>().isTrigger = false;
		return;
	}

	/*
	void OnCollisionEnter(Collision collision)
	{
		//foreach (ContactPoint contact in collision.contacts)
		//{
		//	Debug.DrawRay(contact.point, contact.normal, Color.white);
		//}
		if (collision.relativeVelocity.magnitude < 5) {
			float dist_palm_index = Vector3.Distance(palm.transform.position, indexfinger.transform.position);
			float dist_palm_middle = Vector3.Distance(palm.transform.position, middlefinger.transform.position);
			float dist_palm_ring = Vector3.Distance(palm.transform.position, ringfinger.transform.position);
			float dist_palm_thumb = Vector3.Distance(palm.transform.position, thumb.transform.position);
			Collider c = this.GetComponent<Collider>();
			if (c.bounds.Contains (palm.transform.position) && (dist_palm_index < 0.05 || dist_palm_middle < 0.05 || dist_palm_ring < 0.05 || dist_palm_thumb < 0.05)) {
				//Debug.Log (palm.transform.position);
				Debug.Log ("point is inside collider");
				this.transform.parent = palm.transform;
			} else
				this.transform.parent = null;
		}
	}
	void OnCollisionExit(Collision collision){
		this.transform.parent = null;
	}
*/


}
