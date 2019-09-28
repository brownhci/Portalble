using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPaintMenu : VirtualMenu {
	//public Color[] m_colors;
	//public GameObject m_itemPrefab;
	//public GameObject m_itemPrefabPre;
	//public GameObject m_itemPrefabNxt;
	//public GameObject m_itemPrefabCls;

	//3D color picker
	public GameObject prefab;
	public int gridResolution = 8;

    public float gridSize = 3f;

	private GameObject[] grid;

	// Use this for initialization
	protected override void Start () {
		base.Start ();

		// Add body
		//foreach (Color c in m_colors) {
		//	GameObject item = Instantiate (m_itemPrefab) as GameObject;

		//	item.AddComponent<VMIColor> ();
		//	item.GetComponent<VMIColor> ().m_color = c;

		//	item.SetActive (false);
		//	addBodyItem (item);
		//}

		// Add footer
		//GameObject prevPage = Instantiate (m_itemPrefabPre) as GameObject;
		//prevPage.AddComponent<VMIPrevPage> ();
		//prevPage.transform.Rotate (new Vector3 (0, 90, 0));
		//addFooterItem (prevPage);

		//GameObject nextPage = Instantiate (m_itemPrefabNxt) as GameObject;
		//nextPage.AddComponent<VMINextPage> ();
		//nextPage.transform.Rotate (new Vector3 (0, 90, 0));
		//addFooterItem (nextPage);

		//GameObject closeMenu = Instantiate (m_itemPrefabCls) as GameObject;
		//closeMenu.AddComponent<VMIClose> ();
		//closeMenu.transform.Rotate (new Vector3 (0, 90, 0));
		//addFooterItem (closeMenu);

		grid = new GameObject[gridResolution * gridResolution * gridResolution];
		for (int i = 0, z = 0; z < gridResolution; z++) {
			for (int y = 0; y < gridResolution; y++) {
				for (int x = 0; x < gridResolution; x++, i++) {
					grid[i] = CreateGridPoint(x, y, z);
                    if (!isOpened()) {
                        grid[i].SetActive(false);
                    }
				}
			}
		}
	}

	public override void open(){
		
		this.setToCameraPosition ();
		base.setOpenTrue ();
        if (grid != null) {
            for (int i = 0, z = 0; z < gridResolution; z++) {
                for (int y = 0; y < gridResolution; y++) {
                    for (int x = 0; x < gridResolution; x++, i++) {
                        grid[i].SetActive(true);
                    }
                }
            }
        }
		Debug.Log ("3D matrix opened");
	}

	public override void close() {

		base.setOpenFalse ();
        if (grid != null) {
            for (int i = 0, z = 0; z < gridResolution; z++) {
                for (int y = 0; y < gridResolution; y++) {
                    for (int x = 0; x < gridResolution; x++, i++) {
                        grid[i].SetActive(false);
                    }
                }
            }
        }

		//turn off color matrix but doesn't quit paint mode
		Debug.Log ("3D matrix closed");
	}
		

	private GameObject CreateGridPoint (int x, int y, int z) {
		GameObject point = Instantiate (prefab) as GameObject;
        base.addToParent(point);
        point.transform.localPosition = GetCoordinates(x, y, z);
		Color newColor = new Color(
			(float)x / gridResolution,
			(float)y / gridResolution,
			(float)z / gridResolution
		);

		//point.GetComponent<Renderer> ().material.color = newColor;

		point.AddComponent<VMIColor> ();
		point.GetComponent<VMIColor> ().m_color = newColor;

		point.SetActive (false);

		return point;
	}

	private Vector3 GetCoordinates (int x, int y, int z) {
        /*
		return new Vector3(
			x/3f - (gridResolution - 1) * 0.5f,
			y/3f - (gridResolution - 1) * 0.5f,
			z/3f - (gridResolution - 1) * 0.5f
		);*/
        float cubeLength = gridSize / gridResolution;
        float gridResoF = (float)gridResolution;
        return new Vector3(
            x * cubeLength - gridSize / 2.0f,
            y * cubeLength - gridSize / 2.0f,
            z * cubeLength - gridSize / 2.0f
            );
	}

	public override void setToCameraPosition() {
		Camera cam = Camera.main;

        //transform.position = cam.transform.position + cam.transform.forward * 0.5f;
		//transform.position = cam.transform.position + cam.transform.rotation * new Vector3 (3f, 2f, 2f);
		//transform.rotation = cam.transform.rotation;

        if (cam != null) {
            Vector3 pos = cam.transform.forward;
            pos.y = 0;
            pos = pos.normalized * 2.3f;
            transform.position = pos;
            // orientation
            Quaternion q = Quaternion.FromToRotation(transform.forward, pos.normalized);
            transform.rotation = q * transform.rotation;
        }
	}

}
