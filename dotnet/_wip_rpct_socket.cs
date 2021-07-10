// using System;
// using System.IO;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Net.Sockets;

// namespace rpct_dotnet
// {
//     public class RpctSocket
//     {
//         public RpctSocket(RPCT rpc, Socket socket, SerializationChain serializationChain)
//         {
//             this.Rpc = rpc;
//             this.Socket = socket;
//             this.SerializationChain = serializationChain;

//             this.Rpc.SendToRemote = delegate (MethodCallData methodCall) {
//                 this._sendQueue.Add(methodCall);
//             };
//         }

//         public Socket Socket;
//         public RPCT Rpc;

//         public SerializationChain SerializationChain;

//         public System.Collections.Generic.List<MethodCallData> _sendQueue = new System.Collections.Generic.List<MethodCallData>();

//         public async void PollAsync()
//         {
//             Task sendTask = this._SendQueueAsync();
//             Task recvTask = this.RecvAsync();

//             await Task.WhenAll(sendTask, recvTask);
//         }

//         public async Task _SendQueueAsync()
//         {
//             if (_sendQueue.Count != 0) {
//                 var callArr = new MethodCallDataArray {
//                     list= _sendQueue.ToArray()
//                 };
//                 _sendQueue.Clear();
//                 var buf = this.SerializationChain.ToBytes(callArr);
//                 await this.Socket.SendAsync(buf);
//             }
//         }

//         public async Task RecvAsync()
//         {
//             ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
//             WebSocketReceiveResult result= null;

//             using (var ms = new MemoryStream())
//             {
//                 do
//                 {
//                     result = await this.Socket.ReceiveAsync(buffer, CancellationToken.None);
//                     ms.Write(buffer.Array, buffer.Offset, result.Count);
//                 }
//                 while (!result.EndOfMessage);

//                 ms.Seek(0, SeekOrigin.Begin);

//                 if (result.MessageType == WebSocketMessageType.Text)
//                 {
//                     throw new Exception("Unsupported MessageType == Text");
//                 }

//                 var responseBuf = ms.ToArray();
//                 var responseCallArr = this.SerializationChain.FromBytes(responseBuf);

//                 foreach (var item in responseCallArr.list) {
//                     this.Rpc.ReceiveFromRemote(item);
//                 }
//             }
//         }
//     }
// }