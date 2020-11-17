using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble.Functions.Grab {
    /// <summary>
    /// Grabable Object, mainly used for configuration.
    /// </summary>
    [System.Serializable]
    public class Grabable : MonoBehaviour {
        public GrabableConfig m_configuration;

        /// <summary>
        /// This field is only used for initialization of GrabableConfig.
        /// </summary>
        [SerializeField]
        private int m_initialLock;

        /// <summary>
        /// Default, when using outline material.
        /// </summary>
        public Color m_selectedOutlineColor;
        public Color m_grabbedOutlineColor;

        [SerializeField]
        private bool m_useOutlineMaterial = true;

        /// <summary>
        /// Option, if the user want to use their own material.
        /// </summary>
        public Material m_selectedMaterial;
        public Material m_grabbedMaterial;

        public float m_throwPower = 5f;

        private Material m_unselectedMaterial;

        private List<GrabCollider> m_grabColliders;

        /// <summary>
        /// True if it's ready for grab, it doesn't mean the user is grabbing this.
        /// It only shows that user can grab it. e.g. the hand is in the grab collider.
        /// </summary>
        private bool m_isReadyForGrab;
        public bool IsReadyForGrab {
            get {
                return m_isReadyForGrab;
            }
        }

        /// <summary>
        /// A flag, marks whether it's in left hand grabbing queue.
        /// True for yes, false means it's in right hand grabbing queue.
        /// Use IsReadyForGrab to get if it's ready to be grabbed.
        /// </summary>
        private bool m_isLeftHanded;
        public bool IsLeftHanded {
            get {
                return m_isLeftHanded;
            }
        }

        // Use this for initialization
        void Start() {
            m_isReadyForGrab = false;
            m_isLeftHanded = false;

            if (m_configuration == null) {
                Debug.Log("Create Grab Config");
                m_configuration = new GrabableConfig(m_initialLock);
            }

            m_grabColliders = new List<GrabCollider>();
        }

        // Update is called once per frame
        void Update() {

        }

        internal void OnGrabTriggerEnter(GrabCollider notifier, bool isLeft) {
            // already waiting for grabbing. It's inavailable.
            if (IsReadyForGrab)
                return;

            Debug.Log("Grabable:On Grab Trigger Enter");
            m_isLeftHanded = isLeft;
            Grab.Instance.WaitForGrabbing(this);

            m_grabColliders.Add(notifier);
            /*
            if (GlobalLogIO.m_timeManager != null) {
                string currHand = null;

                if(m_isLeftHanded) {
                    currHand = "Left";
                } else {
                    currHand = "Right";
                }
                Debug.Log("-----------NEWWWWWW Grabable:On Grab Trigger Enter");
               // GlobalLogIO.appendWhateverToFile(GlobalLogIO.fileName, currHand, "enter", this.gameObject.name, GlobalLogIO.m_timeManager.getCurrentTime());
            }
            */
            // Trigger vibration if it's available
            if (Grab.Instance.UseVibration) {
                Vibration.Vibrate(25);
            }

            m_isReadyForGrab = true;
        }

        internal void OnGrabTriggerExit() {
            // nothing needs to be done.
            if (!IsReadyForGrab)
                return;

            Debug.Log("Grabable:On Grab Trigger Exit");
            Grab.Instance.ExitGrabbingQueue(this);

            foreach (GrabCollider gc in m_grabColliders) {
                gc.SetLock(false);
            }

            m_grabColliders.Clear();

            /*
            if (GlobalLogIO.m_timeManager != null) {
                string currHand = null;

                if (m_isLeftHanded) {
                    currHand = "Left";
                }
                else {
                    currHand = "Right";
                }
                Debug.Log("-----------NEWWWWWW Grabable:On Grab Trigger Exit");
                //GlobalLogIO.appendWhateverToFile(GlobalLogIO.fileName, currHand, "exit", this.gameObject.name, GlobalLogIO.m_timeManager.getCurrentTime());
            }

            */


            m_isReadyForGrab = false;
        }

        /// <summary>
        /// Called when user selected this obj
        /// </summary>
        internal void OnSelected() {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer != null && m_selectedMaterial != null && Grab.Instance.UseMaterialChange) {
                // if has renderer, then do material change.
                m_unselectedMaterial = renderer.material;
                if (m_useOutlineMaterial) {
                    Material newInstance = Instantiate<Material>(m_selectedMaterial);
                    newInstance.SetColor("_BodyColor", m_unselectedMaterial.color);
                    newInstance.mainTexture = m_unselectedMaterial.mainTexture;
                    if (newInstance.HasProperty("_OutlineColor")) {
                        newInstance.SetColor("_OutlineColor", m_selectedOutlineColor);
                    }
                    renderer.material = newInstance;
                } else if (m_selectedMaterial != null) {
                    renderer.material = m_selectedMaterial;
                }
            }
        }

        /// <summary>
        /// Called when user deselected this obj.
        /// </summary>
        internal void OnDeSelected() {
            // change material back.
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && m_unselectedMaterial != null) {
                renderer.material = m_unselectedMaterial;
            }
        }

        /// <summary>
        /// Called when it starts to be grabbed.
        /// </summary>
        internal void OnGrabStart() {
            Collider cd = GetComponent<Collider>();
            if (cd != null)
                cd.isTrigger = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }

            if (Grab.Instance.UseMaterialChange) {
                if (m_useOutlineMaterial) {
                    Material mat = GetComponent<Renderer>().sharedMaterial;
                    if (mat.HasProperty("_OutlineColor")) {
                        GetComponent<Renderer>().sharedMaterial.SetColor("_OutlineColor", m_grabbedOutlineColor);
                    }
                }
                else if (m_grabbedMaterial != null) {
                    GetComponent<Renderer>().material = m_grabbedMaterial;
                }
            }

            foreach(GrabCollider gc in m_grabColliders) {
                gc.SetLock(true);
            }
        }

        /// <summary>
        /// Called when it stops to be grabbed.
        /// </summary>
        internal void OnGrabStop(Vector3 releaseVelocity) {
            Collider cd = GetComponent<Collider>();
            if (cd != null)
                cd.isTrigger = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.velocity = releaseVelocity * m_throwPower;
            }

            // material back to selected
            if (Grab.Instance.UseMaterialChange) {
                if (m_useOutlineMaterial) {
                    Material mat = GetComponent<Renderer>().sharedMaterial;
                    if (mat.HasProperty("_OutlineColor")) {
                        GetComponent<Renderer>().sharedMaterial.SetColor("_OutlineColor", m_selectedOutlineColor);
                    }
                }
                else if (m_selectedMaterial != null) {
                    GetComponent<Renderer>().material = m_selectedMaterial;
                }
            }
        }

        /// <summary>
        /// Called when material change setting changed
        /// </summary>
        internal void OnMaterialConfigChanged() {
            // TODO: cancel current material
        }

        /// <summary>
        /// Check if this object is being grabbed.
        /// </summary>
        /// <returns>true for yes, false for no</returns>
        public bool IsBeingGrabbed() {
            return (Grab.Instance.GetGrabbingObject() == this);
        }
    }
}