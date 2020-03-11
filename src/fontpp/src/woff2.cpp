#include <woff2/encode.h>

extern "C" {
#include "errio.h"
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
    bool success;

    struct stderr_redirect_context context = { NULL, -1, -1, -1 };
    stderr_redirect_init(&context);

    try {
        success = woff2::ConvertTTFToWOFF2(in_buffer.data, in_buffer.size, out_buffer->data, &out_buffer->size);
    }
    catch (const std::exception&) {
        success = false;
    }

    stderr_redirect_copy_on_error(context, !success);
    stderr_redirect_free(&context);

    return success;
}
