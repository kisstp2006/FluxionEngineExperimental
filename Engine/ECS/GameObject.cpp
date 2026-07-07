#include "GameObject.h"

namespace fluxion::ecs {

	namespace {
		utl::vector<id::generation_type> generations;
		utl::deque<game_object_id> free_ids;
	}

	namespace game_object {

		game_object_id create_game_object(const game_object_info& info)
		{
			assert(info.transform); //ALL game objects must have a transform component (fo now)
			if (!info.transform) return game_object_id{};
			return game_object_id{ id::invalid_id };
		}

		void remove_game_object(game_object_id id)
		{
			// TODO: implement
		}

		bool is_alive(game_object_id id)
		{
			// TODO: implement
			return false;
		}
	}
}
