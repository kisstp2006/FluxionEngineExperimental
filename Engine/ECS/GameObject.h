#pragma once
#include"ECSCommon.h"

namespace fluxion::ecs {
#define INIT_INFO(component) namespace component { struct init_info; }
	INIT_INFO (transform)
#undef INIT_INFO
	namespace game_object {
		struct game_object_info
		{
			transform::init_info* transform{ nullptr };
		};

		game_object_id create_game_object(const game_object_info& info);
		void remove_game_object(game_object_id id);
		bool is_alive(game_object_id id);
	}
}