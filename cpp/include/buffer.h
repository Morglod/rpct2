#include <stdint.h>

namespace rpct
{

class RPCTBuffer {
public:
    int8_t* buf;
    int32_t size;

    inline ~RPCTBuffer() {
        delete [] this->buf;
    }
};

}