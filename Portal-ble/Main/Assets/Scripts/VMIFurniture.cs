using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VMIFurniture : VirtualMenuItem {
	public GameObject m_furniturePrefab;
	
	public override void onHandGrab ()
	{
		Camera cam = Camera.main;

		// Add object
		GameObject fo = Instantiate(m_furniturePrefab,
			cam.transform.position + cam.transform.forward,
			Quaternion.LookRotation(cam.transform.forward, new Vector3(0, 1, 0))) as GameObject;


		Debug.Log ("Creating new object");
		fo.tag = "InteractableObj";


		Debug.Log ("Adding collider");

		if (fo.GetComponent<Renderer> () != null) {
			SetDepthMaterial (fo);

			BoxCollider c = fo.AddComponent<BoxCollider> ();
			c.isTrigger = false;
		} else {
			Bounds combinedBounds = new Bounds();

			foreach (Transform child in transform) {
				SetDepthMaterial (child.gameObject);

				BoxCollider cc = child.gameObject.AddComponent<BoxCollider> ();
				cc.isTrigger = false;
				combinedBounds.Encapsulate (cc.bounds);
			}

			BoxCollider c = fo.AddComponent<BoxCollider> ();
			c.isTrigger = false;
			c.size = combinedBounds.size;
		}

		Debug.Log ("Adding interaction script");

		InteractionScriptObject iso = fo.AddComponent<InteractionScriptObject> ();
		iso.secondaryMaterial = new Material(Shader.Find ("ModelEffect/VerticsOutline_Always"));
		iso.secondaryMaterial.SetColor ("_OutlineColor", new Color (0, 248.0f / 256.0f, 63.0f / 256.0f, 143.0f / 256.0f));

		Debug.Log ("Adding rigid body");

		Rigidbody r = fo.AddComponent<Rigidbody> ();
		r.collisionDetectionMode = CollisionDetectionMode.Discrete;
		r.mass = 20;
		r.useGravity = true;
		r.velocity = Vector3.zero;
		r.angularVelocity = Vector3.zero;

		// Change context
		hand_l.GetComponent<HandManager> ().contextSwitch ("object");
	}

	private void SetDepthMaterial(GameObject o) {
		o.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/ZTestBlur");
		o.GetComponent<Renderer> ().material.SetTexture ("_CameraDepthTexture", GameObject.Find ("DepthCamera").GetComponent<Camera> ().targetTexture);

		if(m_furniturePrefab.GetComponent<Renderer>() != null) {
			o.GetComponent<Renderer> ().material.color = m_furniturePrefab.GetComponent<Renderer> ().material.color;
		}
	}
}
