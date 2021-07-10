import { MethodCallData, RPCT } from "./core";
import { SerializationChain } from "./io";
import http from 'http';
import { server as WsServer, connection as WsConn } from "websocket";
import { promiseUnwrap, PromiseUnwrap } from "morglod_mystd/lib/promise-unwrap";

export class WebSocketServer {
    rpc!: RPCT;
    serializationChain!: SerializationChain;
    server!: http.Server;
    websocketServer!: WsServer;
    websocketCon!: WsConn;
    waitConnection!: PromiseUnwrap<void>;

    _init = (port = 8080) => {
        this.waitConnection = promiseUnwrap();
    
        this.rpc.sendToRemote = this.handleSendRemote;
        this.server = http.createServer();
        this.websocketServer = new WsServer({
            httpServer: this.server,
            autoAcceptConnections: false,
        });
        this.server.listen(port, () => {
            console.log('listenting')
        });

        this.websocketServer.on('request', req => {
            const con = req.accept();
            this.websocketCon = con;

            con.on("message", data => {
                const buf = data.binaryData!;
                const msgs = this.serializationChain.fromBytes(this.rpc, buf);

                for (const cl of msgs.list) {
                    this.rpc.receiveFromRemote(cl);
                }
            });

            this.waitConnection.resolve();
        });
    };

    handleSendRemote = (callData: MethodCallData) => {
        if (!this.websocketCon) return;

        const buf = this.serializationChain.toBytes(this.rpc, { list: [ callData ] });
        this.websocketCon.sendBytes(Buffer.from(buf));
    };
}