#pragma once

#include "./any.h"

#include <string>
#include <stdint.h>
#include <memory>
#include <vector>

namespace rpct
{

enum MethodCallArg_BuiltDataType : int32_t
{
    Unknown = 0,
    // nullptr
    Null = -2,
    // rpct::BufferPtr
    Buffer = -3,
    // float
    Float = -4,
    // std::string
    String = -5,
    // RPCTMethodCallbackPtr
    Callback = -6,
    // TODO: unordered_map<string, string>
    StringDictionary = -7,
};

struct MethodCallArgData
{
    std::string name;
    int32_t index;
    int32_t data_type;
    any::AnyValuePtr value;
};

struct MethodCallData
{
    std::string rid;
    std::string method_name;
    std::vector<MethodCallArgData> args;
};

struct MethodCallDataArray
{
    std::vector<MethodCallData> list;
};

}
