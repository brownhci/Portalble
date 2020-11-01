using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DummyData : MonoBehaviour
{
    private List<string> cacheMsg = new List<string>();
    string[] tlist;
    private bool cacheLoaded = false;
    private int idx = 0;
    private WSManager wsmanager;
    public bool DummyDataReplay = false;


    // Start is called before the first frame update
    void Start()
    {
        wsmanager = GameObject.Find("WebsocketManager").GetComponent<WSManager>();
        if (DummyDataReplay)
        {
            GetComponent<WSManager>().enableDummyDataReplay();
            loadCache();

        }
    }

    // Update is called once per frame
    void Update()
    {
        // fetch message to portalble
        if (cacheLoaded)
        {
            wsmanager.OnWebSocketUnityReceiveMessage(tlist[idx]);
            idx++;
        }
    }

    public void saveCache()
    {
        string path = "Assets/StreamingAssets/handdatacache.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        for (int i = 0; i < cacheMsg.Count; i++)
            writer.WriteLine(cacheMsg[i]);

        writer.Close();
    }

    public void loadCache()
    {
        string path = "Assets/StreamingAssets/SampleHandData.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        //Debug.Log();
        string t = reader.ReadToEnd();
        tlist = t.Split('\n');
        //Debug.Log(tlist.Length);
        reader.Close();

        cacheLoaded = true;

    }

    public void AddEntry(string message) {
        cacheMsg.Add(message);
    }
}
