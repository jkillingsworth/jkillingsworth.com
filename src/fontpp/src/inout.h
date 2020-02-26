#pragma once

#include <stdbool.h>
#include <stdio.h>

//-------------------------------------------------------------------------------------------------

struct buffer;

bool input_stream_init(FILE** stream, int argc, char** argv);
void input_stream_free(FILE** stream);
bool woff2_stream_init(FILE** stream, int argc, char** argv);
void woff2_stream_free(FILE** stream);

bool input_buffer_init(struct buffer* buffer, FILE* input_stream);
void input_buffer_free(struct buffer* buffer);
bool woff2_buffer_init(struct buffer* buffer, size_t size);
void woff2_buffer_free(struct buffer* buffer);

bool woff2_output_data(FILE* stream, struct buffer buffer);
