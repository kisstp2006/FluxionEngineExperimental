#include "EntryPoint.h"
#include <cstdio>

int main(int argc, char** argv)
{
    auto app = Engine::CreateApplication();
    if (!app)
    {
        fprintf(stderr, "FATAL: CreateApplication() returned null.\n");
        return -1;
    }

    int exitCode = app->Run(argc, argv);

    // app is a unique_ptr — it will be destroyed here, calling ~Application()
    return exitCode;
}
