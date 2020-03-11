#pragma once

#include <stdbool.h>

//-------------------------------------------------------------------------------------------------

struct stderr_redirect_context {
    char* tempio_filename;
    int tempio_fd;
    int stderr_fd;
    int duperr_fd;
};

void stderr_redirect_init(struct stderr_redirect_context* context);
void stderr_redirect_free(struct stderr_redirect_context* context);
void stderr_redirect_copy_on_error(struct stderr_redirect_context context, bool is_error);
