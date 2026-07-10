#include "Script.h"
#include "../GameObject.h"

namespace fluxion::ecs::script
{
	// NOTE: stub implementations. The script storage / id system isn't built
	// yet, so create() returns an invalid component for now. These exist so
	// the engine links; fill them in when the script runtime is implemented.

	namespace {
		utl::vector<detail::script_ptr>      game_object_script_list;
		utl::vector<id::id_type>             id_mapping;

		utl::vector <id::generation_type>    generations;
		utl::vector <script_id>              free_ids;

		bool exsists(script_id id) {
			assert(id::is_valid(id));
			const id::id_type index{ id::index(id) };
			assert(index < generations.size() && id_mapping[index] < game_object_script_list.size());
			assert(generations[index] == id::generation(id));

			return (generations[index] == id::generation(id))
				&& game_object_script_list[id_mapping[index]]
				&& game_object_script_list[id_mapping[index]]->is_valid();
		}
	}
	namespace detail {
		flu8 register_script(size_t tag, script_creator func) {

		}
	}


	component create(const init_info& info, game_object::game_object gameobject)
	{
		assert(gameobject.is_valid());
		assert(info.script_creator);

		script_id id{};
		if (free_ids.size() > id::min_deleted_elements) 
		{
			id = free_ids.front();
			assert(!exsists(id));
			free_ids.pop_back();
			id = script_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		else 
		{
			id = script_id{ (id::id_type)id_mapping.size() };
			id_mapping.emplace_back(id::invalid_id);
			generations.push_back(0);
		}

		assert(id::is_valid(id));

		id_mapping[id::index(id)] = (id::id_type)game_object_script_list.size();
		game_object_script_list.emplace_back(info.script_creator(gameobject));

		const id::id_type index{ (id::id_type)game_object_script_list.size() };
		id_mapping[id::index(id)] = index;
		return component{ id };
	}

	void remove(component c)
	{
		assert(c.is_valid() && exsists(c.get_id()));
		const script_id id{ c.get_id() };
		const id::id_type index{ id_mapping[id::index(id)] };
		const script_id last_id{ game_object_script_list.back()->script().get_id() };
		utl::erase_unordered(game_object_script_list, index);
		id_mapping[id::index(last_id)] = index;
		id_mapping[id::index(id)] = id::invalid_id;
	}
}
