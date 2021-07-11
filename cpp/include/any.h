#pragma once

#include <cstddef>
#include <memory>
#include <typeinfo>

namespace rpct {
namespace any {

class AnyValue
{
public:
    void* _raw;

    template<typename T>
    T* GetAs() const {
        return (T*)this->_raw;
    }

    template<typename T>
    T& GetAsRef() const {
        return *((T*)this->_raw);
    }

    virtual size_t GetSize() const = 0;
    virtual const std::type_info&& GetTypeInfo() = 0;

    inline AnyValue(void* raw) : _raw(raw) {}
    virtual ~AnyValue() {}
};

template<typename T>
class AnyValueT : public AnyValue
{
public:
    T _value;

    size_t GetSize() const override {
        return sizeof(T);
    }

    const std::type_info&& GetTypeInfo() override {
        return std::move(typeid(T));
    }

    AnyValueT(T const& value) : _value(value), AnyValue(&this->_value) {}
    virtual ~AnyValueT() {}
};

typedef std::shared_ptr<AnyValue> AnyValuePtr;

template<typename T>
inline AnyValuePtr make_any(T const& value)
{
    return std::make_shared<AnyValueT<T>>(value);
}

}} //namespace any } rpct }
