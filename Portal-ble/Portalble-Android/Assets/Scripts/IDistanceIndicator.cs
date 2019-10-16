using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portalble {
    public abstract class IDistanceIndicator : MonoBehaviour {
        protected Transform handLT;
        protected Transform handRT;
        protected Transform trackingObject;

        protected float outofdateValue = 0f;
        protected float scaleFactor = 0.012f;

        // Here, find Hand we need
        protected virtual void Start() {
            GameObject gobj = GameObject.Find("Hand_l");
            if (gobj != null)
                handLT = gobj.transform.Find("palm");
            gobj = GameObject.Find("Hand_r");
            if (gobj != null)
                handRT = gobj.transform.Find("palm");
        }

        protected virtual void Update() {
            // Refresh Status
            // Set size
            if (trackingObject != null) {
                MeshFilter mf = trackingObject.GetComponent<MeshFilter>();
                if (mf != null) {
                    Bounds bds = mf.mesh.bounds;
                    // Find the max one
                    float maxvalue = 0f;
                    for (int i = 0; i < 3; ++i) {
                        if (maxvalue < bds.size[i])
                            maxvalue = bds.size[i];
                    }

                    transform.position = trackingObject.position + bds.center;
                    // Adjust size
                    float factor = scaleFactor * maxvalue;

                    transform.localScale = trackingObject.localScale * factor;
                    /* huristic method for our study */

                    //float factor = 0.001f;
                    /* hard-coded, Xiangyu please fix the ratio */
                    //float factor = scaleFactor * maxvalue;
                    //transform.localScale = new Vector3(factor, factor, factor);
                }
                else {
                    transform.position = trackingObject.position;
                }
            }

            // out of date check
            outofdateValue -= Time.deltaTime;
            if (outofdateValue <= 0f) {
                outofdateValue = 0f;
                gameObject.SetActive(false);
            }
        }

        /* what does it do */

        public virtual void SetToAnInteractiveObject(Transform t, float activeTime = 1.2f) {
            // Set position
            trackingObject = t;

            outofdateValue = activeTime;
            gameObject.SetActive(true);
        }

        public virtual void RefreshActiveTime(float time = 1.0f) {
            outofdateValue = time;
        }

        public abstract void UpdateConfig(IndicatorManager.DI_CONFIG config);
    }
}