#pragma once
#include "ECS/ECSCommon.h"


namespace fluxion::ecs::transform {

	struct init_info
	{
		flf32 position[3]{};
		flf32 rotation[4]{};
		flf32 scale[3]{ 1.f, 1.f, 1.f };
	};

	component create(const init_info& info, game_object::game_object gameobject);

	void remove(component c);
}
