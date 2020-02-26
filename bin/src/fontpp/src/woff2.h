#pragma once

#include <stdbool.h>
#include <stddef.h>

//-------------------------------------------------------------------------------------------------

struct buffer;

bool woff2enc_max_size(struct buffer in_buffer, size_t* out_size);
bool woff2enc_compress(struct buffer in_buffer, struct buffer* out_buffer);
