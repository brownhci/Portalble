using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Mediapipe.HandTracking
{
    public class Process : MonoBehaviour
    {


        private const int HAND_TRACKER_RESOLUTION = 256;

        public static Process INSTANCE { get; private set; }

        public Hand current_hand = null;
        private int hand_log_id, converter_log_id;

        [SerializeField]
        private Orientation orientation = Orientation.PORTRAIT;
        [SerializeField]
        private InputManager frame_input_manager = null;

        [SerializeField]
        Text debugText;

        private string current_coords = "0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;";

        string saveString = "";
#if UNITY_ANDROID
        private AndroidJavaObject hand_tracking;
        private HandRect current_hand_rect;
#endif

        private void Awake()
        {
            INSTANCE = this;
            LandmarkConverter.INSTANCE = LandmarkConverter.Create(orientation);
        }

        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log("Process.Start() called");
            if (ScreenLog.INSTANCE != null)
            {
                hand_log_id = ScreenLog.INSTANCE.RegisterLogID();
                converter_log_id = ScreenLog.INSTANCE.RegisterLogID();
            }
#if UNITY_ANDROID
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentUnityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            hand_tracking = new AndroidJavaObject("com.jackie.mediapipe.HandTracking", currentUnityActivity);
            hand_tracking.Call("setResolution", HAND_TRACKER_RESOLUTION);
#endif
            StartCoroutine(Quantization());
        }

        private void Update()
        {
            string text = "no hand position";
            //Debug.Log("current_hand: " + ((current_hand != null) ? current_hand.ToString() : "null"));

            if (null != current_hand)
            {
                Vector3 v3 = current_hand.Position;
                //Debug.Log("v3: " + ((v3 != null) ? v3.ToString() : "null"));
                text = "hand position: (" + v3.x + ", " + v3.y + ", " + v3.z + ")";
                string tmp = "";
                for (int i = 0; i < current_hand.GetLandmarks().Length; i++)
                {
                    //Debug.Log(current_hand.GetLandmarks());
                    tmp = tmp + System.Math.Round(current_hand.GetLandmark(i).x, 5) + ","
                        + System.Math.Round(current_hand.GetLandmark(i).y, 5)
                        + "," + System.Math.Round(current_hand.GetLandmark(i).z, 5) + ";";
                    //Debug.Log("debugText: " + debugText);
                    debugText.text = tmp;
                }
                current_coords = tmp;
                saveString = saveString + tmp + "\n";

                //ScreenLog.INSTANCE.Log(hand_log_id, text);
            }
        }

        public void save()
        {
            System.IO.File.WriteAllText(System.IO.Path.Combine(Application.persistentDataPath, "mediapipe-landmarks" + DateTime.Now.ToString("yyyy''MM''dd''HH''mm") + ".txt"), saveString);
        }

#if UNITY_EDITOR
        List<string> frameList = new List<string>();
        int frame_idx;
#endif

        public void LoadHandSkeletonFromMediaPipe()
        {
#if UNITY_EDITOR
            print("load _pos");
            string path = "Assets/StreamingAssets/mediapipe-landmarks.txt";

            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            string t = reader.ReadToEnd();
            frameList = new List<string>();
            string[] tlist = t.Split('\n');
            for (int i = 0; i < tlist.Length; i += 1)
            {
                frameList.Add(tlist[i]);
            }
            frame_idx = 0;
#endif
        }

        private void FixedUpdate()
        {
#if UNITY_ANDROID

            //Debug.Log("Process.FixedUpdate(): hand_tracking: " + ((hand_tracking != null) ? hand_tracking.ToString() : "null"));


            float[] palm_data = hand_tracking.Call<float[]>("getPalmRect");
            float[] hand_landmarks_data = hand_tracking.Call<float[]>("getLandmarks");

            //Debug.Log("got palm_data & hand_landmarks_data");

            if (null != palm_data) current_hand_rect = HandRect.ParseFrom(palm_data);

            if (null != hand_landmarks_data)
            {
                // Hand new_hand = Hand.MakeFrom(hand_landmarks_data, current_hand_rect);
                // if (null == current_hand) current_hand = new_hand;
                // else current_hand = Hand.DeVibrate(current_hand, new_hand);
                current_hand = Hand.MakeFrom(hand_landmarks_data, current_hand_rect);


            }
#endif
        }

        public IEnumerator Quantization()
        {
            while (true)
            {
                Debug.Log("Quantization");
                yield return new WaitForEndOfFrame();
                FrameInput image = frame_input_manager.GetFrameInput();
                if (null == image) continue;
                if (!LandmarkConverter.INSTANCE.Valid())
                {
                    LandmarkConverter.INSTANCE.SetInput((float)image.width / (float)image.height);
                    LandmarkConverter.INSTANCE.SetOutput((float)Screen.width / (float)Screen.height);
                }
#if UNITY_ANDROID
                hand_tracking.Call("setFrame", image.sbyte_array);
#endif
                yield return null;
            }
        }

        public Vector3 GetPosition()
        {
            if (null == current_hand) return Vector3.zero;
            return current_hand.Position;
        }

        public Vector3 GetFingerLandmark(int index)
        {
            if (null == current_hand) return Vector3.zero;
            return current_hand.GetLandmark(index);
        }

        public string[] GetCoords()
        {
#if UNITY_ANDROID
            return current_coords.Split(';');
#elif UNITY_EDITOR
            string frame = frameList[frame_idx];
            string[] coords = frame.Split(';');
            frame_idx++;
            return coords;
#endif
        }
    }
}