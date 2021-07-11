#pragma once

#include "./core.h"
#include "./any.h"
#include "./callback.h"

#include <unordered_map>
#include <memory>
#include <string>

namespace rpct
{

class RPCT
{
public:
    std::function<void (MethodCallData* data)> SendToRemote;
    std::unordered_map<std::string, RPCTMethodCallbackPtr> Methods;

    int32_t GetDataTypeFromValue(any::AnyValue* value) const;

    MethodCallArgData&& FromArgToArgData(std::string const& name, int32_t index, any::AnyValuePtr const& value);
    any::AnyValuePtr FromArgDataToArg(MethodCallArgData const* arg);

    void AddMethod(std::string const& methodName, RPCTMethodCallbackPtr const& func);
    std::string _AddCallback(RPCTMethodCallbackPtr const& func);

    void Call(std::string const& methodName, size_t const& args_num, any::AnyValuePtr const* args_array);

    void ReceiveFromRemote(MethodCallData const* method_call_data);

    RPCT();
    virtual ~RPCT();
};

}

inline void rpct::RPCT::AddMethod(std::string const& methodName, RPCTMethodCallbackPtr const& func)
{
    this->Methods[methodName] = func;
}