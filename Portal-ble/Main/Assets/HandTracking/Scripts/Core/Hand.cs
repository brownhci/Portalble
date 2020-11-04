using UnityEngine;

namespace Mediapipe.HandTracking {
    public partial class Hand {
        private const float Z_MAX_DISTANCE_LANDMARK = 80f;

        private float length_size;
        private Vector3[] normalize_landmarks;
        private static Vector3[] m_normalize_landmarks;
        private Vector3[] landmarks;
        private HandRect hand_rect;

        public Vector3 Position { get; private set; }
        public Vector3 GetLandmark(int index) => this.landmarks[index];
        public Vector3[] GetLandmarks() => this.landmarks;

        private Hand() {
            this.length_size = 0;
            this.normalize_landmarks = null;
            this.landmarks = null;
        }

        private Hand(Vector3 position, Vector3[] landmarks) {
            this.Position = position;
            this.landmarks = landmarks;
        }

        private Hand(Vector3[] landmarks) {
            this.Position = landmarks[0];
            this.landmarks = landmarks;
        }

        public static Hand MakeFrom(float[] arr_landmark_data, HandRect hand_rect) {
            if (null == arr_landmark_data || arr_landmark_data.Length < 63) return null;
            Vector3[] normalize_landmarks = new Vector3[21];
            m_normalize_landmarks = new Vector3[21];
            if (LandmarkConverter.INSTANCE == null || !LandmarkConverter.INSTANCE.Valid()) return null;
            for (int i = 0; i < 21; i++)
            {
                normalize_landmarks[i] = LandmarkConverter.INSTANCE.Convert(arr_landmark_data[3 * i], arr_landmark_data[3 * i + 1], arr_landmark_data[3 * i + 2]);
                m_normalize_landmarks[i] = LandmarkConverter.INSTANCE.Convert(arr_landmark_data[3 * i], arr_landmark_data[3 * i + 1], arr_landmark_data[3 * i + 2]);
            }
            
            Builder builder = new Builder(DepthSetting.GetDepthEstimate());
            builder.Set(hand_rect);
            builder.Set(normalize_landmarks);
            return builder.Build();
        }


        /* return all the landmarks */
        public static Vector3[] GetLandMarks() {
            return m_normalize_landmarks;
        }

        private class Builder {
            // define các cặp điểm đốt ngón tay.
            private static int[,] finger_couples = { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 4 }, { 0, 5 }, { 5, 6 }, { 6, 7 }, { 7, 8 }, { 0, 9 }, { 9, 10 }, { 10, 11 }, { 11, 12 }, { 0, 13 }, { 13, 14 }, { 14, 15 }, { 15, 16 }, { 0, 17 }, { 17, 18 }, { 18, 19 }, { 19, 20 } };

            private DepthEstimate depth_estimate;
            private Hand hand;
            private HandRect hand_rect;
            private Vector3[] normalize_landmarks;

            public Builder(DepthEstimate depth_estimate) {
                this.depth_estimate = depth_estimate;
                this.hand = new Hand();
                this.hand.landmarks = new Vector3[21];
            }

            public void Set(HandRect hand_rect) => this.hand_rect = hand_rect;
            public void Set(Vector3[] normalize_landmarks) => this.normalize_landmarks = normalize_landmarks;

            public Hand Build() {
                this.hand.normalize_landmarks = normalize_landmarks;


                // visualize the landmarks
                
                //  Calculates the hand magnitude(by the sum of the knuckles) as normalize
                for (int i = 0; i < 20; i++) this.hand.length_size += Vector3.Magnitude(normalize_landmarks[finger_couples[i, 1]] - normalize_landmarks[finger_couples[i, 0]]);

                if (!depth_estimate.Valid()) return null;

                // calculate projection ratio before estimating depth
                depth_estimate.PredictZoomIndicator(hand_rect, this.hand.length_size);

                for (int i = 0; i < 21; i++) {
                    this.hand.landmarks[i] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * this.hand.normalize_landmarks[i].x, Screen.height * this.hand.normalize_landmarks[i].y, this.depth_estimate.PredictDepth(this.hand.normalize_landmarks[i].z)));
                }

                this.hand.Position = this.hand.landmarks[0];
                return this.hand;
            }
        }
    }
}