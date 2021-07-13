#pragma once

#include "./any.h"
#include <memory>
#include "repeat.h"
#include <functional>

namespace rpct
{

class RPCTMethodCallback
{
public:
    virtual void Call(size_t arg_num, rpct::any::AnyValuePtr* array) {}

    RPCTMethodCallback() {}
    virtual ~RPCTMethodCallback() {}
};

typedef std::shared_ptr<RPCTMethodCallback> RPCTMethodCallbackPtr;

class RPCTMethodCallbackFunc : public RPCTMethodCallback
{
public:
    std::function<void (size_t args_num, any::AnyValuePtr* args_array)> _method;

    inline void Call(size_t arg_num, rpct::any::AnyValuePtr* array) override {
        this->_method(arg_num, array);
    }

    RPCTMethodCallbackFunc(std::function<void (size_t args_num, any::AnyValuePtr* args_array)> const& func) : _method(func) {}
    virtual ~RPCTMethodCallbackFunc() {}
};

inline RPCTMethodCallbackPtr AsCallback(std::function<void (size_t args_num, any::AnyValuePtr* args_array)> const& func)
{
    return std::shared_ptr<RPCTMethodCallback>((RPCTMethodCallback*)(new RPCTMethodCallbackFunc(func)));
}

}

// #define RPCT_CALLBACK_ARG_AT(CC_N) , args[CC_N]

// #define RPCT_CALLBACK_CASE_CALL(AA_NUM) \
//     case AA_NUM: \
//         raw_ptr({ \
//             args[0] \
//             RPCT_REPEAT_J(AA_NUM, RPCT_CALLBACK_ARG_AT) \
//         }); \
//     break;

// void rpct_call_callback(rpct::RPCTMethodCallbackRawFunc raw_ptr, size_t arg_num, rpct::any::AnyValuePtr* args)
// {
//     switch (arg_num)
//     {
//     RPCT_REPEAT(5, RPCT_CALLBACK_CASE_CALL)
    
//     default:
//         // throw "rpct_call_callback unsupported args num";
//         break;
//     }

// }

// #undef RPCT_CALLBACK_ARG_AT
// #undef RPCT_CALLBACK_CASE_CALL

