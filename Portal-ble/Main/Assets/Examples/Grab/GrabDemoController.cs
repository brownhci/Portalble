using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble;

namespace Portalble.Examples.BasicGrab {
    public class GrabDemoController : PortalbleGeneralController {
        public Transform placePrefab;
        public float offset = 0.15f;

        public override void OnARPlaneHit(PortalbleHitResult hit) {
            base.OnARPlaneHit(hit);

            if (placePrefab != null) {
                Instantiate(placePrefab, hit.Pose.position + hit.Pose.rotation * Vector3.up * offset, hit.Pose.rotation);
            }
        }
    }
}