#include "Test.hpp"

#define TEST_ECS 1

#if TEST_ECS


class engine_test : public test
{
public:
	bool init() override { return true; }
	void run() override { return true; }
	void stop() override { return true; }
};
#else
#error Atlease one test need to be enabled to run tests
#endif // TEST_ECS

int main() {
	engine_test test{};

	if (test.init()) {
		test.run();
	}
	test.stop();
}