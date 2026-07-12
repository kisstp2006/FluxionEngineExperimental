#pragma once
#include "../ECS/ECSCommon.h"
#include "TransformComponent.h"
#include "ScriptComponent.h"
#include <string> // std::hash<std::string> for the script-registry tag below


namespace fluxion::ecs {
	DEFINE_TYPED_ID(game_object_id);
}


namespace fluxion::ecs::game_object {

	class game_object {

	public:
		constexpr explicit game_object(fluxion::ecs::game_object_id id) : _id{ id } {};
		constexpr game_object() : _id{ id::invalid_id } {};
		constexpr game_object_id get_id()const { return _id; }
		constexpr bool is_valid() const { return id::is_valid(_id); }

		transform::component transform() const;
		script::component script() const;

	private:
		fluxion::ecs::game_object_id _id;
	};
}

namespace fluxion::ecs::script {

	class game_object_scripts : public game_object::game_object {
	public:
		virtual ~game_object_scripts() = default;
		virtual void start() {}
		virtual void update(float dt) {}
		virtual void update() {}

	protected:
		// Fully-qualified type name required: inside this derived class the
		// bare name 'game_object' resolves to the base class's injected name
		// first, so 'game_object::game_object' would mean the base ctor, not
		// the type. The 'fluxion::ecs::' prefix forces the namespace lookup.
		constexpr explicit game_object_scripts(fluxion::ecs::game_object::game_object obj)
			: fluxion::ecs::game_object::game_object{ obj.get_id() } {}
	};

	namespace detail {
		using script_ptr = std::unique_ptr<game_object_scripts>;
		using script_creator = script_ptr(*)(ecs::game_object::game_object game_object);
		using string_hash = std::hash<std::string>;

		flu8 register_script(size_t, script_creator);
		script_creator get_script_creator(size_t tag);

		template<class script_class>
		script_ptr create_script(ecs::game_object::game_object obj) {
			assert(obj.is_valid());
			return std::make_unique<script_class>(obj);
		}

		// Registers a script type under the hash of its name so the editor or
		// loader can instantiate it by tag. Put REGISTER_SCRIPT(MyType) once,
		// at file scope, in the script's .cpp.
#define REGISTER_SCRIPT(TYPE)                                               \
		namespace {                                                         \
		const flu8 _reg_##TYPE                                              \
		{ fluxion::ecs::script::detail::register_script(                    \
			  fluxion::ecs::script::detail::string_hash()(#TYPE),           \
			  &fluxion::ecs::script::detail::create_script<TYPE>) };        \
		}
	} // namespace detail
}
