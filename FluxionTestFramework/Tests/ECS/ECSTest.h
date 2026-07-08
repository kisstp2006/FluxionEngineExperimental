#pragma once
#include "../../Test.hpp"
#include "ECS/GameObject.h"

using namespace fluxion;


class engine_test : public test
{
public:
	bool init() override { return true; }
	bool run() override { return true; }
	bool stop() override { return true; }
};
