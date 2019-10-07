using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using GoogleARCore.Examples.Common;

public class UIController : MonoBehaviour {
    public Portalble.PortalbleGeneralController m_PortalbleController;

	public GameObject m_GCPAwake;
	public GameObject m_GCP;
	public GameObject m_paintButtons;
    private GameObject m_rightPanel;

    public GameObject m_startButton;
    public GameObject m_endButton;

    //bools
    private bool m_show = true;
    private bool m_GCP_show = false;
    private bool m_start_show = true;

    //planes
    private GameObject[] m_planes;

    void Start() {
        if (m_PortalbleController == null) {
            m_PortalbleController = FindObjectOfType<Portalble.PortalbleGeneralController>();
        }
    }

    public void toggleStartEnd() {
        m_start_show = !m_start_show;
        m_endButton.SetActive(!m_start_show);
        m_startButton.SetActive(m_start_show);
    }


	public void toggleGCP() {
        m_GCP_show = !m_GCP_show;

        m_GCP.SetActive(m_GCP_show);
    }


	public void hidePaintButtons() {
        switchRightPanel(null);
	}

	public void showPaintButtons() {
        switchRightPanel(m_paintButtons);
	}

    public void togglePlaneMesh() {
        GlobalStates.isGridVisible = !GlobalStates.isGridVisible;

        m_planes = GameObject.FindGameObjectsWithTag("PlaneGeneratedByARCore");
        Debug.Log("toggled plane " + m_planes.Length);

        foreach (GameObject plane in m_planes) {
            if (plane != null) {
                /* update commented session */
                // DetectedPlaneVisualizer dpv = plane.GetComponent<DetectedPlaneVisualizer>();
                //if (dpv != null) {
                //    dpv.updateVisible();
                //}
            }
        }

        WorldGridDraw wgd = Camera.main.GetComponent<WorldGridDraw>();
        if (wgd != null)
            wgd.SetLineDrawEnable(GlobalStates.isGridVisible);
    }

    /*
    public void updatePlaneMeshVisibility(MeshRenderer newRenderer) {

        m_planes = GameObject.FindGameObjectsWithTag("PlaneGeneratedByARCore");

        newRenderer.enabled = m_show;

        Debug.Log("plane update visibility");
    }*/

    public void CaptureScreenshot() {
        this.changeButtonActiveness(false);
        m_paintButtons.SetActive(false);
        string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        string myFileName = "Portalble" + timeStamp + ".png";

        StartCoroutine("CaptureScreenshotCoroutine", myFileName);
    }

    public void SwitchMultimodal(bool check) {
        GlobalStates.isShift = check;
    }

    private IEnumerator CaptureScreenshotCoroutine(string filename) {
        yield return new WaitForEndOfFrame();
        ScreenShoter.CaptureScreenShot(filename);
        this.changeButtonActiveness(true);
    }

    private void changeButtonActiveness(bool state) {
        m_GCPAwake.SetActive(state);
        m_GCP.SetActive(state);
        //m_paintButtons.SetActive(state);
    }

    public void toggleBrushButton() {
        // get brush state
        if (GameObject.Find("Hand_l").GetComponent<PaintManager>().isSemiTransparentBrush()) {
            m_paintButtons.transform.Find("brushButton").GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
        else {
            m_paintButtons.transform.Find("brushButton").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    /**
     * Call to load a right panel
     */
    public void switchRightPanel(GameObject panel) {
        if (m_rightPanel != null) {
            m_rightPanel.SetActive(false);
        }
        if (m_rightPanel == panel) {
            m_rightPanel = null;
            return;
        }
        m_rightPanel = panel;
        if (m_rightPanel != null) {
            m_rightPanel.SetActive(true);
        }
    }

    public void simpleSwitchButton(Image button) {
        if (button.color == Color.white) {
            button.color = Color.grey;
        }
        else {
            button.color = Color.white;
        }
    }

    public void ToggleARPlaneVisibility() {
        if (m_PortalbleController != null) {
            m_PortalbleController.planeVisibility = !m_PortalbleController.planeVisibility;
        }
    }

    public void ToggleVibration() {
        if (m_PortalbleController != null) {
            m_PortalbleController.UseVibration = !m_PortalbleController.UseVibration;
        }
    }

    public void ToggleGrabHighLight() {
        if (m_PortalbleController != null) {
            m_PortalbleController.GrabHighLight = !m_PortalbleController.GrabHighLight;
        }
    }

    public void ToggleHandAction() {
        if (m_PortalbleController != null) {
            m_PortalbleController.HandActionRecogEnabled = !m_PortalbleController.HandActionRecogEnabled;
        }
    }
}
