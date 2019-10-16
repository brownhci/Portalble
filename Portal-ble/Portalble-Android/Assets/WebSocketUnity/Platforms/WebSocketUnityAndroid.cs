//----------------------------------------------
// WebSocketUnity
// Copyright (c) 2015, Jonathan Pavlou
// All rights reserved
//----------------------------------------------

using UnityEngine;
using System.Collections;

#if UNITY_ANDROID
public class WebSocketUnityAndroid : IWebSocketUnityPlatform {
	
	private AndroidJavaObject mWebSocket;
	
	// Constructor
	// param : url of your server (for example : ws://echo.websocket.org)
	// param : gameObjectName name of the game object who will receive events
	public WebSocketUnityAndroid(string url, string gameObjectName){
		object[] parameters = new object[2];
		parameters[0] = url;
		parameters[1] = gameObjectName;
		mWebSocket = new AndroidJavaObject("com.jonathanpavlou.WebSocketUnity", parameters);
	}
	
	#region Basic features
	
	// Open a connection with the specified url
	public void Open(){
		mWebSocket.Call("connect");
	}
	
	// Close the opened connection
	public void Close(){
		mWebSocket.Call("close");
	}
	
	// Check if the connection is opened
	public bool IsOpened(){
		return mWebSocket.Call<bool>("isOpen");
	}
	
	// Send a message through the connection
	// param : message is the sent message
	public void Send(string message){
		mWebSocket.Call("send", message);
	}
	
	// Send a message through the connection
	// param : data is the sent byte array message
	public void Send(byte[] data){
		mWebSocket.Call("send", data);
	}
	
	
	#endregion

}
#else
public class WebSocketUnityAndroid {}
#endif // UNITY_ANDROID
