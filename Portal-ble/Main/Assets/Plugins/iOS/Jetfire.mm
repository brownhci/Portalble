#import "JFRWebSocket.h"

typedef void (*UNITY_CONNECT_CALLBACK)();
typedef void (*UNITY_DISCONNECT_CALLBACK)(char*);
typedef void (*UNITY_RECEIVEMESSAGE_CALLBACK)(char*);
typedef void (*UNITY_RECEIVEDATA_CALLBACK)(void*,unsigned long);

@interface UnityJetfire : NSObject <JFRWebSocketDelegate>
{
    @public
    UNITY_CONNECT_CALLBACK _connectCallback;
    UNITY_DISCONNECT_CALLBACK _disConnectCallback;
    UNITY_RECEIVEMESSAGE_CALLBACK _receiveMessageCallback;
    UNITY_RECEIVEDATA_CALLBACK _receiveDataCallback;
}
@property(nonatomic, strong)JFRWebSocket *socket;
@end

@implementation UnityJetfire

-(void)websocketDidConnect:(JFRWebSocket*)socket {
    NSLog(@"websocket is connected");
    _connectCallback();    
}

-(void)websocketDidDisconnect:(JFRWebSocket*)socket error:(NSError*)error {
    NSLog(@"websocket is disconnected: %@", [error localizedDescription]);
    //[self.socket connect];
    NSString* string = [error localizedDescription];
    _disConnectCallback((char *) [string UTF8String]);
}

-(void)websocket:(JFRWebSocket*)socket didReceiveMessage:(NSString*)string {
 //NSLog(@"Received text: %@", string);
    _receiveMessageCallback((char *) [string UTF8String]);
}

-(void)websocket:(JFRWebSocket*)socket didReceiveData:(NSData*)data {
//    NSLog(@"Received data: %@", data);
    
    NSUInteger len = [data length];
    
    NSLog(@"websocket dataSize: %u",len);
    if (len < 1048576){
//        Byte *byteData = (Byte*)malloc(len);
//        memcpy(byteData, [data bytes], len);
        unsigned char *byteData = (unsigned char *)[data bytes];
        _receiveDataCallback(byteData,len);
        //free(byteData);
    }
    data = nil;
}

@end


extern "C" {

    UnityJetfire *client;

    void JetfireOpen(
             const char* _path,
             UNITY_CONNECT_CALLBACK _connectCallback,
             UNITY_DISCONNECT_CALLBACK _disConnectCallback,
             UNITY_RECEIVEMESSAGE_CALLBACK _receiveMessageCallback,
             UNITY_RECEIVEDATA_CALLBACK _receiveDataCallback)
    {
        client = [[UnityJetfire alloc] init];
        client->_connectCallback = _connectCallback;
        client->_disConnectCallback = _disConnectCallback;
        client->_receiveMessageCallback = _receiveMessageCallback;
        client->_receiveDataCallback = _receiveDataCallback;
        
        NSString *path = [NSString stringWithUTF8String:_path];
        NSLog(@"connect: %@",path);
        client.socket = [[JFRWebSocket alloc] initWithURL:[NSURL URLWithString:path] protocols:@[@"chat",@"superchat"]];
        client.socket.delegate = client;
        [client.socket connect];
    }

    void JetfireConnect(){
        [client.socket connect];
    }

    void JetfireClose(){
        [client.socket disconnect];
    }

    void JetfirePing(){
        uint8_t byte = 1;
        NSData *data = [NSData dataWithBytes:&byte length:1];
        [client.socket writePing: data];
    }

    void JetfireSendMsg(const char * msg){
        [client.socket writeString: [NSString stringWithUTF8String:msg]];
    }

    void JetfireSendData(const void * bytes,int size){
        //NSLog(@"SendData %d",size);
        
        NSData *data = [NSData dataWithBytes:(const void *)bytes length:(sizeof(uint8_t) * size)];
        [client.socket writeData: data];
    }

    bool JetfireIsConnected(){
        // NSLog(@"is connection: %d", client.socket.isConnected);
        return client.socket.isConnected;
    }
}
