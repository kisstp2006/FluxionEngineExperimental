#pragma once
#include "../../Test.hpp"

using namespace fluxion;


class engine_test : public test
{
public:
	bool init() override { return true; }
	void run() override { return true; }
	void stop() override { return true; }
};