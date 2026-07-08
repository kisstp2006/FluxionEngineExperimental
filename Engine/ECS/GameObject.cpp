#include "GameObject.h"
#include "Components/Transform.h"

namespace fluxion::ecs {

	namespace {
		utl::vector<transform::component> transforms;
		utl::vector<id::generation_type> generations;
		utl::deque<game_object_id> free_ids;
	}

	namespace game_object {


		
		game_object create_game_object(const game_object_info& info)
		{
			assert(info.transform); //ALL game objects must have a transform component (fo now)
			if (!info.transform) return game_object{};

			game_object_id id;
			
			if (free_ids.size() > id::min_deleted_elements) 
			{
				id = free_ids.front();
				assert(!is_alive(game_object{ id }));
				free_ids.pop_front();
				id = game_object_id{ id::new_generation(id) };
				++generations[id::index(id)];
			}
			else 
			{
				id = game_object_id{ (id::id_type)generations.size() };
				generations.push_back(0);

				transforms.emplace_back();
			}

			const game_object new_game_object{ id };
			const id::id_type index{ id::index(id) };

			// For now we need to create a transform component with every game object
			assert(!transforms[index].is_valid());
			transforms[index] = transform::create_transform(*info.transform, new_game_object);

			if (!transforms[index].is_valid()) return {};

			return new_game_object;
		}

		void remove_game_object(game_object gameobject)
		{
			const game_object_id id{gameobject.get_id()};
			const id::id_type index{ id::index(id) };
			assert(is_alive(gameobject));
			if (is_alive(gameobject))
			{
				transform::remove_transform(transforms[index]);
				transforms[index] = {}; // reset the slot so is_alive() reports dead and create can reuse it
				free_ids.push_back(id);
			}

		}

		bool is_alive(game_object gameobject)
		{
			assert(gameobject.is_valid());
			const game_object_id id{ gameobject.get_id() };
			const id::id_type index{ id::index(id) };
			assert(index < generations.size());
			// Alive = generation matches AND the transform slot is in use
			// (every game object must have a transform for now).
			return (generations[index] == id::generation(id) && transforms[index].is_valid());


		}

		transform::component
			game_object::transform() const 
		{
			assert(is_alive(*this));
			assert(is_valid());
			const id::id_type index{ id::index(_id) };
			


			return transforms[index];
		}
	}
}
