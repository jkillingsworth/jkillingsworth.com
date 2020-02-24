#include <stdlib.h>
#include "inout.h"
#include "types.h"
#include "woff2.h"

//-------------------------------------------------------------------------------------------------

int main(int argc, char** argv)
{
    int status = EXIT_FAILURE;
    FILE* input_stream = NULL;
    FILE* woff2_stream = NULL;
    struct buffer input_buffer = { NULL, 0 };
    struct buffer woff2_buffer = { NULL, 0 };

    if (!input_stream_init(&input_stream, argc, argv))
        goto end;

    if (!input_buffer_init(&input_buffer, input_stream))
        goto end;

    size_t max_size;

    if (!woff2enc_max_size(input_buffer, &max_size))
        goto end;

    if (!woff2_buffer_init(&woff2_buffer, max_size))
        goto end;

    if (!woff2enc_compress(input_buffer, &woff2_buffer))
        goto end;

    if (!woff2_stream_init(&woff2_stream, argc, argv))
        goto end;

    if (!woff2_output_data(woff2_stream, woff2_buffer))
        goto end;

    status = EXIT_SUCCESS;

end:
    woff2_stream_free(&woff2_stream);
    woff2_buffer_free(&woff2_buffer);
    input_buffer_free(&input_buffer);
    input_stream_free(&input_stream);

    return status;
}
