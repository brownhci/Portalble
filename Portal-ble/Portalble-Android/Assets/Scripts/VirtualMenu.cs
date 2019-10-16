using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMenu : MonoBehaviour {

	public float m_width;
	public float m_height;
	public float m_depth;

	public int m_rows;
	public int m_cols;
	private int m_page = 0;

	private List<GameObject> m_body = new List<GameObject>();
	private List<GameObject> m_footer = new List<GameObject>();

	private bool m_opened = false;

	private Vector3 m_goalPos;
	private Quaternion m_goalRot;

	private Vector3 m_vel = new Vector3 (0, 0, 0);
	private Vector3 m_acc = new Vector3 (0, 0, 0);

	private Quaternion m_rvel = new Quaternion();
	private Quaternion m_racc = new Quaternion();

	private float m_elapsed = 0.0f;

	protected virtual void Start () {
		
	}
	
	// Update transform relative to camera
	protected virtual void Update () {
		/*
		Camera cam = Camera.main;

		if (m_elapsed > 0.1f) {
			m_goalPos = cam.transform.position + cam.transform.rotation * new Vector3 (-m_width / 2, m_height / 2, -m_depth);
			m_goalRot = cam.transform.rotation;
			m_elapsed = 0.0f;
		}

		transform.position = Vector3.Lerp (transform.position, m_goalPos, m_elapsed * 10);
		transform.rotation = Quaternion.Slerp (transform.rotation, m_goalRot, m_elapsed * 10);

		m_elapsed += Time.fixedDeltaTime;
		*/
	}

	public virtual void setToCameraPosition() {
		Camera cam = Camera.main;

		transform.position = cam.transform.position + cam.transform.rotation * new Vector3 (-m_width / 2, m_height / 2, -m_depth);
		transform.rotation = cam.transform.rotation;
	}

	public void addBodyItem(GameObject item) {
		m_body.Add(item);
		item.transform.SetParent (this.transform);

		if (m_opened) {
			updateBody ();
		}
	}

	public void addFooterItem(GameObject item) {
		m_footer.Add(item);
		item.transform.SetParent (this.transform);

		if (m_opened) {
			updateFooter ();
		}
	}

	public void removeBodyItem(GameObject item) {
		m_body.Remove(item);
		Destroy (item);

		if (m_opened) {
			updateBody ();
		}
	}

	public void removeFooterItem(GameObject item) {
		m_footer.Remove(item);
		Destroy (item);

		if (m_opened) {
			updateFooter ();
		}
	}

	public void clearBody() {
		foreach(GameObject item in m_body) {
			Destroy (item);
		}

		m_body.Clear();

		if (m_opened) {
			updateBody ();
		}
	}

	public void clearFooter() {
		foreach(GameObject item in m_footer) {
			Destroy (item);
		}

		m_footer.Clear();

		if (m_opened) {
			updateFooter ();
		}
	}

	public void setPage(int page) {
		m_page = page;
		//Debug.Log ("Set page");

		if (m_opened) {
			updateBody ();
			updateFooter ();
		}
	}

	public void nextPage() {
		setPage(m_page + 1);
	}

	public void prevPage() {
		setPage(m_page - 1);
	}

    public virtual void open() {
        m_opened = true;
        Debug.Log("Virtual Menu Opened");

        setToCameraPosition();
		updateBody();
		updateFooter();
	}

	public virtual void close() {
		m_opened = false;
		Debug.Log ("Virtual Menu Closed");

		foreach (GameObject o in m_body) {
			o.SetActive(false);
		}

		foreach (GameObject o in m_footer) {
			o.SetActive(false);
		}
	}

	public void updateBody() {
		// Disable all children
		foreach (GameObject o in m_body) {
			o.SetActive(false);
		}

		// Clamp page number
		int numPerPage = m_rows * m_cols;
		m_page = Mathf.Clamp(m_page, 0, m_body.Count / numPerPage);

		// Get range to make active
		int startIndex = numPerPage * m_page;
		int endIndex = Mathf.Min(m_body.Count, numPerPage * (m_page + 1));

		// Set positions of active elements
		float deltaX = m_width / m_cols;
		float deltaY = -m_height / m_rows;

		float startX = deltaX / 2;
		float startY = 0;

		//Debug.Log ("Updating menu");
		//Debug.Log (m_body.Count);
		//Debug.Log (m_page);
		//Debug.Log (startIndex);
		//Debug.Log (endIndex);

		for (int i = startIndex; i < endIndex; i++) {
			int col = (i - startIndex) % m_cols;
			int row = (i - startIndex) / m_cols;

			float xPos = startX + deltaX * col;
			float yPos = startY + deltaY * row;

			m_body [i].SetActive (true);
			m_body [i].transform.SetParent (this.transform);
			m_body [i].transform.localPosition = new Vector3(xPos, yPos, 0);
		}	
	}

	public void updateFooter() {
		// Enable all children
		foreach (GameObject o in m_footer) {
			o.SetActive(true);
		}

		// Set positions of elements
		float deltaX = m_width / m_footer.Count;
		float startX = deltaX / 2;

		for (int i = 0; i < m_footer.Count; i++) {
			float xPos = startX + deltaX * i;

			m_footer [i].SetActive (true);
			m_footer [i].transform.SetParent (this.transform);
			m_footer [i].transform.localPosition = new Vector3(xPos, -m_height, 0);
		}
	}

	public void setOpenTrue(){
		m_opened = true;
	}

	public void setOpenFalse(){
		m_opened = false;
	}

	//for 3D color picker. similar to addBodyItem but not added to m_body
	public void addToParent(GameObject item){
		item.transform.SetParent (this.transform);
	}

    public bool isOpened() {
        return m_opened;
    }
}
