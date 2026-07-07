#pragma once
#include"../ECS/ECSCommon.h"

namespace fluxion::ecs::transform 
{
	DEFINE_TYPED_ID(transform_id);

	class component final
	{
	public:
		constexpr explicit component(transform_id id) : _id{ id } {};
		constexpr explicit component() : _id{ id::invalid_id } {};
		constexpr transform_id get_id()const { return _id; }
		constexpr bool is_valid() const { return id::isValid(_id); }

	private:
		transform_id _id;
	};

}