using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Mediapipe.HandTracking.ARCore {
    public class ARCoreDepthSetting : DepthSetting {

        [SerializeField]
        private ARRaycastManager raycast_manager = null;

        private List<ARRaycastHit> out_hits = new List<ARRaycastHit>();
        private Pose current_pose = default;
        private float current_distance = default;

        private new void Awake() {
            // instantiate depth estimate object
            depth_estimate = new DepthEstimate();
            base.Awake();
        }

        public void FixedUpdate() {
            if (raycast_manager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), out_hits, TrackableType.PlaneWithinPolygon)) {
                current_pose = out_hits[0].pose;
                current_distance = out_hits[0].distance;
            }
        }

        public bool SetDepth() {
            if (current_distance == default) return false;
            depth_estimate.default_depth = current_distance;
            base.EnableProcess();
            return true;
        }
    }
}