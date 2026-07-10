#include "Script.h"
#include "../GameObject.h"
#include <unordered_map> // std::unordered_map for the script registry

namespace fluxion::ecs::script
{
	namespace {

		utl::vector<detail::script_ptr>      game_object_script_list;
		utl::vector<id::id_type>             id_mapping;

		utl::vector<id::generation_type>     generations;
		// free_ids must be a deque: create() reads front() and removes the
		// SAME element with pop_front(). (A vector + pop_back would remove a
		// different id than the one read.)
		utl::deque<script_id>                free_ids;

		using script_registry = std::unordered_map<size_t, detail::script_creator>;

		// NOTE: the registry lives inside a function so it is guaranteed to be
		// initialized before first use, regardless of static-init order.
		script_registry&
		registry()
		{
			static script_registry reg;
			return reg;
		}

		bool
		exists(script_id id)
		{
			assert(id::is_valid(id));
			const id::id_type index{ id::index(id) };
			assert(index < generations.size() && id_mapping[index] < game_object_script_list.size());
			assert(generations[index] == id::generation(id));

			return (generations[index] == id::generation(id))
				&& game_object_script_list[id_mapping[index]]
				&& game_object_script_list[id_mapping[index]]->is_valid();
		}
	} // anonymous namespace

	namespace detail {

		flu8
		register_script(size_t tag, script_creator func)
		{
			bool result{ registry().insert(script_registry::value_type{ tag, func }).second };
			assert(result);
			return result;
		}

		script_creator
		get_script_creator(size_t tag)
		{
			auto script = registry().find(tag);
			assert(script != registry().end() && script->first == tag);
			return script->second;
		}
	} // namespace detail

	component
	create(const init_info& info, game_object::game_object gameobject)
	{
		assert(gameobject.is_valid());
		assert(info.script_creator);

		script_id id{};
		if (free_ids.size() > id::min_deleted_elements)
		{
			id = free_ids.front();
			assert(!exists(id));
			free_ids.pop_front();
			id = script_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		else
		{
			id = script_id{ (id::id_type)id_mapping.size() };
			id_mapping.emplace_back();
			generations.push_back(0);
		}

		assert(id::is_valid(id));
		// Index is the slot the new script WILL occupy, i.e. the current size
		// taken BEFORE emplace_back (taking it after would be off by one).
		const id::id_type index{ (id::id_type)game_object_script_list.size() };
		game_object_script_list.emplace_back(info.script_creator(gameobject));
		assert(game_object_script_list.back()->get_id() == gameobject.get_id());
		id_mapping[id::index(id)] = index;
		return component{ id };
	}

	void
	remove(component c)
	{
		assert(c.is_valid() && exists(c.get_id()));
		const script_id id{ c.get_id() };
		const id::id_type index{ id_mapping[id::index(id)] };
		const script_id last_id{ game_object_script_list.back()->script().get_id() };
		utl::erase_unordered(game_object_script_list, index);
		id_mapping[id::index(last_id)] = index;
		id_mapping[id::index(id)] = id::invalid_id;
	}

	void
	update(float dt)
	{
		for (auto& ptr : game_object_script_list)
		{
			ptr->update(dt);
		}
	}
}
