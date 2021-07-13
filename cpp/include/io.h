#pragma once

#include "./core.h"
#include "./buf.h"

namespace rpct
{
    class RPCT;

    class IBinarySerializer
    {
    public:
        inline virtual BufferPtr ToBytes(RPCT* rpct, MethodCallDataArray* data) = 0;
        inline virtual MethodCallDataArray FromBytes(RPCT* rpct, BufferPtr const& buf) = 0;
        inline virtual ~IBinarySerializer() {}
    };

    typedef std::shared_ptr<IBinarySerializer> IBinarySerializerPtr;

    class ISerializationChainPart
    {
    public:
        inline virtual BufferPtr To(RPCT* rpct, BufferPtr const& buf) = 0;
        inline virtual BufferPtr From(RPCT* rpct, BufferPtr const& buf) = 0;
        inline virtual ~ISerializationChainPart() {}
    };

    typedef std::shared_ptr<ISerializationChainPart> ISerializationChainPartPtr;
    typedef std::vector<ISerializationChainPartPtr> ISerializationChain;

    class SerializationChain : public IBinarySerializer
    {
        IBinarySerializerPtr Serializer;
        ISerializationChain Chain;

        inline SerializationChain(IBinarySerializerPtr const& serializer, ISerializationChain const& Chain) : Serializer(serializer), Chain(Chain)
        {
        }

        inline BufferPtr ToBytes(RPCT* rpct, MethodCallDataArray* data) override {
            auto buf = this->Serializer->ToBytes(rpct, data);
            for (auto chainPart : this->Chain) {
                buf = chainPart->To(rpct, buf);
            }
            return buf;
        }

        inline MethodCallDataArray FromBytes(RPCT* rpct, BufferPtr const& data) override {
            auto buf = data;
            for (size_t i = this->Chain.size() -1; i >= 0; --i) {
                buf = this->Chain[i]->From(rpct, buf);
            }
            return this->Serializer->FromBytes(rpct, buf);
        }

        inline virtual ~SerializationChain() {}
    };
}