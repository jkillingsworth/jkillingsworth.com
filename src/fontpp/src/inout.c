#include <stdlib.h>
#include "inout.h"
#include "types.h"

//-------------------------------------------------------------------------------------------------

static void report_error(const char* message)
{
    fprintf(stderr, message);
    fprintf(stderr, "\n");
}

//-------------------------------------------------------------------------------------------------

bool input_stream_init(FILE** stream, int argc, char** argv)
{
    const int index = 1;

    if (argc <= index) {
        report_error("No filename provided for input stream.");
        return false;
    }

    *stream = fopen(argv[index], "rb");

    if (*stream == NULL) {
        report_error("Error opening input stream.");
        return false;
    }

    return true;
}

void input_stream_free(FILE** stream)
{
    if (*stream == NULL)
        return;

    if (fclose(*stream) != 0) {
        report_error("Error closing input stream.");
        return;
    }

    *stream = NULL;
}

//-------------------------------------------------------------------------------------------------

bool woff2_stream_init(FILE** stream, int argc, char** argv)
{
    const int index = 2;

    if (argc <= index) {
        report_error("No filename provided for output stream.");
        return false;
    }

    *stream = fopen(argv[index], "wb");

    if (*stream == NULL) {
        report_error("Error opening output stream.");
        return false;
    }

    return true;
}

void woff2_stream_free(FILE** stream)
{
    if (*stream == NULL)
        return;

    if (fclose(*stream) != 0) {
        report_error("Error closing output stream.");
        return;
    }

    *stream = NULL;
}

//-------------------------------------------------------------------------------------------------

bool input_buffer_init(struct buffer* buffer, FILE* stream)
{
    size_t realloc_size = 4;

    while (feof(stream) == 0) {
        uint8_t* data = realloc(buffer->data, realloc_size);

        if (data == NULL) {
            report_error("Error allocating input buffer.");
            return false;
        }

        buffer->data = data;

        uint8_t* ptr = buffer->data + buffer->size;
        size_t items = realloc_size - buffer->size;
        size_t n = fread(ptr, sizeof(*ptr), items, stream);

        if (ferror(stream) != 0) {
            report_error("Error reading input stream.");
            return false;
        }

        buffer->size += n;
        realloc_size *= 2;
    }

    return true;
}

void input_buffer_free(struct buffer* buffer)
{
    free(buffer->data);
    buffer->data = NULL;
    buffer->size = 0;
}

//-------------------------------------------------------------------------------------------------

bool woff2_buffer_init(struct buffer* buffer, size_t size)
{
    uint8_t* data = malloc(size);

    if (data == NULL) {
        report_error("Error allocating output buffer.");
        return false;
    }

    buffer->data = data;
    buffer->size = size;

    return true;
}

void woff2_buffer_free(struct buffer* buffer)
{
    free(buffer->data);
    buffer->data = NULL;
    buffer->size = 0;
}

//-------------------------------------------------------------------------------------------------

bool woff2_output_data(FILE* stream, struct buffer buffer)
{
    uint8_t* ptr = buffer.data;
    size_t items = buffer.size;
    fwrite(ptr, sizeof(*ptr), items, stream);

    if (ferror(stream) != 0) {
        report_error("Error writing output stream.");
        return false;
    }

    return true;
}
