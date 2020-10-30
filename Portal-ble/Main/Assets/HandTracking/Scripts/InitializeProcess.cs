using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.HandTracking
{
    public class InitializeProcess : MonoBehaviour
    {
        public ARCore.ARCoreDepthSetting depthSetting;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("InitializeProcess.Start()");
            StartCoroutine("SetDepth");
        }

        IEnumerator SetDepth()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.25f);
                if (depthSetting.SetDepth()) break;
            }
        }
    }
}