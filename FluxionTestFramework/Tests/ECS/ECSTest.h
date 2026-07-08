#pragma once
#include "../../Test.h"
#include "ECS/GameObject.h"
#include "ECS/Components/Transform.h"

#include <iostream>
#include <cstdio>
#include <ctime>

using namespace fluxion;
using namespace fluxion::ecs;

// Every test header defines a class named engine_test so main.cpp
// can instantiate the one selected by the TEST_* define.
class engine_test : public test
{
public:
	bool init() override
	{
		srand((flu32)time(nullptr));
		return true;
	}

	bool run() override
	{
		do {
			for (flu32 i{ 0 }; i < 1000; ++i) {
				create_random();
				remove_random();
				num_game_objects = (flu32)game_objects.size();
			}
		} while (getchar() != 'q');

		return true;
	}

	bool stop() override
	{
		return true;
	}

private:
	void create_random()
	{
		flu32 count = rand() % 30;
		if (game_objects.empty())
		{
			count = 1000;
		}

		transform::init_info transform_info{};
		game_object::game_object_info info{ &transform_info };

		while (count > 0)
		{
			++added;
			game_object::game_object obj{ game_object::create_game_object(info) };
			assert(obj.is_valid());
			game_objects.push_back(obj);
			--count;
		}
	}

	void remove_random()
	{
		// TODO: remove a random batch of game objects
	}

	utl::vector<game_object::game_object> game_objects;

	flu32 added{ 0 };
	flu32 removed{ 0 };
	flu32 num_game_objects{ 0 };
};
