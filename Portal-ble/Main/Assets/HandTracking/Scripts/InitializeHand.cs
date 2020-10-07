using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.HandTracking
{
    public class InitializeHand : MonoBehaviour
    {
        [SerializeField]
        private GameObject depthSettingObject = null;
        private ARCore.ARCoreDepthSetting depthSetting;
        private bool doneSettingDepth = false;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("InitializeHand.Start()");
            depthSetting = depthSettingObject.GetComponent<ARCore.ARCoreDepthSetting>();
            StartCoroutine("SetDepth");
        }

        IEnumerator SetDepth()
        {
            while (!doneSettingDepth)
            {
                yield return new WaitForSeconds(0.25f);
                doneSettingDepth = depthSetting.SetDepth();
            }
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}