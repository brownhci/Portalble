using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Portalble
{
    /// <summary>
    /// A base controller class. Provide some useful AR application interfaces.
    /// Like visualize scanned planes and notify when user hit a scanned plane.
    /// It's highly recommended to derive a customized AR controller from this class.
    /// 
    /// This must be changed in order to compile with IOS
    /// Some functions from this files are copied from ARCORE 1.2 - 1.8, 
    /// for integratin with IOS, please be aware of this. 
    /// 
    /// Important!!!!
    /// For future updates, make sure 
    /// 1. Add a mesh collider to the scanned meshes
    /// 2. Setup the layer to DetectedPlane
    /// </summary>
    public class PortalbleGeneralController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image
        /// </summary>
        public Camera m_FirstPersonCamera;

        /// <summary>
        /// The id of layer that all unity ARCore planes belong to.
        /// Used for interaction with unity plane. -1 for disable.
        /// </summary>
        public int m_UnityPlaneInteractionLayer;

        /// <summary>
        /// Wether enable interaction with ARCore plane (Just ARCore data, not Unity)
        /// </summary>
        public bool m_EnableARPlaneInteraction;

        /// <summary>
        /// Wether enable interaction with Unity Plane. InteractionLayer must not be negative.
        /// </summary>
        public bool m_EnableUnityPlaneInteraction;

        /// <summary>
        /// Wether enable plane generator. Plane Visualizer must be set.
        /// </summary>
        public bool m_EnablePlaneGenerator;

        /// <summary>
        /// Wether enable hand distance warning system.
        /// </summary>
        public bool m_EnableHandDistanceWarning;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes. If PlaneGenerator is disabled, this field is ignored.
        /// </summary>
        public GameObject m_DetectedPlanePrefab;

        /// <summary>
        /// AR Support class
        /// </summary>
        public PortalbleARSupport m_ARSupport;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        protected List<PortalbleARPlane> m_AllPlanes = new List<PortalbleARPlane>();

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        protected List<PortalbleARPlane> m_NewPlanes = new List<PortalbleARPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        protected bool m_IsQuitting = false;

        /// <summary>
        /// Left hand transform (might be null if controller doesn't use any information of hand transformation)
        /// </summary>
        protected Transform m_tLeftHand;

        /// <summary>
        /// Right hand transform (might be null if controller doesn't use any information of hand transformation)
        /// </summary>
        protected Transform m_tRightHand;

        /// <summary>
        /// Used for red screen distance check.
        /// </summary>
        public GameObject m_RedScreen;

        /// <summary>
        /// The instance of red screen object.
        /// </summary>
        protected GameObject m_RedScreenInstance;

        protected WSManager m_WSManager;

        // Use this for initialization
        protected virtual void Start()
        {
            // Empty
            m_UnityPlaneInteractionLayer = 0x1 << m_UnityPlaneInteractionLayer;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // _UpdateApplicationLifecycle();

            m_AllPlanes = m_ARSupport.getPlanes();

            // If Interaction with ARCore planes is available.
            if (m_EnableARPlaneInteraction)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    // if current touch is valid. (notice: here is a ! mark before the logic)
                    if (!(Input.touchCount < 1 || touch.phase != TouchPhase.Began))
                    {
                        // if hitting on a gui button, then don't do any interaction.
                        /* condition only passes when no gui is hit */
                        if (EventSystem.current == null || !(EventSystem.current.IsPointerOverGameObject()
                            || EventSystem.current.currentSelectedGameObject != null))
                        {
                            List<PortalbleHitResult> hits = new List<PortalbleHitResult>();
                            if (m_ARSupport.Raycast(touch.position, hits))
                            {
                                OnARPlaneHit(hits[0]);
                            }
                        }
                    }
                }
            }

            // If interaction with Unity planes is available
            if (m_EnableUnityPlaneInteraction && m_UnityPlaneInteractionLayer >= 0)
            {
                if (m_FirstPersonCamera)
                {
                    Touch touch;
                    if (Input.touchCount > 0)
                    {
                        // If current touch is valid. (Notice: Here is a ! mark before the logic)
                        if (!(Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began))
                        {
                            // If hitting on a gui button, then don't do any interaction.
                            if (EventSystem.current == null || !(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null))
                            {
                                Ray ray = m_FirstPersonCamera.ScreenPointToRay(touch.position);
                                RaycastHit hit;
                                if (Physics.Raycast(ray, out hit, 9999f, 0x0fffffff))
                                {
                                    if (hit.transform.GetComponent<UnityEngine.XR.ARFoundation.ARPlane>() != null)
                                        OnUnityPlaneHit(hit);
                                }
                            }
                        }
                    }
                }
            }

            //if (m_EnablePlaneGenerator) {
            //    if (Session.Status == SessionStatus.Tracking) {
            //        // Must be tracking.
            //        // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            //        Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
            //        for (int i = 0; i < m_NewPlanes.Count; i++) {
            //            // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
            //            // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
            //            // coordinates.
            //            GameObject planeObject = Instantiate(m_DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            //            planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
            //        }
            //    }
            //}

            if (m_EnableHandDistanceWarning)
            {
                // Find two hands
                UpdateHandTransform();

                bool foc = false;
                /**
                if (m_tLeftHand != null && CheckDistance(m_tLeftHand, out foc)) {
                    OnHandInappropriateDistance(foc);
                }
                else if (m_tRightHand != null && CheckDistance(m_tRightHand, out foc)) {
                    OnHandInappropriateDistance(foc);
                }**/

                // Check visualization availability..
                if (m_RedScreenInstance == null)
                {
                    m_RedScreenInstance = GameObject.Instantiate(m_RedScreen);
                }

                // Currently, only check for right hand. Cause we don't know activity of hand.
                if (m_WSManager == null)
                {
                    m_WSManager = GameObject.FindObjectOfType<WSManager>();
                }
                if (m_WSManager != null && m_RedScreenInstance != null)
                {
                    string activeHand = m_WSManager.getActiveHand();
                    bool distanceCheckResult = false;
                    if (activeHand == "NO_HAND")
                    {
                        m_RedScreenInstance.SetActive(false);
                    }
                    else if (activeHand == "LEFT_HAND")
                    {
                        distanceCheckResult = m_tLeftHand != null && CheckDistance(m_tLeftHand, out foc);
                    }
                    else
                    {
                        distanceCheckResult = m_tRightHand != null && CheckDistance(m_tRightHand, out foc);
                    }

                    if (distanceCheckResult)
                    {
                        if (m_RedScreenInstance != null && m_RedScreenInstance.activeSelf == false)
                            m_RedScreenInstance.SetActive(true);
                        OnHandInappropriateDistance(foc);
                    }
                    else
                    {
                        if (m_RedScreenInstance != null && m_RedScreenInstance.activeSelf == true)
                            m_RedScreenInstance.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        //private void _UpdateApplicationLifecycle() {
        //    // Exit the app when the 'back' button is pressed.
        //    if (Input.GetKey(KeyCode.Escape)) {
        //        Application.Quit();
        //    }

        //    // Only allow the screen to sleep when not tracking.
        //    if (Session.Status != SessionStatus.Tracking) {
        //        const int lostTrackingSleepTimeout = 15;
        //        Screen.sleepTimeout = lostTrackingSleepTimeout;
        //    }
        //    else {
        //        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //    }

        //    if (m_IsQuitting) {
        //        return;
        //    }

        //    // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        //    if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
        //        _ShowAndroidToastMessage("Camera permission is needed to run this application.");
        //        m_IsQuitting = true;
        //        Invoke("_DoQuit", 0.5f);
        //    }
        //    else if (Session.Status.IsError()) {
        //        _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
        //        m_IsQuitting = true;
        //        Invoke("_DoQuit", 0.5f);
        //    }
        //}

        /// <summary>
        /// Update hand transform objects
        /// </summary>
        private void UpdateHandTransform()
        {
            if (m_tLeftHand == null)
            {
                GameObject gobj = GameObject.Find("Hand_l");
                if (gobj != null)
                {
                    m_tLeftHand = gobj.transform.Find("palm");
                }
            }
            if (m_tRightHand == null)
            {
                GameObject gobj = GameObject.Find("Hand_r");
                if (gobj != null)
                {
                    m_tRightHand = gobj.transform.Find("palm");
                }
            }
        }

        /// <summary>
        /// Check if given transform is either too far or too close to camera.
        /// </summary>
        /// <param name="t">The transform for checking</param>
        /// <param name="FarOrClose">true if too far, false if too close</param>
        /// <returns>true if the distance is inappropriate while false if the distance is fine</returns>
        private bool CheckDistance(Transform t, out bool FarOrClose)
        {
            if (m_FirstPersonCamera == null)
            {
                FarOrClose = false;
                return false;
            }

            float nearD = GlobalStates.globalConfigFile.NearLeapOutboundDistance;
            float farD = GlobalStates.globalConfigFile.FarLeapOutboundDistance;
            if (nearD == 0f && farD == 0f)
            {
                FarOrClose = false;
                return false;
            }

            Vector3 disV = t.position - m_FirstPersonCamera.transform.position;
            float dis = disV.magnitude;
            if (dis < nearD)
            {
                FarOrClose = false;
                return true;
            }
            else if (dis > farD)
            {
                FarOrClose = true;
                return true;
            }

            FarOrClose = false;
            return false;
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        //private void _DoQuit() {
        //    Application.Quit();
        //}

        ///// <summary>
        ///// Show an Android toast message.
        ///// </summary>
        ///// <param name="message">Message string to show in the toast.</param>
        //private void _ShowAndroidToastMessage(string message) {
        //    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        //    if (unityActivity != null) {
        //        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        //        unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        //        {
        //            AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
        //                message, 0);
        //            toastObject.Call("show");
        //        }));
        //    }
        //}

        /// <summary>
        /// Give a Unity plane and get corresponding ARCore plane.
        /// </summary>
        /// <param name="plane">The unity plane object</param>
        /// <returns>The corresponding DetectedPlane Object</returns>
        //public DetectedPlane GetDetectedPlaneFromUnityPlane(Transform plane) {
        //    DetectedPlaneVisualizer dpv = plane.GetComponent<DetectedPlaneVisualizer>();
        //    if (dpv != null) {
        //        DetectedPlane dp = dpv.GetDetectedPlane();
        //        return dp;
        //    }
        //    return null;
        //}

        /// <summary>
        /// It's called when user touched an ARCore Plane on the screen.
        /// </summary>
        /// <param name="hit">TrackableHit Object</param>
        public virtual void OnARPlaneHit(PortalbleHitResult hit) { }

        /// <summary>
        /// It's called when user touched an Unity Visualized Plane on the screen.
        /// </summary>
        /// <param name="hit">RaycastHit Object</param>
        public virtual void OnUnityPlaneHit(RaycastHit hit) { }

        /// <summary>
        /// It's called when user's hand stay from Leapmotion either too far or too close.
        /// </summary>
        /// <param name="FarOrClose">ture for too far while false for too close</param>
        /// 
        public virtual void OnHandInappropriateDistance(bool FarOrClose)
        {
            Text text = null;
            if (m_RedScreenInstance != null)
            {
                Transform t = m_RedScreenInstance.transform.Find("Text");
                if (t != null)
                {
                    text = t.GetComponent<Text>();
                }
            }
            // Check if the instance exists.
            if (text != null)
            {
                if (FarOrClose)
                {
                    Debug.Log("Distance Too far");
                    text.text = "Move Phone or your self closer to the hand";
                }
                else
                {
                    Debug.Log("Distance Too close");
                    text.text = "Too Close";
                }
            }
        }
    }
}
