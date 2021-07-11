#include "../include/rpct.h"

using namespace rpct;

int main() {
    RPCT local;
    RPCT remote;

    local.SendToRemote = [&remote](MethodCallData* data) {
        remote.ReceiveFromRemote(data);
    };

    remote.SendToRemote = [&local](MethodCallData* data) {
        local.ReceiveFromRemote(data);
    };

    remote.AddMethod("hello", AsCallback([](size_t args_num, any::AnyValuePtr* args) {
        any::AnyValuePtr args2[] {
            any::make_any(std::string("hello"))
        };
        args[0]->GetAsRef<RPCTMethodCallbackPtr>()->Call(1, args2);
    }));

    any::AnyValuePtr args2[] {
        any::make_any(AsCallback([](size_t args_num, any::AnyValuePtr* args) {
            printf("world %s\n", args[0]->GetAs<std::string>()->c_str());
        }))
    };
    local.Call("hello", 1, args2);

    return 0;
}