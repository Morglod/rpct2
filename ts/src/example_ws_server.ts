import { MethodCallData, RPCT } from "./core";
import { RpctJson } from "./json";
import { RpctMsgpack } from "./msgpack";
import { WebSocketServer } from "./websocket-server";

(async () => {
    var local = new RPCT();
    const wsServer = new WebSocketServer();

    wsServer.rpc = local;
    wsServer.serializationChain = RpctMsgpack.serializationChain();
    wsServer._init();

    await wsServer.waitConnection.promise;

    local.call(
        "hello",
        (part1: string, part2: string) => {
            console.log(part1 + part2);
        }
    );
})();