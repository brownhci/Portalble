using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Portalble {
    [CustomEditor(typeof(GrabableObject))]
    public class GrabableObjectEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            GrabableObject gobj = (GrabableObject)target;
            if (GUILayout.Button("Generate")) {
                gobj.GenerateGrabObject();
            }
        }
    }
}