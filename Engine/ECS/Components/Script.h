#pragma once
// EngineAPI/GameObject.h because init_info below stores a detail::script_creator,
// which is declared there in fluxion::ecs::script::detail (alongside the
// game_object class and game_object_script).
#include "EngineAPI/GameObject.h"


namespace fluxion::ecs::script {
	struct init_info
	{
		detail::script_creator script_creator;
	};
	

	component create(const init_info& info, game_object::game_object gameobject);

	void remove(component c);
}
