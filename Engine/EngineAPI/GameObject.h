#pragma once
#include "../ECS/ECSCommon.h"
#include "TransformComponent.h"

namespace fluxion::ecs {
	DEFINE_TYPED_ID(game_object_id);
}


namespace fluxion::ecs::game_object {

	class game_object {

	public:
		constexpr explicit game_object(fluxion::ecs::game_object_id id) : _id{ id } {};
		constexpr explicit game_object() : _id{ id::invalid_id } {};
		constexpr game_object_id get_id()const { return _id; }
		constexpr bool is_valid() const { return id::isValid(_id); }


	private:
		fluxion::ecs::game_object_id _id;
	};
}