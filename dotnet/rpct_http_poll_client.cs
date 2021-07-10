using System;
using System.Net.Http;
using System.Threading;

namespace rpct_dotnet
{
    public class HttpPollClient
    {
        public HttpPollClient(RPCT rpc, string endpoint, SerializationChain serializationChain)
        {
            this.Rpc = rpc;
            this.Endpoint = endpoint;
            this.SerializationChain = serializationChain;

            this.Rpc.SendToRemote = delegate (MethodCallData methodCall) {
                this._sendQueue.Add(methodCall);
            };
        }

        public HttpClient Client = new HttpClient();
        public RPCT Rpc;

        public string Endpoint;

        public SerializationChain SerializationChain;

        public System.Collections.Generic.List<MethodCallData> _sendQueue = new System.Collections.Generic.List<MethodCallData>();

        public async void PollAsync()
        {
            var callArr = new MethodCallDataArray {
                list= _sendQueue.ToArray()
            };
            _sendQueue.Clear();
            var buf = this.SerializationChain.ToBytes(this.Rpc, callArr);

            var response = await this.Client.PostAsync(this.Endpoint, new System.Net.Http.ByteArrayContent(buf));
            var responseBuf = await response.Content.ReadAsByteArrayAsync();
            var responseCallArr = this.SerializationChain.FromBytes(this.Rpc, responseBuf);

            foreach (var item in responseCallArr.list) {
                this.Rpc.ReceiveFromRemote(item);
            }
        }
    }
}