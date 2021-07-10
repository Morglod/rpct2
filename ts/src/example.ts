import { MethodCallData, RPCT } from "./core";

(() => {
    var local = new RPCT();
    var remote = new RPCT();

    local.sendToRemote = (data: MethodCallData) => {
        remote.receiveFromRemote(data);
    };
    remote.sendToRemote = (data: MethodCallData) => {
        local.receiveFromRemote(data);
    };

    remote.addMethod("hello", (remote_cb: any) => {
        remote_cb("Hello ", "World!");
    });

    local.call(
        "hello",
        (part1: string, part2: string) => {
            console.log(part1 + part2);
        }
    );
})();