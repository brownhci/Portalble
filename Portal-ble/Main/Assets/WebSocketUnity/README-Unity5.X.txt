//----------------------------------------------
// WebSocketUnity
// Copyright (c) 2015, Jonathan Pavlou
// All rights reserved
//----------------------------------------------

-------------------
1. Web Socket Unity
-------------------

Thank you for buying WebSocketUnity!

This package provides code to use client websocket with only one implementation for all platforms.
Currently it works on Desktop (PC/MAC), WebPlayer, WebGL (only with Unity 5), iOS (device or simulator) and Android.
I didn't test on  Linux but it should be the same than PC or Mac.

It does NOT require Unity Pro.

Don't forget to read the part 2 (Important notes to use this package) to know what you need
to do to work on different platforms. I used existing implementation for each platforms, so you 
can add or extend platforms if you want, for example you will be able to add Win8 platforms.

You can contact me if you want to speak about important updates on the implementation (support@pavlou.fr), I will 
try to add it if I have the time.

> Update 1.3 (2016/01)
 - Add WebGL support (only with Unity 5)
 - Editor will use Desktop implementation with all targets to be able to work in Editor

> Update 1.2 (2015/05)
 - Compatible with Unity 5
 

--------------------------------------
2. Important notes to use this package
--------------------------------------

Desktop : nothing specific to do

WebPlayer :
You can't use ws://echo.websocket.org because you need specific server to manage connection with WebPlayer (you can read :
http://docs.unity3d.com/Manual/SecuritySandbox.html to understand). You can use this server for example :
http://code.google.com/p/flashpolicyd/ but there are other implementations on internet.
After you need a custom websocket server, there are lot of implementations in Ruby on internet. To validate my tests (with
bytes message) I used : https://github.com/willryan/em-websocket.

WebGL : nothing specific to do

iOS (device and simulator) : nothing specific to do

Android :
In PlayerSettings, you need to choose "require" in the field : Internet Access.


-----------
3. Features
-----------

Functions :
open   				desktop  webplayer  webGL  ios  android
open wss (basic)	        desktop	 webplayer  webGL   ios		
close 				desktop  webplayer  webGL  ios  android
isOpen 				desktop  webplayer  webGL  ios  android
send string			desktop  webplayer  webGL  ios  android
send byte                     desktop  webplayer  webGL  ios  android

Callbacks :
onMessage string              desktop  webplayer  webGL  ios  android
onMessage byte                desktop  webplayer  webGL  ios  android
onOpen				desktop  webplayer  webGL  ios  android
onClose				desktop  webplayer  webGL  ios  android
onError				desktop  webplayer  webGL  ios  android

Note : basic WSS connection : currently, I don't manage parameters for the connection, only direct opening.

---------
4. Sample
---------

You can check TestWebSocketController.cs file for the full demo.

// first step : we need to create the WebSocket
//      You can use a websocketURL (ws://XXXXXXX or wss://XXXXXXXXX)
//		You need to give a MonoBehaviour object who will receive events
//		This object must extend WebSocketUnityDelegate interface
//      For example : TestWebSocketController inherits of WebSocketUnityDelegate
//		You can create your own websocket server
//		for example : https://github.com/willryan/em-websocket
// Warning : some libraries (for example on ios) don't support reconnection
//         so you need to create a new websocketunity before each connect (destroy your object when receive a disconnect or an error)
webSocket = new WebSocketUnity("ws://echo.websocket.org", this); // <= public server
//webSocket = new WebSocketUnity("ws://localhost:8080", this); // <= local server

// Second Step : we open the connection
webSocket.Open();

// Third Step : we need to close the connection when we finish
webSocket.Close();

// Fourth Step : we can send message
//    In this sample, we are waiting a "echo" message from the server
//    when we will receive this message, we will be able to change the display
webSocket.Send("Hello World");

// Fifth Step : we can send Data
//    In this sample, we are waiting a "echo" message from the server
//    when we will receive this message, we will be able to change the display
// You need a server which manages bytes message

int test1 = 42;
int test2 = 33;
byte[] data = new byte[8];

byte[] testB1 = System.BitConverter.GetBytes(test1);
byte[] testB2 = System.BitConverter.GetBytes(test2);

testB1.CopyTo(data,0);
testB2.CopyTo(data,4);

webSocket.Send(data);


After you need to implement the code inside your callback to manage all events (see the demo file).



