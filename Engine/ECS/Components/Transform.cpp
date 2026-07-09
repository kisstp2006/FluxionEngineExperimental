#include "Transform.h"
#include"../../ECS/GameObject.h"

namespace fluxion::ecs::transform 
{
	namespace {
		utl::vector<math::v3> positions;
		utl::vector<math::v4> rotations;
		utl::vector<math::v3> scales;

	}//private-like namespace

	component create(const init_info& info, game_object::game_object gameobject) {
		assert(gameobject.is_valid());
		const id::id_type game_object_index{ id::index(gameobject.get_id()) };

		if (positions.size() > game_object_index) {
			positions[game_object_index] = math::v3(info.position);
			rotations[game_object_index] = math::v4(info.rotation);
			scales[game_object_index] = math::v3(info.scale);
		}
		else 
		{
			assert(positions.size() == game_object_index);
			positions.emplace_back(info.position);
			rotations.emplace_back(info.rotation);
			scales.emplace_back(info.scale);
		}


		// Slot index == game object index in both branches
		// (the append branch asserts size() == game_object_index).
		return component(transform_id{ game_object_index });
	}

	void remove(component c) {

		assert(c.is_valid());

	}
	math::v3 component::position() const {
		assert(is_valid());
		return positions[id::index(_id)];
	}

	math::v4 component::rotation() const {
		assert(is_valid());
		return rotations[id::index(_id)];
	}

	math::v3 component::scale() const {
		assert(is_valid());
		return scales[id::index(_id)];
	}
}