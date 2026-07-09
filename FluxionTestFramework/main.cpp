#define TEST_ECS 1

// MSVC CRT debug-heap leak tracking (Windows + MSVC, Debug only). Other
// toolchains use their own tools (Valgrind, ASan) so this is compiled out.
// _CRTDBG_MAP_ALLOC must be defined before <stdlib.h>/<crtdbg.h> so the CRT
// records __FILE__ / __LINE__ for every allocation in the leak dump.
#if defined(_MSC_VER) && defined(_DEBUG)
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#endif

#if defined(_MSC_VER)
#pragma comment(lib,"engine.lib")
#endif


#if TEST_ECS
#include "Tests/ECS/ECSTest.h"
#else
#error At least one test needs to be enabled to run tests
#endif // TEST_ECS

int main() {
#if defined(_MSC_VER) && defined(_DEBUG)
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif


    engine_test test{};

    if (test.init()) {
        test.run();
    }
    test.stop();
}
