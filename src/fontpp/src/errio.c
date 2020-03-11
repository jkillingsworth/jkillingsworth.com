#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include "errio.h"

//-------------------------------------------------------------------------------------------------

static void fail_stop(const char* error_message)
{
    perror(error_message);
    exit(EXIT_FAILURE);
}

//-------------------------------------------------------------------------------------------------

void stderr_redirect_init(struct stderr_redirect_context* context)
{
    context->tempio_filename = strdup("./tempio-XXXXXX");
    if (context->tempio_filename == NULL)
        fail_stop("Error creating tempio filename template");

    context->tempio_fd = mkstemp(context->tempio_filename);
    if (context->tempio_fd == -1)
        fail_stop("Error creating tempio file");

    context->stderr_fd = fileno(stderr);
    if (context->stderr_fd == -1)
        fail_stop("Error getting file descriptor for stderr");

    context->duperr_fd = dup(context->stderr_fd);
    if (context->duperr_fd == -1)
        fail_stop("Error duplicating stderr file descriptor");

    if (fflush(stderr) != 0)
        fail_stop("Error flushing stderr");

    if (dup2(context->tempio_fd, context->stderr_fd) == -1)
        fail_stop("Error duplicating tempio to stderr");
}

void stderr_redirect_free(struct stderr_redirect_context* context)
{
    if (fflush(stderr) != 0)
        fail_stop("Error flushing stderr");

    if (dup2(context->duperr_fd, context->stderr_fd) == -1)
        fail_stop("Error duplicating duperr to stderr");

    if (close(context->duperr_fd) == -1)
        fail_stop("Error closing duperr file descriptor");

    if (close(context->tempio_fd) == -1)
        fail_stop("Error closing tempio file descriptor");

    if (remove(context->tempio_filename) == -1)
        fail_stop("Error removing tempio file");

    free(context->tempio_filename);

    context->tempio_filename = NULL;
    context->tempio_fd = -1;
    context->stderr_fd = -1;
    context->duperr_fd = -1;
}

//-------------------------------------------------------------------------------------------------

void stderr_redirect_copy_on_error(struct stderr_redirect_context context, bool is_error)
{
    if (is_error == false)
        return;

    if (fflush(stderr) != 0)
        fail_stop("Error flushing stderr");

    off_t size = lseek(context.tempio_fd, 0, SEEK_CUR);
    if (size == -1)
        fail_stop("Error seeking tempio current position");

    void* data = malloc(size);
    if (data == NULL)
        fail_stop("Error allocating buffer");

    if (lseek(context.tempio_fd, 0, SEEK_SET) == -1)
        fail_stop("Error seeking tempio to beginning");

    if (read(context.tempio_fd, data, size) == -1)
        fail_stop("Error reading tempio");

    if (write(context.duperr_fd, data, size) == -1)
        fail_stop("Error writing duperr");

    free(data);
}
