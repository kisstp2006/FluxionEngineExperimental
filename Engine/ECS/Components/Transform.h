#pragma once
#include "ECS/ECSCommon.h"

namespace fluxion::ecs::transform {
	DEFINE_TYPED_ID(transform_id);

	struct init_info
	{
		flf32 position[3]{};
		flf32 rotation[4]{};
		flf32 scale[3]{ 1.f, 1.f, 1.f };
	};

	transform_id create_transform(const init_info& info, game_object_id gameobject_id);

	void remove_transform(transform_id id);
}
