using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMenuItem : MonoBehaviour {

	protected VirtualMenu m_menu = null;

	protected GameObject hand_l, thumb_l, indexfinger_l, middlefinger_l, ringfinger_l, palm_l;
    protected GameObject hand_r, thumb_r, indexfinger_r, middlefinger_r, ringfinger_r, palm_r;
	protected bool m_collidingWithHand = false;

	private Shader[] m_primaryShader;
	private Shader m_secondaryShader;
	private Color m_color;
	private string m_lastLGesture = "";
    private string m_lastRGesture = "";

    private int m_handInFlag = 0x0;

	// Use this for initialization
	protected virtual void Start () {
		// Get shaders
		if (this.GetComponent<Renderer> () != null) {
			m_primaryShader = new Shader[1];
			m_primaryShader[0] = this.GetComponent<Renderer> ().material.shader;
            if (GetComponent<Renderer>().material.HasProperty("_Color"))
                m_color = GetComponent<Renderer>().material.color;
            else
                m_color = Color.white;
		} else {
			m_primaryShader = new Shader[transform.childCount];

			int i = 0;
			foreach (Transform child in transform) {
				m_primaryShader [i] = child.GetComponent<Renderer> ().material.shader;
				i++;
			}
		}

		m_secondaryShader = Shader.Find ("ModelEffect/VerticsOutline_Always");

		// Get hand objects
		hand_l = GameObject.Find ("Hand_l").gameObject;

		thumb_l = hand_l.transform.GetChild (0).gameObject;
		indexfinger_l = hand_l.transform.GetChild (1).gameObject;
		middlefinger_l = hand_l.transform.GetChild (2).gameObject;
		ringfinger_l = hand_l.transform.GetChild (3).gameObject;
		palm_l = hand_l.transform.GetChild (5).gameObject;

        hand_r = GameObject.Find("Hand_r").gameObject;

        thumb_r = hand_r.transform.GetChild(0).gameObject;
        indexfinger_r = hand_r.transform.GetChild(1).gameObject;
        middlefinger_r = hand_r.transform.GetChild(2).gameObject;
        ringfinger_r = hand_r.transform.GetChild(3).gameObject;
        palm_r = hand_r.transform.GetChild(5).gameObject;
    }
	
	// Update is called once per frame
	protected virtual void Update () {
		if (this.transform.parent != null) {
			m_menu = this.transform.parent.gameObject.GetComponent<VirtualMenu>();
		}

		if (m_menu != null) {
			// Detect collision
			if (!m_collidingWithHand) {
				m_collidingWithHand = collidesWithHand ();

				if (m_collidingWithHand) {
					onHandIn ();
				}
			} else {
				m_collidingWithHand = collidesWithHand ();

				if (!m_collidingWithHand) {
					onHandOut ();
				}
			}

			// Detect grab
			if (m_collidingWithHand) {
                if ((m_handInFlag & 0x1) != 0) {
                    // Left Hand in
                    string curLGesture = hand_l.GetComponent<GestureControl>().bufferedGesture();

                    if (curLGesture != m_lastLGesture && curLGesture == "pinch") {
                        onHandGrab();
                    }
                }
                else if ((m_handInFlag & 0x2) != 0) {
                    // Right Hand in
                    string curRGesture = hand_r.GetComponent<GestureControl>().bufferedGesture();
                    if (curRGesture != m_lastRGesture && curRGesture == "pinch") {
                        onHandGrab();
                    }
                }
			}

			// Reset last gesture
			m_lastLGesture = hand_l.GetComponent<GestureControl> ().bufferedGesture ();
            m_lastRGesture = hand_r.GetComponent<GestureControl>().bufferedGesture ();
		}
	}

	bool collidesWithHand() {
        bool ret = false;
        if (collidesWithFinger(thumb_l) ||
            collidesWithFinger(indexfinger_l) ||
            collidesWithFinger(middlefinger_l) ||
            collidesWithFinger(ringfinger_l) ||
            collidesWithObject(palm_l)) {
            m_handInFlag |= 0x1;
            ret = true;
        } else {
            m_handInFlag &= 0x2;
        }
        if(
            collidesWithFinger(thumb_r) ||
            collidesWithFinger(indexfinger_r) ||
            collidesWithFinger(middlefinger_r) ||
            collidesWithFinger(ringfinger_r) ||
            collidesWithObject(palm_r)) {
            m_handInFlag |= 0x2;
            ret = true;
        } else {
            m_handInFlag &= 0x1;
        }

        return ret;
    }

	bool collidesWithFinger(GameObject finger) {
		for (int i = 0; i < 3; i++) {
			//Debug.Log (finger);
			//Debug.Log (finger.transform.GetChild (i));

			if (collidesWithObject(finger.transform.GetChild (i).gameObject)) {
				return true;
			}
		}

		return false;
	}

	bool collidesWithObject(GameObject o) {
		if (this.GetComponent<Collider> () != null) {
			return objectCollidesWithObject (this.gameObject, o);
		} else {
			foreach (Transform child in transform) {
				if (objectCollidesWithObject (child.gameObject, o)) {
					return true;
				}
			}

			return false;
		}
	}

	bool objectCollidesWithObject(GameObject o1, GameObject o2) {
		Collider c = o1.GetComponent<Collider>();
		return c.bounds.Intersects (o2.GetComponent<Collider> ().bounds);
	}

	private void highlightMaterial(GameObject g) {
		if (g.GetComponent<Renderer> () != null) {
			g.GetComponent<Renderer> ().material.shader = m_secondaryShader;
			g.GetComponent<Renderer> ().material.SetColor ("_OutlineColor", new Color (0, 248.0f / 256.0f, 63.0f / 256.0f, 143.0f / 256.0f));
			g.GetComponent<Renderer> ().material.SetFloat ("_OutlineWidth", 0.001f);
		}
	}

	private void unhighlightMaterial(GameObject g, int i) {
		if (g.GetComponent<Renderer> () != null) {
			g.GetComponent<Renderer> ().material.shader = m_primaryShader[i];
			g.GetComponent<Renderer> ().material.color = m_color;
		}
	}

	public virtual void onHandIn() {
        Debug.Log("Hand IN");
		highlightMaterial (this.gameObject);

		foreach (Transform child in transform) {
			highlightMaterial (child.gameObject);
		}
	}

	public virtual void onHandOut() {
		if (this.GetComponent<Renderer> () != null) {
			unhighlightMaterial (this.gameObject, 0);
		} else {
			int i = 0;
			foreach (Transform child in transform) {
				unhighlightMaterial (child.gameObject, i);
				i++;
			}
		}
	}

	public virtual void onHandGrab() {

	}
}
