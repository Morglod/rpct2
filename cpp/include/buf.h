#pragma once

#include <stdint.h>
#include <memory>

namespace rpct
{
    class Buffer {
    public:
        uint8_t* mem = nullptr;
        int32_t size = 0;
        bool owned = true;

        inline Buffer() {}
        inline Buffer(uint8_t* mem, int32_t size, bool owned = true) : mem(mem), size(size), owned(owned) {}

        inline virtual ~Buffer() {
            if (mem && owned) {
                delete [] mem;
                mem = nullptr;
            }
        }

        inline static BufferPtr New(int32_t size) {
            return std::make_shared<Buffer>(new uint8_t[size], size);
        }

        inline static BufferPtr New() {
            return std::make_shared<Buffer>();
        }

        // unsafe!!
        inline static BufferPtr Slice(void* mem, int32_t size) {
            return std::make_shared<Buffer>(mem, size, false);
        }
    };

    typedef std::shared_ptr<Buffer> BufferPtr;
}