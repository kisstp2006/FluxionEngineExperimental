#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif

#include "Common/CommonHeaders.h"
#include "Common/Id.h"
#include "ECS/GameObject.h"
#include "ECS/Components/Transform.h"

using namespace fluxion;

namespace {

	struct transform_component
	{
		flu32 position[3];
		flu32 rotation[3];
		flu32 scale[3];


		ecs::transform::init_info to_init_info() {
			using namespace DirectX;
			ecs::transform::init_info info;

			memcpy(&info.position[0],&position[0],sizeof(flu32)*_countof(position));
			memcpy(&info.scale[0], &scale[0], sizeof(flu32) * _countof(scale));
			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };


		}
	};

	struct gane_object_descriptor
	{
		transform_component transform;
	};
}

EDITOR_INTERFACE id::id_type CreateGameObject(game_object_descriptor* e) {

}