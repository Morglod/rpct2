using System;
using System.Threading.Tasks;

namespace rpct_dotnet
{
    class Program
    {
        static void MainLoopback(string[] args)
        {
            var local = new RPCT();
            var remote = new RPCT();

            local.SendToRemote = (MethodCallData data) => {
                remote.ReceiveFromRemote(data);
            };
            remote.SendToRemote = (MethodCallData data) => {
                local.ReceiveFromRemote(data);
            };

            remote.AddMethod("hello", RPCT.AsCallback(delegate (DCallback remote_cb) {
                remote_cb.Invoke("Hello ", "World!");
            }));

            local.Call(
                "hello",
                RPCT.AsCallback(delegate (string part1, string part2) {
                    Console.WriteLine(part1 + part2);
                })
            );
        }
        static async Task MainWsClient(string[] args)
        {
            var local = new RPCT();
            var ws = new System.Net.WebSockets.ClientWebSocket();
            await ws.ConnectAsync(new Uri("ws://127.0.0.1:8080/"), System.Threading.CancellationToken.None);

            var wsRpc = new RpctWebsocket(local, ws, RpctMsgpack.SerializationChain());

            local.AddMethod("hello", RPCT.AsCallback(delegate (DCallback remote_cb) {
                remote_cb.Invoke("Hello ", "World!");
            }));

            while(ws.State != System.Net.WebSockets.WebSocketState.Closed) {
                await wsRpc.PollAsync();
                System.Threading.Thread.Sleep(10);
            }
        }

        static void Main(string[] args)
        {
            MainWsClient(args).GetAwaiter().GetResult();
        }
    }
}
