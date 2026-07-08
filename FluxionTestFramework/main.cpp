#define TEST_ECS 1

#if TEST_ECS
#include "Tests/ECS/ECSTest.h"
#else
#error At least one test needs to be enabled to run tests
#endif // TEST_ECS

int main() {
	engine_test test{};

	if (test.init()) {
		test.run();
	}
	test.stop();
}
