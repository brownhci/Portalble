using UnityEngine;

namespace Mediapipe.HandTracking {
    public abstract class DepthSetting : MonoBehaviour {

        [SerializeField]
        private GameObject process = null, drawing = null, input = null;

        protected static DepthEstimate depth_estimate = null;

        public static DepthEstimate GetDepthEstimate() => depth_estimate;

        protected void Awake() {
            process.SetActive(false);
            drawing.SetActive(false);
            input.SetActive(false);
        }

        protected void EnableProcess() {
            Debug.Log("DepthSetting.EnableProcess() called");
            process.SetActive(true);
            drawing.SetActive(true);
            input.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}