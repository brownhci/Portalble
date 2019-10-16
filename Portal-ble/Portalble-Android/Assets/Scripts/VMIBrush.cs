using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VMIBrush : VirtualMenuItem {
    public int leftMaterialID = 0;
    public int rightMaterialID = -1;
    public Material targetMat = null;
    private bool itemAvailable = false;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        int tmpLeftID = -1;

        // Set some initializing work
        if (leftMaterialID < 0 && targetMat != null) {
            // It means insert new material
            if (hand_l != null) {
                PaintManager pm = hand_l.GetComponent<PaintManager>();
                if (pm != null) {
                    tmpLeftID = pm.AddALineMaterial(targetMat);
                }
            }
        } else {
            Debug.LogError("A brush virtual menu item has invalid left hand parameters");
            return;
        }

        // Check right hand
        if (rightMaterialID < 0) {
            // Mean automatic right hand material
            // If left hand is available, use the same ID
            if (leftMaterialID >= 0) {
                rightMaterialID = leftMaterialID;
            }
            else if (targetMat != null) {
                if (hand_r != null) {
                    PaintManager pm = hand_r.GetComponent<PaintManager>();
                    if (pm != null) {
                        rightMaterialID = pm.AddALineMaterial(targetMat);
                    }
                }
            }
            else {
                // Error
                Debug.LogError("A brush virtual menu item has invalid right hand parameters");
                return;
            }
        }

        if (leftMaterialID < 0 && tmpLeftID >= 0) {
            leftMaterialID = tmpLeftID;
        }

        itemAvailable = true;
	}

    public override void onHandGrab() {
        Debug.Log("stroke material is grabbed");
        //base.onHandGrab();
        if (itemAvailable) {
            if (hand_l != null) {
                hand_l.GetComponent<PaintManager>().SetCurrentMaterial(leftMaterialID);
            }
            if (hand_r != null) {
                hand_r.GetComponent<PaintManager>().SetCurrentMaterial(rightMaterialID);
            }

            hand_l.GetComponent<HandManager>().contextSwitch("paint");
            hand_r.GetComponent<HandManager>().contextSwitch("paint");
        }

       
        m_menu.close();
    }
}
