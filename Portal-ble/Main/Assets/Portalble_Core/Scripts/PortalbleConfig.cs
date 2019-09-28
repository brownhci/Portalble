using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Portalble {
    /// <summary>
    /// Provided calibration function for Portalble
    /// </summary>
    public class PortalbleConfig {
        private string m_ConfigFileName = "portalble-config";
        private int m_version = 0;

        private Vector3 m_HandOffset;
        private float m_MeshHandScale;
        private float m_nearDis;
        private float m_farDis;

        private bool m_isAvailable;
        private bool m_useKalman;
        /// <summary>
        /// Constructor
        /// </summary>
        public PortalbleConfig() {
            m_isAvailable = false;
            ReadConfig();
        }

        /// <summary>
        /// Getter, check if it's available;
        /// </summary>
        public bool Available {
            get {
                return m_isAvailable;
            }
        }

        /// <summary>
        /// Getter, check if it's available;
        /// </summary>
        public bool useKalman
        {
            get
            {
                return m_useKalman;
            }
            set {
                m_useKalman = true;
            }
        }

        /// <summary>
        /// Hand Offset
        /// </summary>
        public Vector3 HandOffset {
            get {
                return m_isAvailable ? m_HandOffset : Vector3.zero;
            }
            set {
                m_HandOffset = value;
            }
        }

        /// <summary>
        /// Mesh Hand Scale Factor
        /// </summary>
        public float MeshHandScale {
            get {
                return m_isAvailable ? m_MeshHandScale : 1.0f;
            }
            set {
                if (value >= 0f) {
                    m_MeshHandScale = value;
                }
            }
        }

        /// <summary>
        /// Near Outbound distance. (For Leapmotion losing tracking)
        /// </summary>
        public float NearLeapOutboundDistance {
            get {
                return m_isAvailable ? m_nearDis : 0f;
            }
            set {
                m_nearDis = value;
            }
        }

        /// <summary>
        /// Far Outbound distance. (For Leapmotion losing tracking)
        /// </summary>
        public float FarLeapOutboundDistance {
            get {
                return m_isAvailable ? m_farDis : 0f;
            }
            set {
                m_farDis = value;
            }
        }

        /// <summary>
        /// Get config file path
        /// </summary>
        /// <returns>Config file path</returns>
        public string GetFilePath() {
            string config_path;
            if (Application.isMobilePlatform) {
                config_path = Path.Combine(Application.persistentDataPath, m_ConfigFileName);
            }
            else {
                config_path = Path.Combine(Application.streamingAssetsPath, m_ConfigFileName);
            }
            return config_path;
        }

        /// <summary>
        /// Read config file
        /// </summary>
        /// <returns>true for success, false for failure</returns>
        public bool ReadConfig() {
            if (IsConfigFileExist() == false) {
                return false;
            }

            string config_path = GetFilePath();
            // Try to read it.
            try {
                using (StreamReader sr = new StreamReader(config_path)) {
                    int file_version = int.Parse(sr.ReadLine());
                    // Read HandOffsetVector
                    string handoffsetv = sr.ReadLine();
                    string[] comps = handoffsetv.Split(new char[1] { ',' });
                    for (int i = 0; i < 3; ++i) {
                        m_HandOffset[i] = float.Parse(comps[i]);
                    }
                    // Read HandScaleFactor
                    m_MeshHandScale = float.Parse(sr.ReadLine());
                    // Read Near, Far distance
                    m_nearDis = float.Parse(sr.ReadLine());
                    if (m_nearDis < 0f)
                        m_nearDis = 0f;
                    m_farDis = float.Parse(sr.ReadLine());
                    if (m_farDis < m_nearDis)
                        m_farDis = m_nearDis + 1f;

                    m_isAvailable = true;
                    return true;
                }
            }
            catch (System.Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// Save the config to the file
        /// </summary>
        /// <returns></returns>
        public bool SaveConfig() {
            // Check if it's an available config file. If not, do not write.
            if (!RefreshAvailability()) {
                Debug.LogWarning("Trying to write an invalid configuration into config file.");
                return false;
            }
            string config_path = GetFilePath();
            try {
                using (StreamWriter sw = new StreamWriter(config_path)) {
                    // Write version code
                    sw.WriteLine(m_version);
                    // Write HandOffsetVector
                    sw.WriteLine(TurnVector3ToString(m_HandOffset));
                    // Write Hand Size Factor
                    sw.WriteLine(m_MeshHandScale);
                    // Near and Far size factor
                    sw.WriteLine(m_nearDis);
                    sw.WriteLine(m_farDis);
                    return true;
                }
            }
            catch (System.Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// Check if config file exists
        /// </summary>
        /// <returns>true for exist, false for not exist</returns>
        public bool IsConfigFileExist() {
            string config_path;
            if (Application.isMobilePlatform) {
                config_path = Path.Combine(Application.persistentDataPath, m_ConfigFileName);
            }
            else {
                config_path = Path.Combine(Application.streamingAssetsPath, m_ConfigFileName);
            }

            return File.Exists(config_path);
        }

        /// <summary>
        /// Check all field and refresh availability.
        /// </summary>
        /// <returns></returns>
        public bool RefreshAvailability() {
            m_isAvailable = true;
            if (m_MeshHandScale < 0f)
                m_isAvailable = false;
            if (m_nearDis < 0f)
                m_isAvailable = false;
            if (m_farDis <= m_nearDis)
                m_isAvailable = false;
            return m_isAvailable;
        }

        /// <summary>
        /// Turn a vector3 to a string
        /// </summary>
        /// <param name="vec">Vector to be casted</param>
        /// <returns>The result string</returns>
        private string TurnVector3ToString(Vector3 vec) {
            string[] comp = new string[3];
            for (int i = 0; i < 3; ++i) {
                comp[i] = vec[i].ToString();
            }
            return string.Join(",", comp);
        }
    }
}