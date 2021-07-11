#include "../include/rpct.h"

std::string rnd_uuid()
{
    // TODO: implement
    return "";
}

namespace rpct
{

RPCT::RPCT() {}
RPCT::~RPCT() {}

int32_t RPCT::GetDataTypeFromValue(any::AnyValue* value) const {
    return MethodCallArg_BuiltDataType::Unknown;
}


MethodCallArgData&& RPCT::FromArgToArgData(std::string const& name, int32_t index, any::AnyValuePtr const& value)
{
    int32_t data_type = this->GetDataTypeFromValue(value.get());
    any::AnyValuePtr converted_val = value;

    if (data_type == MethodCallArg_BuiltDataType::Unknown) {

    }
    else if (data_type == MethodCallArg_BuiltDataType::Null) {
        
    }
    else if (data_type == MethodCallArg_BuiltDataType::Buffer) {
        
    }
    else if (data_type == MethodCallArg_BuiltDataType::Float) {
        
    }
    else if (data_type == MethodCallArg_BuiltDataType::String) {
        
    }
    else if (data_type == MethodCallArg_BuiltDataType::Callback) {
        // _AddCallback and return string
        auto cb = value->GetAs<RPCTMethodCallbackPtr>();
        auto cb_name = this->_AddCallback(*cb);
        converted_val = any::make_any<std::string>(std::move(cb_name));
    }
    else {
        //
    }

    MethodCallArgData result;
    result.data_type = data_type;
    result.value = converted_val;
    result.index = index;
    result.name = name;

    return std::move(result);
}

any::AnyValuePtr RPCT::FromArgDataToArg(MethodCallArgData const* arg)
{
    any::AnyValuePtr value = arg->value;

    if (arg->data_type == MethodCallArg_BuiltDataType::Unknown) {

    }
    else if (arg->data_type == MethodCallArg_BuiltDataType::Null) {
        
    }
    else if (arg->data_type == MethodCallArg_BuiltDataType::Buffer) {
        
    }
    else if (arg->data_type == MethodCallArg_BuiltDataType::Float) {
        
    }
    else if (arg->data_type == MethodCallArg_BuiltDataType::String) {
        
    }
    else if (arg->data_type == MethodCallArg_BuiltDataType::Callback) {
        auto cb_name = arg->value->GetAs<std::string>();

        auto cb = AsCallback(([cb_name, this](size_t const& args_num, any::AnyValuePtr const* args_array) {
            this->Call(*cb_name, args_num, args_array);
        }));
        value = any::make_any(cb_name);
    }
    else {
        //
    }

    return value;
}

std::string RPCT::_AddCallback(RPCTMethodCallbackPtr const& func)
{
    std::string method_name = rnd_uuid();
    this->AddMethod(method_name, func);
    return method_name;
}

void RPCT::Call(std::string const& methodName, size_t const& args_num, any::AnyValuePtr const* args_array)
{
    std::string rid = rnd_uuid();
    std::vector<MethodCallArgData> args;
    args.reserve(args_num);

    for (size_t i = 0; i < args_num; ++i) {
        std::string argName = "";
        int32_t argIndex = i;
        int32_t dataType = this->GetDataTypeFromValue(args_array[i].get());
        args.emplace_back(this->FromArgToArgData(argName, argIndex, args_array[i]));
    }

    MethodCallData callData {
        .rid = rid,
        .method_name = methodName,
        .args = args
    };

    this->SendToRemote(&callData);
}

void RPCT::ReceiveFromRemote(MethodCallData const* method_call_data)
{
    if (this->Methods.count(method_call_data->method_name) == 0) {
        return;
    }

    const size_t args_num = method_call_data->args.size();
    std::vector<any::AnyValuePtr> converted_args(args_num);

    for (size_t i = 0; i < args_num; ++i) {
        converted_args[i] = this->FromArgDataToArg(&method_call_data->args[i]);
    }

    RPCTMethodCallbackPtr& cb = this->Methods[method_call_data->method_name];
    cb->Call(args_num, converted_args.data());
}

}