#define TEST_ECS 1

#if _DEBUG
// Must be defined before including stdlib.h / crtdbg.h so the CRT
// records __FILE__ / __LINE__ for every allocation in the leak dump.
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#endif

#pragma comment(lib,"engine.lib")


#if TEST_ECS
#include "Tests/ECS/ECSTest.h"
#else
#error At least one test needs to be enabled to run tests
#endif // TEST_ECS

int main() {
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	
	engine_test test{};

	if (test.init()) {
		test.run();
	}
	test.stop();
}
