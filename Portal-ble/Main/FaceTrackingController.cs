using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class FaceTrackingController : MonoBehaviour, WebSocketUnityDelegate {
	// Web Socket for Unity
	private WebSocketUnity webSocket;

	public Vector2 faceTrackingScreenDims = new Vector2 (480, 320);
	private float eyeDistance = -1.0f;
	private float eyeScale = -1.0f;
	private Vector2 eyeCentroid = new Vector2(0, 0);

	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	public void OnEnable()
	{
		// Create web socket
		webSocket = new WebSocketUnity("ws://172.18.138.247:9999", this);

		// Open the connection
		webSocket.Open();
	}

	public void Update() {
	}
		
	#region WebSocketUnityDelegate implementation

	// These callbacks come from WebSocketUnityDelegate
	// You will need them to manage websocket events

	// This event happens when the websocket is opened
	public void OnWebSocketUnityOpen (string sender)
	{
		Debug.Log("WebSocket connected, "+sender);
		GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "WebSocket connected, "+sender;
	}

	// This event happens when the websocket is closed
	public void OnWebSocketUnityClose (string reason)
	{
		Debug.Log("WebSocket Close : "+reason);
		GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "WebSocket Close : "+reason;
	}

	// This event happens when the websocket received a message
	public void OnWebSocketUnityReceiveMessage (string message)
	{
		Debug.Log("Received from server : " + message);
		GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "Received from server : " + message;

		// Get string
		string[] pointsStr = message.Split(',');
		Vector2[] points = new Vector2[3];

		// Convert to points
		for (int i = 0; i < pointsStr.Length; i += 2) {
			float x = float.Parse (pointsStr [i], CultureInfo.InvariantCulture.NumberFormat);
			float y = float.Parse (pointsStr [i+1], CultureInfo.InvariantCulture.NumberFormat);
			points [i / 2] = new Vector2 (x / faceTrackingScreenDims.x, y / faceTrackingScreenDims.y) - new Vector2(0.5f, 0.5f);
		}

		// Get distance between eyes
		float d = (points [1] - points [0]).magnitude;
		float scale = 0.0622f / d;
		Vector2 centroid = (points [0] + points [1] + points [2]) / 3.0f;

		// TODO: initially set the below based on user a standard distance away from the camera
		// TODO: solve for translation of points from default using projection matrix
		// TODO: rotation based on this

		// Update parameters / camera
		if (eyeDistance < 0) {
			eyeDistance = d;
			eyeScale = scale;
			eyeCentroid = centroid;
		} else {
			transform.localPosition = new Vector3(centroid.x - eyeCentroid.x, centroid.y - eyeCentroid.y, 0) * 0.1f;
		}

	}

	// This event happens when the websocket received data (on mobile : ios and android)
	// you need to decode it and call after the same callback than PC
	public void OnWebSocketUnityReceiveDataOnMobile(string base64EncodedData)
	{
		// it's a limitation when we communicate between plugin and C# scripts, we need to use string
		byte[] decodedData = webSocket.decodeBase64String(base64EncodedData);
		OnWebSocketUnityReceiveData(decodedData);
	}

	// This event happens when the websocket did receive data
	public void OnWebSocketUnityReceiveData(byte[] data)
	{	
		int testInt1 = System.BitConverter.ToInt32(data,0);
		int testInt2 = System.BitConverter.ToInt32(data,4);;

		Debug.Log("Received data from server : " + testInt1+", "+testInt2);
		GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "Received data from server : " + testInt1+", "+testInt2;
	}

	// This event happens when you get an error@
	public void OnWebSocketUnityError (string error)
	{
		Debug.LogError("WebSocket Error : "+ error);
		GameObject.Find("NotificationText").GetComponent<TextMesh>().text = "WebSocket Error : "+ error;
	}

	#endregion
}
