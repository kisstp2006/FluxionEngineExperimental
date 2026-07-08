#pragma once
#include "../../Test.h"
#include "ECS/GameObject.h"
#include "ECS/Components/Transform.h"

#include <iostream>
#include <cstdio>
#include <ctime>

using namespace fluxion;
using namespace fluxion::ecs;

// Shorter aliases for the nested types
namespace game_object_ns = fluxion::ecs::game_object;
namespace transform_ns   = fluxion::ecs::transform;

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

		transform_ns::init_info transform_info{};
		game_object_ns::game_object_info info{ &transform_info };

		while (count > 0)
		{
			++added;
			game_object_ns::game_object obj{ game_object_ns::create_game_object(info) };
			assert(obj.is_valid());
			game_objects.push_back(obj);
			assert(game_object_ns::is_alive(obj));
			--count;
		}
	}

	void remove_random()
	{
		flu32 count = rand() % 30;
		if (game_objects.size() < 1000) return;

		while (count > 0)
		{
			const flu32 index{ (flu32)rand() % (flu32)game_objects.size() };
			const game_object_ns::game_object gameobject{ game_objects[index] };
			assert(gameobject.is_valid());
			if (gameobject.is_valid()) {
				game_object_ns::remove_game_object(gameobject);
				game_objects.erase(game_objects.begin() + index);
				assert(!game_object_ns::is_alive(gameobject));
			}
			--count;
		}
	}


	utl::vector<game_object_ns::game_object> game_objects;

	flu32 added{ 0 };
	flu32 removed{ 0 };
	flu32 num_game_objects{ 0 };

	void print_results() const
	{
		std::cout << "══════════════════════════════\n";
		std::cout << "  Game Objects created : " << added << "\n";
		std::cout << "  Game Objects removed : " << removed << "\n";
		std::cout << "  Game Objects alive   : " << num_game_objects << "\n";
		std::cout << "══════════════════════════════\n";
	}
};
