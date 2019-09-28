using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMenuBrush : VirtualMenu {
    public Transform[] brushes;
    //public GameObject m_itemPrefabCls;
    private List<Transform> brushChildren;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        brushChildren = new List<Transform>(brushes.Length);

        // Add brushes to menu body
        int i = 0;
        foreach (Transform brush in brushes) {
            Transform brush_temp = Instantiate(brush) as Transform;
            //brush_temp.localPosition = Vector3.zero;
            brush_temp.gameObject.SetActive(false);
            addToParent(brush_temp.gameObject);
            brushChildren.Add(brush_temp);
        }

    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public override void open() {
        base.open();
        setToCameraPosition();
        setOpenTrue();
        for(int i = 0; i < brushChildren.Count; ++i) {
            brushChildren[i].gameObject.SetActive(true);
            float xPos = 0.0f + (0.15f * i);
            float yPos = 0.0f;

            brushChildren[i].localPosition = new Vector3(xPos, yPos, 0);
        }
    }

    public override void close() {
        setOpenFalse();
        if (brushChildren != null) {
            for (int i = 0; i < brushChildren.Count; ++i) {
                brushChildren[i].gameObject.SetActive(false);
            }
        }
    }
}
