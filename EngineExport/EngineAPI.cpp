#ifndef EDITOR_INTERFACE
#if defined(_WIN32)
// MSVC / Windows: export the symbol from the DLL.
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#else
// GCC / Clang: make the symbol publicly visible in the shared object.
// (Effective when the library is built with -fvisibility=hidden, which is
//  the build system's job; harmless otherwise.)
#define EDITOR_INTERFACE extern "C" __attribute__((visibility("default")))
#endif
#endif

#include "Common/CommonHeaders.h"
#include "Common/Id.h"
#include "ECS/GameObject.h"
#include "ECS/Components/Transform.h"

using namespace fluxion;

namespace {

	struct transform_component
	{
		flf32 position[3];
		flf32 rotation[3];
		flf32 scale[3];

		ecs::transform::init_info to_init_info() const
		{
			ecs::transform::init_info info{};

			memcpy(&info.position[0], &position[0], sizeof(info.position));
			memcpy(&info.scale[0],    &scale[0],    sizeof(info.scale));

			// The editor sends Euler angles, the engine stores a quaternion.
			const math::v4 quat = math::quat_from_euler(
				math::v3{ rotation[0], rotation[1], rotation[2] }
			);

			memcpy(&info.rotation[0], &quat.x, sizeof(info.rotation));

			return info;
		}
	};

	struct game_object_descriptor
	{
		transform_component transform;
	};
	ecs::game_object::game_object gameobject_from_id(id::id_type id) {
		return ecs::game_object::game_object{ ecs::game_object_id{ id } };
	}

} // anonymous namespace

EDITOR_INTERFACE id::id_type CreateGameObject(game_object_descriptor* e)
{
	assert(e);
	game_object_descriptor& desc{ *e };
	ecs::transform::init_info transform_info{ desc.transform.to_init_info() };
	ecs::game_object::game_object_info info{ &transform_info };

	return ecs::game_object::create_game_object(info).get_id();
}

EDITOR_INTERFACE void RemoveGameObject(id::id_type id)
{
	assert(id::is_valid(id));
	ecs::game_object::remove_game_object(gameobject_from_id(id));
}
