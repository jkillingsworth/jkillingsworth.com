#include <woff2/encode.h>

extern "C" {
#include "types.h"
#include "woff2.h"
}

//-------------------------------------------------------------------------------------------------

bool woff2enc_max_size(struct buffer in_buffer, size_t* out_size)
{
    try {
        *out_size = woff2::MaxWOFF2CompressedSize(in_buffer.data, in_buffer.size);
        return true;
    }
    catch (const std::exception&) {
        return false;
    }
}

bool woff2enc_compress(struct buffer in_buffer, struct buffer* out_buffer)
{
    try {
        return woff2::ConvertTTFToWOFF2(in_buffer.data, in_buffer.size, out_buffer->data, &out_buffer->size);
    }
    catch (const std::exception&) {
        return false;
    }
}
