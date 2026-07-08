#pragma once
#include "../../Test.hpp"
#include "ECS/GameObject.h"
#include "ECS/Components/Transform.h"

#include<iostream>
#include<ctime>


using namespace fluxion;
using namespace fluxion::ecs;


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
				create_ramdom();
				remove_ramdom();
				num_game_objects = (flu32)gameobjects.size();
			}

		} while (getchar() != 'q');
		
		return true; 
	}
	bool stop() override { 
		return true; 
	}

private:
	void create_ramdom()
	{
		flu32 count = rand() % 30;
		if (gameobjects.empty())
		{
			count = 1000;
		}
		transform::init_info transfom_info{};
		game_object::game_object_info gameobject_info{
			&transfom_info,

		
		};
		while (count<0)
		{
			++added;
			game_object::game_object gameobject{ game_object::create_game_object(gameobject_info) };
			assert(gameobject.is_valid());
			gameobjects.push_back(gameobject);
			--count;
		}
	}
	void remove_ramdom()
	{

	}


	utl::vector<game_object::game_object> gameobjects;

	flu32 added{ 0 };
	flu32 removed{ 0 };
	flu32 num_game_objects{ 0 };

};
