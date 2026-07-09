#pragma once
// GameObject.h (not just ECSCommon.h) because init_info below stores a
// detail::script_creator, which is declared there in fluxion::ecs::script::detail.
#include "ECS/GameObject.h"


namespace fluxion::ecs::script {
	struct init_info
	{
		detail::script_creator script_creator;
	};
	

	component create(const init_info& info, game_object::game_object gameobject);

	void remove(component c);
}
