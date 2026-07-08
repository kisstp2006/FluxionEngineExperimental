#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif

#include "Common/CommonHeaders.h"
#include "Common/Id.h"
#include "ECS/GameObject.h"
#include "ECS/Components/Transform.h"

using namespace fluxion;

namespace {

}

EDITOR_INTERFACE id::id_type CreateGameObject(game_object_descriptor* e) {

}