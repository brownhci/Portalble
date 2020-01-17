//----------------------------------------------
// WebSocketUnity
// Copyright (c) 2015, Jonathan Pavlou
// All rights reserved
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

#if UNITY_WEBGL

public class WebSocketUnityWebGL : IWebSocketUnityPlatform {	
	
	delegate void CallbackReceivedBytesData(int delegatetionId, int socketInstance, int blobLength);

	[DllImport("__Internal")]
	private static extern int WebSocketInit (string url, int delegateId, string gameobjectname, CallbackReceivedBytesData dataCallback);
			
	[DllImport("__Internal")]
	private static extern int WebSocketConnect (int socketInstance);
	
	[DllImport("__Internal")]
	private static extern int WebSocketState (int socketInstance);
	
	[DllImport("__Internal")]
	private static extern void WebSocketSendData (int socketInstance, byte[] ptr, int length);
	
	[DllImport("__Internal")]
	private static extern void WebSocketSendString (int socketInstance, string text);
	
	[DllImport("__Internal")]
	private static extern void WebSocketClose (int socketInstance);	
	
	[DllImport("__Internal")]
	private static extern void WebSocketGetBlobData(int socketInstance, byte[] ptr, int length);	
	
	private int mReservedIndex = -1;
	private static List<WebSocketUnityDelegate> sDelegateObjects = new List<WebSocketUnityDelegate>();
	
	// Constructor
	// param : url of your server (for example : ws://echo.websocket.org)
	// param : the game object who will receive events
	public WebSocketUnityWebGL(string url, WebSocketUnityDelegate delegateObject, string gameObjectName){
		Uri uri = new Uri(url);		
		sDelegateObjects.Add(delegateObject);
		int delegateId = sDelegateObjects.ToArray().Length - 1;
		
		mReservedIndex = WebSocketInit(uri.ToString(), delegateId, gameObjectName, OnReceivedBytesData);
	}
	
	#region Basic features
	
	// Open a connection with the specified url
	public void Open(){
		WebSocketConnect(mReservedIndex);
	}
	
	// Close the opened connection
	public void Close(){
		WebSocketClose (mReservedIndex);
	}
	
	// Check if the connection is opened
	public bool IsOpened(){
		return WebSocketState(mReservedIndex)==1;
	}
	
	// Send a message through the connection
	// param : message is the sent message
	public void Send(string message){
		WebSocketSendString(mReservedIndex, message);
	}
	
	// Send a message through the connection
	// param : data is the sent byte array message
	public void Send(byte[] data){
		WebSocketSendData(mReservedIndex, data, data.Length);
	}
	
	#endregion
	
	[MonoPInvokeCallback(typeof(CallbackReceivedBytesData))]
	public static void OnReceivedBytesData(int delegateId, int socketInstance, int blobLength){	
		byte[] data = new byte[blobLength];
		WebSocketGetBlobData(socketInstance, data, blobLength);
		(sDelegateObjects[delegateId] as WebSocketUnityDelegate).OnWebSocketUnityReceiveData(data);
	}
	
}
#else
public class WebSocketUnityWebGL {}
#endif //#if UNITY_WEBGL