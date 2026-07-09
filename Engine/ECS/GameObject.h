#pragma once
#include"ECSCommon.h"

namespace fluxion::ecs {
#define INIT_INFO(component) namespace component { struct init_info; }
	INIT_INFO (transform)
	INIT_INFO(script)

#undef INIT_INFO
	namespace game_object {
		struct game_object_info
		{
			transform::init_info* transform{ nullptr };
			script::init_info* script{ nullptr };
		};

		game_object create_game_object(const game_object_info& info);
		void remove_game_object(game_object id);
		bool is_alive(game_object gameobject);
	}
	namespace script {     
		class game_object_script : public game_object::game_object {
		public:
			virtual ~game_object_script() = default;
			virtual void start() {

			}
			virtual void update(float dt) {

			}
			virtual void update() {

			}
		protected:
			// Fully-qualified type name required: inside this derived class the
			// bare name 'game_object' resolves to the base class's injected name
			// first, so 'game_object::game_object' would mean the base ctor, not
			// the type. The 'fluxion::ecs::' prefix forces the namespace lookup.
			constexpr explicit game_object_script(fluxion::ecs::game_object::game_object obj)
				: fluxion::ecs::game_object::game_object{ obj.get_id() } {}
		};

		namespace detail{
			using script_ptr = std::unique_ptr<game_object_script>;
			using script_creator = script_ptr(*)(ecs::game_object::game_object game_object);

			template<class script_class>
			script_ptr create_script(ecs::game_object::game_object obj) {
				assert(obj.is_valid());
				return std::make_unique<script_class>(obj);
			}
		}// detail namespace
	}
}