//Copyright (c) 2016 Jonathan Pavlou

var WebSocketUnityJsLib = {
$instances: [],

	WebSocketInit: function(url, delegateId, obj, dataCallback)
	{
		var socket = {
			socket: null,
			blobDataArray: [],
			callbacks: {},
			delegateId: delegateId,
			url: Pointer_stringify(url),
			gameobjectname : Pointer_stringify(obj)
		}	
		socket.callbacks.dataReceived = dataCallback;
		var instance = instances.push(socket) - 1;
		return instance;
	},

	WebSocketConnect: function(socketInstance)
	{
		var socket = instances[socketInstance];
		socket.socket = new WebSocket(socket.url);	
		
		socket.socket.onopen = function(evt){ 
			SendMessage(socket.gameobjectname, 'OnWebSocketUnityOpen', 'websocketwebgl');	
		}; 
		
		socket.socket.onclose = function (evt) {
			SendMessage(socket.gameobjectname, 'OnWebSocketUnityClose', evt.code.toString());	
		};
		
		socket.socket.onerror = function (evt) {
			var errorMsg = evt.data;
			console.log("[websocket error] "+errorMsg);
			SendMessage(socket.gameobjectname, 'OnWebSocketUnityError', errorMsg);	
		};
		
		socket.socket.onmessage = function (e) {
			//console.log(e);
			if (e.data instanceof Blob)
			{
				// blob message : need to store it and notify the websocketunityWebgl manager
				var reader = new FileReader();
				reader.addEventListener("loadend", function() {
					var array = new Uint8Array(reader.result);
					socket.blobDataArray.push(array);
	        		Runtime.dynCall('viii', socket.callbacks.dataReceived, [socket.delegateId, socketInstance, array.length]);
				});
				reader.readAsArrayBuffer(e.data);
			}
			else
			{
				// string message : we can send directly
				SendMessage(socket.gameobjectname, 'OnWebSocketUnityReceiveMessage', e.data);	
			}
		};
	},
	
	WebSocketClose: function (socketInstance)
	{
		var socket = instances[socketInstance];
		socket.socket.close();
	},
	
	WebSocketState: function (socketInstance)
	{
		var socket = instances[socketInstance];
		if ((typeof socket == "undefined") || (socket == null) )
			return 0;
		return socket.socket.readyState;
	},
	
	WebSocketSendString: function (socketInstance, text)
	{
		var socket = instances[socketInstance];
		socket.socket.send (Pointer_stringify(text));
	},
	
	WebSocketSendData: function (socketInstance, ptr, length)
	{
		var socket = instances[socketInstance];
		socket.socket.send (HEAPU8.buffer.slice(ptr, ptr+length));
	},
	
	WebSocketGetBlobData: function (socketInstance, ptr, length)
	{
		var socket = instances[socketInstance];
		if (socket.blobDataArray.length == 0)
			return 0;
		if (socket.blobDataArray[0].length > length)
			return 0;
		HEAPU8.set(socket.blobDataArray[0], ptr);
		socket.blobDataArray = socket.blobDataArray.slice(1);
	}
	
};

autoAddDeps(WebSocketUnityJsLib, '$instances');
mergeInto(LibraryManager.library, WebSocketUnityJsLib);
