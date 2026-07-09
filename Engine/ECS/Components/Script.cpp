#include "Script.h"

namespace fluxion::ecs::script
{
	// NOTE: stub implementations. The script storage / id system isn't built
	// yet, so create() returns an invalid component for now. These exist so
	// the engine links; fill them in when the script runtime is implemented.

	component create(const init_info& info, game_object::game_object gameobject)
	{
		assert(gameobject.is_valid());
		assert(info.script_creator);
		// TODO: allocate a script id and store info.script_creator(gameobject).
		return {};
	}

	void remove(component c)
	{
		assert(c.is_valid());
		// TODO: destroy the script instance and free its id.
	}
}
