#pragma once
#include "../ECS/ECSCommon.h"
#include "TransformComponent.h"
#include "ScriptComponent.h"


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

	private:
		fluxion::ecs::game_object_id _id;
	};
}

namespace fluxion::ecs::script {

	class game_object_script : public game_object::game_object {
	public:
		virtual ~game_object_script() = default;
		virtual void start() {}
		virtual void update(float dt) {}
		virtual void update() {}

	protected:
		// Fully-qualified type name required: inside this derived class the
		// bare name 'game_object' resolves to the base class's injected name
		// first, so 'game_object::game_object' would mean the base ctor, not
		// the type. The 'fluxion::ecs::' prefix forces the namespace lookup.
		constexpr explicit game_object_script(fluxion::ecs::game_object::game_object obj)
			: fluxion::ecs::game_object::game_object{ obj.get_id() } {}
	};

	namespace detail {
		using script_ptr = std::unique_ptr<game_object_script>;
		using script_creator = script_ptr(*)(ecs::game_object::game_object game_object);

		template<class script_class>
		script_ptr create_script(ecs::game_object::game_object obj) {
			assert(obj.is_valid());
			return std::make_unique<script_class>(obj);
		}
	} // namespace detail
}
