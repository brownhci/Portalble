using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portalble {
    /// <summary>
    /// The class for distance indicator using arc length style.
    /// </summary>
    public class DistanceIndicatorArc : IDistanceIndicator {
        public AudioSource notifySound;
        public LineRenderer distanceLine;
        public Slider arcSlider;
        private Image sliderColor;

        protected override void Start() {
            base.Start();
            scaleFactor = 0.01f;
            sliderColor = this.transform.Find("DistanceSlider").transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>();
        }

        // Update is called once per frame
        protected override void Update() {
            // Make it look towards camera
            Camera cam = Camera.main;
            if (cam != null) {
                transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
            }

            // If it's tracking, find distance
            /* what is this tracking object?*/
            if (trackingObject != null) {
                // Debug.Log("tracking:" + trackingObject.position + "&" + handLT.position);
                float minDis = 99999f;
                // float arcDis = (trackingObject.position - cam.transform.position).magnitude;

                bool isLeftNear = true;
                if (handLT != null) {
                    minDis = (trackingObject.position - handLT.position).magnitude;
                }
                if (handRT != null) {
                    float tmpDis = (trackingObject.position - handRT.position).magnitude;
                    // Same logic, since if handLT doesn't exist, minDis is -1, where tmpDis is impossible to be negative
                    if (tmpDis < minDis) {
                        minDis = tmpDis;
                        isLeftNear = false;
                    }
                }

                // pitch varies depends on distance
                notifySound.pitch = 4f / (1f + minDis * 10f);

                // Debug.Log(minDis);
                minDis *= 1000f;
                // arcDis *= 1000f;

                // Set Arc
                float maxDis = arcSlider.maxValue;
                float upperbound = 0.95f * maxDis;
                float currentValue = Mathf.Clamp(minDis, 0f, upperbound);

                // Calculate Rotation degrees
                float degree = (1f - currentValue / maxDis) * 180f;
                arcSlider.transform.localRotation = Quaternion.Euler(0f, 0f, degree);
                arcSlider.value = maxDis - currentValue;

                //Debug.Log(handLT);
                //Debug.Log(handRT);
                /* current min is 120, max is 1200 */
                arcSlider.value = maxDis - currentValue;
                // TODO, let Jing design the color...
                sliderColor.color = MapColor(arcSlider.value, sliderColor.color, MapTransparency(arcSlider.value, 0, 4800));

                // Draw lines
                distanceLine.positionCount = 2;
                distanceLine.SetPosition(0, trackingObject.position);
                if (isLeftNear) {
                    distanceLine.SetPosition(1, handLT.position);
                }
                else {
                    distanceLine.SetPosition(1, handRT.position);
                }
            }

            base.Update();
        }

        private float MapTransparency(float v, int low, int high) {
            float _v = Mathf.Pow((v+200) / (high - low),2) + 0.2f;
            if (_v > 1)
                return 1;
            else
                return _v;
        }

        
        private Color MapColor(float v, Color c, float transparency) {
            //Debug.Log("distance: " + v);
            if (v > (4800 - 600)) {
                c.b += 0.05f;
                if (c.b >= 1)
                    c.b = 1;
            }

            if (v < (4800-600)) {
                c.b -= 0.1f;
                if (c.b < 0.29f)
                    c.b = 0.29f;
            }
            return new Color(c.r,c.g,c.b, transparency);
        }

        public override void UpdateConfig(IndicatorManager.DI_CONFIG config) {
            arcSlider.gameObject.SetActive(config.useSphereText);
            notifySound.gameObject.SetActive(config.useSound);
            distanceLine.gameObject.SetActive(config.useLine);
        }
    }
}