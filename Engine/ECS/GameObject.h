#pragma once
#include"ECSCommon.h"

namespace fluxion::ecs {
#define INIT_INFO(component) namespace component { struct init_info; }
	INIT_INFO (transform)
	INIT_INFO(script)

#undef INIT_INFO
	namespace game_object {
		struct game_object_info
		{
			transform::init_info* transform{ nullptr };
			script::init_info* script{ nullptr };
		};

		game_object create(const game_object_info& info);
		void remove(game_object_id id);
		bool is_alive(game_object_id id);
	}

}